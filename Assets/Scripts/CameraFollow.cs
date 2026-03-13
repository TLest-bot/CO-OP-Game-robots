using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance;

    private Transform target;

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

    void LateUpdate()
    {
        if (target == null) return;

        // Simply calculate where the camera should be
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move from current position to desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    }
}