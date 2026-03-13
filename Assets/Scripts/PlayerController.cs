using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public float fastFallGravity = 4f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isFastFalling;
    private float defaultGravity;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;
    }

    // This now matches the "Vector 2" type in your screenshot
    void OnMove(InputValue value)
    {
        if (!IsOwner) return;
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (!IsOwner) return;
        if (value.isPressed && Mathf.Abs(rb.linearVelocity.y) < 0.1f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void OnFastfall(InputValue value) // Note the lowercase 'f' to match your screenshot
    {
        if (!IsOwner) return;
        isFastFalling = value.isPressed;
    }

    void Update()
    {
        if (!IsOwner) return;

        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }


    void FixedUpdate()
    {
        if (!IsOwner) return;

        // Use moveInput.x for horizontal movement
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        if (isFastFalling && rb.linearVelocity.y < 0)
        {
            rb.gravityScale = fastFallGravity;
        }
        else
        {
            rb.gravityScale = defaultGravity;
        }
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Check if this specific spawned prefab belongs to the local player
        if (IsOwner)
        {
            if (CameraFollow.Instance != null)
            {
                Debug.Log("Local Player spawned, assigning camera target!");
                CameraFollow.Instance.SetTarget(this.transform);
            }
            else
            {
                Debug.LogError("CameraFollow instance not found in the scene!");
            }
        }
    }
}