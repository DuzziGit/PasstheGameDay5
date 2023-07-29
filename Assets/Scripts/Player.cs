using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour, IHittable
{
    Rigidbody2D rb;

    [Header("Stats")]
    [SerializeField]
    private int maxHealth;
    private int curHealth;

    [Header("Movement")]
    [SerializeField]
    private float moveSpeed;
    Vector2 moveAxis, lookAxis;
    [SerializeField] private PlayerControls controls;
    private InputAction move, look, fire, earlyReload, roll;
	public GameObject head;
	public Animator animator;
    [Header("Shooting")]
    [SerializeField] GameObject[] projectileObjects;
    [SerializeField] private Transform weapon, firePoint;
    [SerializeField] private float rotSpeed = 8f;
    [SerializeField] private WandController wandController;


    [Header("Ammo")]
    public List<int> gunChamber = new List<int> { 2, 2, 2, 2, 2, 5 };
    private int currentShot;
    private bool canBeDamaged;

    //Dash/Roll
    private bool canDash = false;
    private bool isDashing = false;
    private float dashingPower = 15f;
    private float dashingTime = 0.8f;
    private float dashingCooldown = 2.0f;

    [Header("Effects")]
    [SerializeField] private SpriteRenderer spriteRend;
    private Material defaultMat, flashMat;
    [SerializeField] private float yAmp = 0.1f, yFrq = 16f;
    [SerializeField] private float aniMoveSpeed;

    private GameObject gameManager;
    private UIManager uiScript;

    // Shooting variables
    private Vector2 shootingDirection;
    [SerializeField] private GameObject bulletPrefab;

    private void Awake()
    {
        SoundManagerScript.PlaySound("encounter_loop");
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        //Locates GameManager object in scene and gets reference to UI script.
        gameManager = GameObject.Find("GameManager");
        uiScript = gameManager.GetComponent<UIManager>();
        uiScript.AmmoUpdate(gunChamber);
        wandController = GetComponentInChildren<WandController>();

        curHealth = maxHealth;
        controls = new PlayerControls();
        currentShot = 0;
    }

    private void OnEnable()
    {
        move = controls.Player.Move;
        move.Enable();

        /*
        earlyReload = controls.Player.Reload;
        earlyReload.Enable();
        earlyReload.performed += EarlyReload;
        */

        roll = controls.Player.Roll;
        roll.Enable();
        roll.performed += Roll;

        fire = controls.Player.Fire;
        fire.Enable();
        fire.performed += Fire;
    }

    private void OnDisable()
    {
        //  move.Disable();
        //fire.Disable();
    }

    private void Start()
    {
        Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera.Follow = this.gameObject.transform;
        canDash = true;
    }

     private void Update()
{
    // Calculate the direction from the player to the mouse position
    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    shootingDirection = mousePosition - (Vector2)firePoint.position;

    // Normalize the shooting direction vector to get a unit vector
    shootingDirection.Normalize();

    moveAxis = move.ReadValue<Vector2>();
    UpdateAnimator();

    // Check if the mouse is inside the wand controller's radius
    float distanceToMouse = Vector2.Distance(mousePosition, transform.position); // Use 'transform' instead of 'player'
    if (distanceToMouse <= wandController.radius)
    {
        // The mouse is inside the radius, prevent the player from firing
        if (fire.enabled)
            fire.Disable();
    }
    else
    {
        // The mouse is outside the radius, allow the player to fire
        if (!fire.enabled)
            fire.Enable();
    }
}
    public void Reload(int min, int max)
    {
        SoundManagerScript.PlaySound("reload");
        gunChamber.Clear();
        for (int i = 0; i < 6; i++)
        {
            gunChamber.Add(Random.Range(min, max));
            uiScript.AmmoUpdate(gunChamber);
            currentShot = 0;
        }
        firePoint.DOPunchRotation(new Vector3(0, 0, 361), .25f);
    }

    public void Roll(InputAction.CallbackContext context)
    {
        if (canDash)
        {
            StartCoroutine(Dash());
        }
    }

    void UpdateAnimator()
    {
        Vector2 latSpeed = rb.velocity;
        aniMoveSpeed = Vector3.SqrMagnitude(latSpeed);
        if (aniMoveSpeed > 0)
        {
            float yPos = Mathf.Sin(Time.time * yFrq) * yAmp;
            spriteRend.gameObject.transform.localPosition = new Vector3(0, yPos, 0);
        }
        else
            spriteRend.gameObject.transform.localPosition = Vector3.zero;

        var direction = Mathf.Sign(lookAxis.x);
        spriteRend.transform.localScale = new Vector3(0.4f * direction, 0.4f, 1f);
        firePoint.transform.localScale = new Vector3(0.4f * direction, 0.4f, 1f);
    }
private void Fire(InputAction.CallbackContext context)
{
    if (currentShot == 6)//reload
        {
            Reload(1, 6);
        }
        else//fire
        {
            SoundManagerScript.PlaySound("fire");

          GameObject proj = Instantiate(projectileObjects[gunChamber[0]], firePoint.transform.position, weapon.rotation);
        proj.GetComponent<Projectile>().ChangeDirection(shootingDirection.normalized);

        gunChamber.RemoveAt(0);
        weapon.DOPunchRotation(new Vector3(0, 0, 60f), 0.12f);
        uiScript.RotateBarrel(currentShot);
        currentShot++;
    }
}

    public void DoDamage(int damage)
    {
        curHealth -= damage;
        if (curHealth < 1)
        {
            canBeDamaged = false;
            StartCoroutine(HitVisual());

            curHealth -= damage;
            uiScript.HealthChange(curHealth);
            Debug.Log("current hp =  " + curHealth);
            if (curHealth < 1)
            {
                uiScript.gameOver.gameObject.SetActive(true);
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            }
        }
        uiScript.HealthChange(curHealth);
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            rb.velocity = moveAxis.normalized * moveSpeed;
            //rb.MovePosition(rb.position + moveAxis.normalized * moveSpeed * Time.fixedDeltaTime);
        }
    }

    public void Hit(int dam)
    {
        DoDamage(dam);
    }

   private IEnumerator Dash()
    {

        animator.SetBool("isRolling", true);
        head.SetActive(false);
        spriteRend.color = Color.black;
        canDash = false;
        isDashing = true;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("PlayerBullets"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("EnemyBullets"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemies"), true);

        rb.velocity = moveAxis * dashingPower;
        yield return new WaitForSeconds(dashingTime);
        spriteRend.color = Color.white;
        animator.SetBool("isRolling", false);

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("PlayerBullets"), false);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("EnemyBullets"), false);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemies"), false);
        head.SetActive(true);

        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);



        canDash = true;


    }
    private IEnumerator HitVisual()
    {
        for (int x = 0; x < 7; x++)
        {
            spriteRend.color = Color.cyan;
            yield return new WaitForSeconds(0.1f);
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
        canBeDamaged = true;
    }

    private IEnumerator IgnorePlayer()
    {
        yield return new WaitForSeconds(0.1f);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("PlayerBullets"), false);
    }
}