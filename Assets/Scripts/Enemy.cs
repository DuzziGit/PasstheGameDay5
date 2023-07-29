using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour,IHittable
{
    private Rigidbody2D rb;

    private Transform player;

    private Player realPlayer;

    [SerializeField] private SpriteRenderer spriteRend;
    [SerializeField]
    private bool active = false;


    [SerializeField]
    private float maxHealth;
    private float health;

	[SerializeField]
	private int meleeDamage;

	[SerializeField]
    private float speed;
    [SerializeField]
    private float stoppingDistance;
    [SerializeField]
    private float retreatDistance;

    [Header("shooting")]
    [SerializeField]
    private GameObject projectileObject;
    [SerializeField]
    private float startTimeBtwShots;
    private float timeBtwShots;

    


    public void Activate(Transform player)
    {
        this.player = player;
        active = true;
    }
    public void DoDamage(float damage)
    {
        if(active)
             health -= damage;
    }

    public void Hit(int dam)
    {
        StartCoroutine(HitVisual());
        DoDamage(dam);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;



        spriteRend = GetComponentInChildren<SpriteRenderer>();
        health = maxHealth;
    }

    private void Start()
    {
        timeBtwShots = startTimeBtwShots;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!active)
            return;

		FireBullet();

		if (health <= 0)
            Destroy(gameObject);

		
		Movement();
        UpdateAnimator();
        

    }

    private void Movement()
    {
        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);

        if (distanceFromPlayer > stoppingDistance) { 
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
       // else if (distanceFromPlayer < retreatDistance)
       // {
         //   transform.position = Vector2.MoveTowards(transform.position, realPlayer.position, -speed * Time.deltaTime);

        }
    }

    private void FireBullet()
    {
        if (timeBtwShots <= 0)
        {
            Instantiate(projectileObject, transform.position, Quaternion.identity);
            
            timeBtwShots = startTimeBtwShots;
        }

        else
        {
            timeBtwShots -= Time.deltaTime;
        }
    }


[Header("Effects")]
    [SerializeField] private float aniMoveSpeed, yAmp = 0.1f, yFrq = 24f;
    void UpdateAnimator()
    {
        var direction = Mathf.Sign(player.transform.position.x-transform.position.x);
        spriteRend.transform.localScale = new Vector3(direction, 1f, 1f);

        Vector2 latSpeed = rb.velocity;
        aniMoveSpeed = Vector3.SqrMagnitude(latSpeed);
        if (aniMoveSpeed > 0)
        {
            float yPos = Mathf.Sin(Time.time * yFrq) * yAmp;
            spriteRend.gameObject.transform.localPosition = new Vector3(0, yPos, 0);//bounce sprite
        }
        else
            spriteRend.gameObject.transform.localPosition = Vector3.zero;
    }

	private IEnumerator HitVisual()
	{

		rb.isKinematic = true;
		spriteRend.color = Color.red;
		yield return new WaitForSeconds(0.2f);
        spriteRend.color = Color.white;
		rb.isKinematic = false;

	}


	void OnTriggerStay2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
            GameObject.Find("Player(Clone)").GetComponent<Player>().DoDamage(meleeDamage);
		}
	}
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			GameObject.Find("Player(Clone)").GetComponent<Player>().DoDamage(meleeDamage);
		}
	}
}
