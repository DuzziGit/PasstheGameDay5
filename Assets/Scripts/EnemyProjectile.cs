using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private int damage;
    private int lifeTime = 3;

    private Vector2 target, dir;

    private bool canHitSelf;
    private GameObject owner;
    private Rigidbody2D rb;
    private Vector3 lastVelocity;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemies"), LayerMask.NameToLayer("EnemyBullets"), true);
    }

    void Start()
    {
        target = Vector2.up;
		Transform player = GameObject.FindGameObjectWithTag("Player").transform;
	}

    // Update is called once per frame
    void Update()
    {
        //transform.position = Vector2.MoveTowards(transform.position, target + (Vector2)transform.position, speed * Time.deltaTime);
        lastVelocity = rb.velocity;


        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = (dir * Mathf.Max(speed, 0f) /** Time.fixedDeltaTime*/);
    }
    public void ChangeTarget(Vector2 target, GameObject self)
    {
        this.target = new Vector2(target.x - transform.position.x, target.y - transform.position.y);
        this.owner = self;
    }
    public void ChangeDirection(Vector2 newDir)
    {
        dir = newDir;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<Player>().DoDamage(damage);
            Destroy(gameObject);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject != owner)
        {
            Vector2 wallNormal = collision.contacts[0].normal;
            ChangeDirection(Vector3.Reflect(lastVelocity.normalized * 2f, wallNormal));
            Debug.Log(dir);
            lifeTime -= 1;
        }
    }
}
