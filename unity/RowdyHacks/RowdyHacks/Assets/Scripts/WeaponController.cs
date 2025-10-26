using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public int ammoInCylinder = 6;
    public float reloadDuration = 1.0f;

    public float fireRate = 0.2f;
    public float maxHitDistance = 100f;
    public Transform muzzle;
    public LayerMask hitLayers; // layers the bullet can impact

    public GameObject dustEffectPrefab; // particle effect prefab
    public GameObject hitEffectPrefab; // particle effect prefab

    public AudioClip shotClip;
    public AudioClip emptyClip;
    public AudioClip reloadClip;
    public AudioSource audioSource;

    private bool isReloading = false;
    private float lastFireTime = -999f;

    //Crosshair Controller Variables
    public GameObject crosshair;

    private float followZDepth;
    private float rotationSpeed = 100f;
    private float enlargeMultiplier = 1.2f;
    private float enlargeDuration = 0.1f;
    private Vector3 initialScale;
    private float enlargeTimer = 0f;

    //Gun Sprite Controller Variables
    public GameObject gunSprite;

    private float distanceFromPivot = 1.25f;

    private void Start()
    {
        initialScale = crosshair.transform.localScale;
        followZDepth = Camera.main.transform.localPosition.z * -1;
        Cursor.visible = false;
    }

    void Update()
    {
        FollowMouse();
        RotateCrosshair();

        if (Input.GetKeyDown(KeyCode.R))
            TryStartReload();

        if (Input.GetMouseButtonDown(0))
            TryFire();
    }

    private void FixedUpdate()
    {
        if (enlargeTimer > 0)
        {
            enlargeTimer -= Time.deltaTime;
            if (enlargeTimer <= 0)
            {
                crosshair.transform.localScale = initialScale;
            }
        }
    }

    private void FollowMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = followZDepth;

        //Manage Crosshair Position
        crosshair.transform.position = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldMousePosition.z = 0f;

        //Manage Gun Sprite
        Vector3 dir = worldMousePosition - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Flip gun sprite if needed
        if (angle > 90 || angle < -90)
        {
            gunSprite.GetComponent<SpriteRenderer>().flipY = true;
        }
        else
        {
            gunSprite.GetComponent<SpriteRenderer>().flipY = false;
        }

        gunSprite.transform.position = transform.position + dir.normalized * distanceFromPivot;
        gunSprite.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void RotateCrosshair()
    {
        crosshair.transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    private void TryFire()
    {
        if (isReloading) return;
        if (Time.time - lastFireTime < fireRate) return;
        lastFireTime = Time.time;

        if (ammoInCylinder > 0)
            FireShot();
        else if (audioSource && emptyClip)
            audioSource.PlayOneShot(emptyClip);

        if (ammoInCylinder == 0)
        {
            TryStartReload();
        }
    }

    private void FireShot()
    {
        ammoInCylinder--;

        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;

        Vector2 direction = (worldPos - transform.position).normalized;

        // Raycast from muzzle forward
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxHitDistance, hitLayers);
        if (hit.collider != null)
        {
            //ON HIT ENEMY EFFECTS HERE
            if (hit.transform.CompareTag("Enemy"))
            {
                hit.transform.GetComponent<EnemyController>().Hit(1);
            }

            // Impact FX
            //if (hitEffectPrefab != null)
            //{
            //    GameObject fx = Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            //    Destroy(fx, .5f); // cleanup
            //}
        }
        else if (dustEffectPrefab != null)
        {
            mousePos.z = 10f;

            GameObject fx = Instantiate(dustEffectPrefab, worldPos, Quaternion.LookRotation(hit.normal));
            Destroy(fx, .3f); // cleanup
        }

        //VISUAL/AUDIO EFFECTS
        //Modifies Crosshair Size when firing
        crosshair.transform.localScale = initialScale * enlargeMultiplier;
        enlargeTimer = enlargeDuration;

        CameraController.Instance.Shake(.075f, .05f);

        // Play fire sound
        if (audioSource && shotClip)
            audioSource.PlayOneShot(shotClip);

        gunSprite.GetComponent<Animator>().Play("GunSpriteShoot");
    }

    private void TryStartReload()
    {
        if (isReloading) return;
        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        if (audioSource && reloadClip) audioSource.PlayOneShot(reloadClip);
        yield return new WaitForSeconds(reloadDuration);

        ammoInCylinder = 6;

        isReloading = false;
    }
}
