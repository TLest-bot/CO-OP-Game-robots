using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance;
    public float standardSize = 6;

    public Transform target;
    public Transform target2;

    [Header("Follow Settings")]
    [Range(0, 1)] public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 2, -10);

    void Awake()
    {
        Instance = this;
    }

    public void SetTarget(Transform playerTransform)
    {
        target = playerTransform;

        // Immediate snap on spawn
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }

    public void SetTarget2(Transform playerTransform)
    {
        target2 = playerTransform;

        // Immediate snap on spawn
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;
        float Z = transform.position.z;

        // Simply calculate where the camera should be
        Vector3 desiredPosition = ((target.position + target2.position) / 2) + offset;
        desiredPosition.z = Z;

        float dist = Vector2.Distance(target.transform.position, target2.transform.position);
        Camera cam = GetComponent<Camera>();
        if (dist > 7.5)
        {
            cam.orthographicSize = standardSize * (dist / 10);
        }
        else
        {
            cam.orthographicSize = standardSize;
        }

        // Smoothly move from current position to desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    }
}