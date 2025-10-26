using UnityEngine;

public class HomingProjectile : ProjectileController
{
    private Transform target;

    public override void Awake()
    {
        base.Awake();
    }

    void Update()
    {
        if (target == null || rb == null) return;

        Vector2 directionToTarget = ((Vector2)target.position - rb.position).normalized;
        
        rb.linearVelocity = directionToTarget * speed; // move forward
    }

    public override void ShootProjectile()
    {
        // override so it doesn't set velocity once and forget
        rb.linearVelocity = transform.up * speed;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
