using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BossController : FlyingEnemyController
{
    private float verticalOffset = 3f; // stays above player

    private float horizontalSpeed = 1f;
    private float leftBound = -6f;
    private float rightBound = 6f;

    private float floatAmplitude = 0.3f;
    private float floatFrequency = 1.5f;

    private Vector2 targetPos;
    private bool movingRight = true;

    private float maxAttackDelay = 3f;   // starting attack interval
    private float minAttackDelay = 1.5f;   // speed limit
    private float attackAccel = 0.05f;   // how fast it gets quicker
    private float currentAttackDelay;

    private GameObject alienContainer;

    private Animator anim;

    [SerializeField] GameObject flyingEnemy;
    [SerializeField] GameObject explodingEnemy;
    [SerializeField] GameObject homingProjectile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    new void Start()
    {
        base.Start();

        currentAttackDelay = maxAttackDelay;
        anim = GetComponent<Animator>();
        alienContainer = GameObject.Find("AlienContainer");

        Camera.main.orthographicSize = 7f;
        CameraController.Instance.SetCameraBounds();
        CameraController.Instance.SetOffset(new Vector3(0f, 3f, -10f));

        StartCoroutine(BossAttack());
    }

    void Update()
    {
        Move();
    }

    private new void Move()
    {
        if (playerObj == null) return;

        // Dynamic horizontal patrol bounds around the player
        float playerX = playerObj.transform.position.x;
        float x = Mathf.Lerp(transform.position.x, playerX, Time.deltaTime * 0.5f);

        if (movingRight)
        {
            x += horizontalSpeed * Time.deltaTime;
            if (x >= playerX + rightBound) movingRight = false;
        }
        else
        {
            x -= horizontalSpeed * Time.deltaTime;
            if (x <= playerX + leftBound) movingRight = true;
        }

        // Always stay above player (dynamic vertical tracking)
        float y = playerObj.transform.position.y + verticalOffset;

        // Floating effect on top of vertical tracking
        float floatOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        y += floatOffset;

        transform.position = new Vector3(x, y, transform.position.z);
    }

    private void ShootHomingProjectile(Transform target, Vector3 offset, float speed)
    {
        GameObject proj = Instantiate(homingProjectile, transform.position + offset, Quaternion.identity);
        HomingProjectile pController = proj.GetComponent<HomingProjectile>();

        pController.SetSpeed(speed);
        pController.SetTarget(target);

        pController.ShootProjectile();
    }

    private void ShootRow()
    {
        ShootProjectile(-(Vector3.up)/3, new Vector2(-1.125f, -.4f), 4f);
        ShootProjectile(- (Vector3.up)/ 3, new Vector2(-.75f, -.4f), 4f);
        ShootProjectile(-(Vector3.up) / 3, new Vector2(-.375f, -.4f), 4f);
        ShootProjectile(-(Vector3.up) / 3, new Vector2(0f, -.4f), 4f);
        ShootProjectile(-(Vector3.up) / 3, new Vector2(.375f, -.4f), 4f);
        ShootProjectile(-(Vector3.up) / 3, new Vector2(.75f, -.4f), 4f);
        ShootProjectile(-(Vector3.up) / 3, new Vector2(1.125f, -.4f), 4f);
    }

    private void ShootTargeted()
    {
        Vector2 direction = (playerObj.transform.position - transform.position).normalized;

        ShootProjectile(new Vector3(-.75f, -.4f, 0f), direction, 4f);
        ShootProjectile(new Vector3(-.375f, -.4f, 0f), direction, 4f);
        ShootProjectile(new Vector3(0f, -.4f, 0f), direction, 4f);
        ShootProjectile(new Vector3(.375f, -.4f, 0f), direction, 4f);
        ShootProjectile(new Vector3(.75f, -.4f, 0f), direction, 4f);
    }

    private void ShootHoming()
    {
        ShootHomingProjectile(playerObj.transform, new Vector3(-1.125f, -.4f, 0f), 3f);
        ShootHomingProjectile(playerObj.transform, new Vector3(0f, -.4f, 0f), 3f);
        ShootHomingProjectile(playerObj.transform, new Vector3(1.125f, -.4f, 0f), 3f);
    }

    private void SummonFlying()
    {
        for (int i = -1; i <= 1; i++)
        {
            GameObject newAlien = Instantiate(flyingEnemy, transform.position - new Vector3(.5f * i, -.5f, 0f), Quaternion.identity);
            if (alienContainer != null) newAlien.transform.parent = alienContainer.transform;

            newAlien.GetComponent<EnemyController>().playerObj = playerObj;
            newAlien.GetComponent<FlyingEnemyController>().orbitRadius = 2f;
        }

    }

    private void SummonExplosive()
    {
        for (int i = -1; i <= 1; i++)
        {
            GameObject newAlien = Instantiate(explodingEnemy, transform.position - new Vector3(.5f * i, -.5f, 0f), Quaternion.identity);
            if (alienContainer != null) newAlien.transform.parent = alienContainer.transform;

            newAlien.GetComponent<EnemyController>().playerObj = playerObj;
            newAlien.GetComponent<ExplosiveEnemyController>().explosionDamage = 2;
        }
    }

    private IEnumerator BossAttack()
    {
        Vector2 direction = (playerObj.transform.position - transform.position).normalized;
        while (true)
        {
            yield return new WaitForSeconds(3f);
            int randomInt = Random.Range(1, 6);

            switch (randomInt)
            {
                case 1:
                    ShootRow();
                    break;
                case 2:
                    ShootTargeted();
                    break;
                case 3:
                    ShootHoming();
                    break;
                case 4:
                    SummonFlying();
                    break;
                case 5:
                    SummonExplosive();
                    break;
            }

            anim.Play("BossAttack");

            currentAttackDelay = Mathf.Max(currentAttackDelay - attackAccel, minAttackDelay);
        }
    }
}
