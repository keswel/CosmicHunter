using System.Collections;
using UnityEngine;

public class FlyingEnemyController : EnemyController
{
    public GameObject projectile;

    public float orbitRadius = 4f;       // desired distance from player
    private float orbitSpeed = 2f;        // speed along the orbit
    private float radialSpeed = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    new void Start()
    {
        base.Start();
        StartCoroutine(ProjectileCooldown());
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    protected void ShootProjectile(Vector3 offset, Vector2 direction, float speed)
    {
        GameObject proj = Instantiate(projectile, transform.position + offset, Quaternion.identity);
        ProjectileController pController = proj.GetComponent<ProjectileController>();
        
        pController.SetDirection(direction);
        pController.SetSpeed(speed);

        pController.ShootProjectile();
    }

    public override void Move()
    {
        if (playerObj == null) return;

        Vector2 toPlayer = playerObj.transform.position - transform.position;
        float distance = toPlayer.magnitude;

        // Tangential direction for orbit
        Vector2 tangent = new Vector2(toPlayer.y, -toPlayer.x).normalized; // counter-clockwise
                                                                           // Vector2 tangent = new Vector2(-toPlayer.y, toPlayer.x).normalized; // clockwise
        // Radial correction
        Vector2 radial = Vector2.zero;
        float distanceError = distance - orbitRadius;

        radial = toPlayer.normalized * distanceError * radialSpeed;

        // Combine tangential orbit + radial correction
        Vector2 velocity = tangent * orbitSpeed + radial;

        rb.linearVelocity = velocity;
    }

    public override void Die()
    {
        StopCoroutine(ProjectileCooldown());

        StartCoroutine(ExplodeBot());
    }

    private IEnumerator ProjectileCooldown()
    {
        while (true)
        {
            Vector2 direction = (playerObj.transform.position - transform.position).normalized;

            ShootProjectile(-Vector3.up/4,direction, 2f);
            Debug.Log("trying to shoot projectile");
            yield return new WaitForSeconds(1f);
        }
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

        for (int i = 0; i < 4; i++)
        {
            spriteRenderer.color = Color.yellow;
            transform.localScale = originalScale * 1.2f;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
            transform.localScale = originalScale * 1.1f;
            yield return new WaitForSeconds(0.05f);
        }

        transform.localScale = new Vector3(1f, 1f, 1f);
        animator.Play("FlyingRobotDeath");

        CameraController.Instance.Shake(.15f, .08f);

        ShootProjectile(-Vector3.up / 4, transform.up, 2f);
        ShootProjectile(-Vector3.up / 4, transform.right, 2f);
        ShootProjectile(-Vector3.up / 4, -transform.up, 2f);
        ShootProjectile(-Vector3.up / 4, -transform.right, 2f);

        Destroy(gameObject, .3f);
    }
}
