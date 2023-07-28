using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private int damage;
    private int lifeTime = 3;

    private Vector2 target,dir;

    private bool canHitSelf;
    private GameObject owner;
    private Rigidbody2D rb;
    private Vector3 lastVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("PlayerBullets"), true);
    }
    // Start is called before the first frame update
    void Start()
    {
        target = Vector2.up;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = Vector2.MoveTowards(transform.position, target + (Vector2)transform.position, speed * Time.deltaTime);
        lastVelocity = rb.velocity;


        if (lifeTime<=0)
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
        if (!canHitSelf && other.gameObject == owner)
            return;

        IHittable hit = other.GetComponent<IHittable>();
        if (hit != null)
        {
            hit.Hit(damage);
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
    private enum Spell { Wound, Skewer, Guardian, Frog, Freeze, Explosion }
    private Spell spellEffect;
    private void SpellEffect()
    {
        switch (spellEffect)
        {
            case Spell.Wound:
                break;
            case Spell.Skewer:
                break;
            case Spell.Guardian:
                break;
            case Spell.Frog:
                break;
            case Spell.Freeze:
                break;
            case Spell.Explosion:
                break;
            default:
                break;
        }
    }
}
