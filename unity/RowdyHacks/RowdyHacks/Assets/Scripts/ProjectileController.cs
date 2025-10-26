using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    protected int damage = 1;
    protected Rigidbody2D rb;
    protected Vector2 direction = new Vector2(0f, 0f);
    protected float speed = 0f;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject projContainer = GameObject.Find("ProjectileContainer");

        if (projContainer != null) transform.parent = projContainer.transform;

        Destroy(gameObject, 5f);
    }

    public virtual void ShootProjectile()
    {
        if (rb != null) rb.linearVelocity = direction * speed;
    }

    public void SetDirection(Vector2 direction)
    {
        this.direction = direction;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().Hit(damage);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
