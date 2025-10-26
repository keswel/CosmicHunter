using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    public int health = 3;
    private float moveSpeed = 5f;
    private Vector2 movement;
    private bool invincible = false;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
    }

    void Update()
    {
        // Get movement input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Animator control
        animator.SetFloat("MoveX", movement.x);
        animator.SetFloat("MoveY", movement.y);

        animator.SetBool("IsMoving", movement.sqrMagnitude > 0.01f);
    }

    void FixedUpdate()
    {
        movement.Normalize();
        rb.linearVelocity = movement * moveSpeed;
    }

    public void Hit(int damage)
    {
        if (!invincible)
        {
            health -= damage;

            StartCoroutine(DamageFeedback());
        }
    }

    private IEnumerator DamageFeedback()
    {
        CameraController.Instance.Shake(.1f, .1f);

        invincible = true;

        Color originalColor = Color.white;

        // Flash red
        spriteRenderer.color = Color.red;

        // Punch scale
        Vector3 originalScale = new Vector3(.5f, .5f, 1f);
        transform.localScale = originalScale * 1.1f;

        yield return new WaitForSeconds(0.15f);

        transform.localScale = originalScale * 1.0f;

        yield return new WaitForSeconds(0.15f);

        spriteRenderer.color = originalColor;
        transform.localScale = originalScale;

        invincible = false;
    }
}
