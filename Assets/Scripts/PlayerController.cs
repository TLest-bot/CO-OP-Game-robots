using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public float fastFallGravity = 4f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isFastFalling;
    private float defaultGravity;

    [Header("Continuous Footstep Settings")]
    public AudioSource footstepSource;
    public float fadeSpeed = 5f;
    public float maxVolume = 0.5f;

    void Start()
    {
        if (footstepSource == null) footstepSource = GetComponent<AudioSource>();


        if (Camera.main != null)
        {
            footstepSource.rolloffMode = AudioRolloffMode.Linear;
            footstepSource.minDistance = 1f;
            footstepSource.maxDistance = 20f;
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;

    }

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

    void OnFastfall(InputValue value)
    {
        return;
        if (!IsOwner) return;
        isFastFalling = value.isPressed;
    }

    void Update()
    {
        if (!IsOwner) return;
        HandleContinuousFootsteps();
        Vector2 moveDirection = new Vector2(moveInput.x, 0);

        if (Mathf.Abs(rb.linearVelocity.x) < moveSpeed)
        {
            float accelerationForce = 50f;
            rb.AddForce(moveDirection * accelerationForce);
        }
    }


    void FixedUpdate()
    {
        if (!IsOwner) return;

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

    void HandleContinuousFootsteps()
    {
        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        bool isGrounded = Mathf.Abs(rb.linearVelocity.y) < 0.01f;

        if (isMoving && isGrounded)
        {
            if (!footstepSource.isPlaying) footstepSource.Play();
            footstepSource.volume = Mathf.MoveTowards(footstepSource.volume, maxVolume, fadeSpeed * Time.deltaTime);
        }
        else
        {
            footstepSource.volume = Mathf.MoveTowards(footstepSource.volume, 0f, fadeSpeed * Time.deltaTime);
            if (footstepSource.volume <= 0f && footstepSource.isPlaying) footstepSource.Stop();
        }
    }

}