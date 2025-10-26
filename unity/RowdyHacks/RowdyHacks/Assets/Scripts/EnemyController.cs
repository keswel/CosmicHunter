using System.Collections;
using System.Dynamic;
using UnityEngine;
using UnityEngine.U2D;

public class EnemyController : MonoBehaviour
{
    public int health;
    public int damage;
    public float speed;

    public GameObject playerObj;

    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rb;

    public AudioClip bob_death; // Assign in Inspector
    private AudioSource audioSource;

    protected void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        Move();

    }

    public virtual void Move()
    {
        if (playerObj == null) return;

        // calculate direction to player & move towards player
        Vector2 direction = (playerObj.transform.position - transform.position).normalized;
        //transform.position = Vector2.MoveTowards(transform.position, playerObj.transform.position, speed * Time.deltaTime);
        rb.linearVelocity = direction * speed;
    }

    public void Hit(int damage)
    {
        health -= damage;

        StartCoroutine(DamageFeedback());
    }

    public virtual void Die()
    {
        Destroy(gameObject);
        audioSource.PlayOneShot(bob_death);

    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().Hit(damage);
        }
    }

    protected IEnumerator DamageFeedback()
    {
        Color originalColor = Color.white;

        // Flash red
        spriteRenderer.color = Color.red;

        // Punch scale
        Vector3 originalScale = new Vector3(1f, 1f, 1f);
        transform.localScale = originalScale * 1.2f;

        yield return new WaitForSeconds(0.1f);

        transform.localScale = originalScale * 1.1f;

        yield return new WaitForSeconds(0.1f);

        spriteRenderer.color = originalColor;
        transform.localScale = originalScale;

        if (health <= 0) Die();
    }
}