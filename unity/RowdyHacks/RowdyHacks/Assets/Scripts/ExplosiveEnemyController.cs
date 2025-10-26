using System.Collections;
using UnityEngine;

public class ExplosiveEnemyController : EnemyController
{
    public LayerMask hitLayer;
    public LayerMask playerLayer;

    public int explosionDamage = 2;
    private bool isExploding = false;
    private float detectionRadius = 1.5f;
    private float radius = 2f;

    public AudioClip explosionSound; // Assign in Inspector
    public AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    new void Start()
    {
        base.Start();
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isExploding) return;

        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        if (hit != null)
        {
            Explode();
            isExploding = true;
        }

        Move();
    }

    public override void Die()
    {
        Explode();
    }

    void Explode()
    {
        StartCoroutine(ExplodeBot());
    }

    private IEnumerator ExplodeBot()
    {
        speed = 0;
        gameObject.layer = 0;

        //Remove hitbox
        Destroy(gameObject.GetComponent<BoxCollider2D>());


        Color originalColor = Color.white;

        // Punch scale
        Vector3 originalScale = new Vector3(1f, 1f, 1f);

        for (int i= 0; i < 10; i++)
        {
            spriteRenderer.color = Color.yellow;
            transform.localScale = originalScale * 1.2f;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
            transform.localScale = originalScale * 1.1f;
            yield return new WaitForSeconds(0.05f);
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, hitLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy") && hit.transform != transform)
            {
                hit.GetComponent<EnemyController>().Hit(explosionDamage);
            }
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<PlayerController>().Hit(explosionDamage);
            }
        }

        transform.localScale = new Vector3(2f, 2f, 2f);
        animator.Play("ExplodingRobotDeath");

        audioSource.PlayOneShot(explosionSound);
        CameraController.Instance.Shake(.2f, .15f);

        Destroy(gameObject, .3f);
    }
}
