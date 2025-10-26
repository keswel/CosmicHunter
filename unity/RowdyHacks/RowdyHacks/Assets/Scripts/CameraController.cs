using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    public bool camLock = false;

    private Vector3 offset = new Vector3(0f, 0f, -10f);
    private float smoothTime = .2f;
    private Vector3 velocity = Vector3.zero;

    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.1f;

    [SerializeField] Transform target;

    public Vector2 min;
    public Vector2 max;

    private Vector2 minBounds;
    private Vector2 maxBounds;

    void Awake()
    {
        Instance = this;

        SetCameraBounds();
    }

    void LateUpdate()
    {
        if (camLock || target == null) return;

        // Desired camera position
        Vector3 targetPos = target.position + offset;

        // Smooth follow
        Vector3 smoothPos = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

        // Clamp to bounds
        smoothPos.x = Mathf.Clamp(smoothPos.x, minBounds.x, maxBounds.x);
        smoothPos.y = Mathf.Clamp(smoothPos.y, minBounds.y, maxBounds.y);

        transform.position = smoothPos;

        // Screenshake
        if (shakeDuration > 0)
        {
            transform.localPosition += (Vector3)(Random.insideUnitCircle * shakeMagnitude);
            shakeDuration -= Time.deltaTime;
        }
    }

    public void Shake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }

    public void SetOffset(Vector3 offset)
    {
        this.offset = offset;
    }

    public void SetCameraBounds()
    {
        float camHalfHeight = Camera.main.orthographicSize;
        float camHalfWidth = camHalfHeight * Camera.main.aspect;

        minBounds = min;
        maxBounds = max;

        minBounds.x += camHalfWidth;
        maxBounds.x -= camHalfWidth;
        minBounds.y += camHalfHeight;
        maxBounds.y -= camHalfHeight;
    }
}
