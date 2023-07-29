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

	public Animator animator;

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

	[Header("Shooting")]
	[SerializeField] GameObject[] projectileObjects;
	[SerializeField] private Transform weapon, firePoint;
	[SerializeField] private float rotSpeed = 8f;
	[Header("Ammo")]
	public List<int> gunChamber = new List<int> { 2, 2, 2, 2, 2, 5 };
	private int currentShot;

	private bool canBeDamaged = true;


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
	// Update is called once per frame
	void Update()
	{

		if (curHealth <= 0)
			Debug.Log("death");

		RotateWeapon();

		moveAxis = move.ReadValue<Vector2>();

		UpdateAnimator();
	}
	public void Reload(int min, int max)
	{
		SoundManagerScript.PlaySound("reload");
		gunChamber.Clear();
		for (int i = 0; i < 6; i++)
		{
			gunChamber.Add(i);
			uiScript.AmmoUpdate(gunChamber);
			currentShot = 0;
		}
		firePoint.DOPunchRotation(new Vector3(0, 0, 361), .25f);
	}
	/*
	public void EarlyReload(InputAction.CallbackContext context)
	{
		Debug.Log("early reload");
		Reload(1, 6);
	}
	*/


	public void Roll(InputAction.CallbackContext context)
	{
		if (canDash)
		{
			StartCoroutine(Dash());
		}

	}

	private void RotateWeapon()
	{

		//Changed code so you aim with mouse, much smoother gameplay
		lookAxis = Camera.main.ScreenToWorldPoint(Input.mousePosition) - weapon.position;
		float angle = Mathf.Atan2(lookAxis.y, lookAxis.x) * Mathf.Rad2Deg;
		Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
		weapon.rotation = rotation;

		/*lookAxis = new Vector2(Mathf.Sin(moveAxis.x), Mathf.Sin(moveAxis.y));
        float angle = Mathf.Atan2(lookAxis.y, lookAxis.x) * Mathf.Rad2Deg;
        Quaternion newRot = Quaternion.Euler(0, 0, angle - 90f);
        weapon.rotation = Quaternion.Slerp(transform.rotation, newRot, rotSpeed);*/
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
		//	spriteRend.transform.localScale = new Vector3(direction, 1f, 1f);
		firePoint.transform.localScale = new Vector3(direction, 1f, 1f);

	}
	private void Fire(InputAction.CallbackContext context)
	{
		if (currentShot == 6)//reload
		{
			Reload(1, 6);
		}
		else//fire
		{
			Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("PlayerBullets"), true);
			StartCoroutine(IgnorePlayer());
			SoundManagerScript.PlaySound("fire");
			GameObject proj = Instantiate(projectileObjects[gunChamber[0]], firePoint.transform.position, weapon.rotation);
			gunChamber.RemoveAt(0);
			weapon.DOPunchRotation(new Vector3(0, 0, 60f), 0.12f);
			proj.GetComponent<Projectile>().ChangeDirection(lookAxis);
			//Debug.Log(proj.GetComponent<Projectile>());
			uiScript.RotateBarrel(currentShot);
			currentShot++;
		}
	}
	public void DoDamage(int damage)
	{
		if (canBeDamaged)
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
		//uiScript.HealthChange(curHealth);

	}

	private void FixedUpdate()
	{
		if (!isDashing)
		{
			rb.velocity = moveAxis.normalized * moveSpeed /** Time.fixedDeltaTime*/;
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
		yield return new WaitForSeconds(0.7f);
		Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("PlayerBullets"), false);
	}

}
