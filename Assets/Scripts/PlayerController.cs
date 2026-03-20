using System;
using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public float fastFallGravity = 4f;
    public float deaccelrationSpeedX = 0.1f;
    public float deaccelrationSpeedY = 0.1f;
    public float maxYSpeed = 0.1f;
    public Vector2 gravityscale;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastMoveInput;
    private bool isFastFalling;
    private float defaultGravity;
    private PlayerTeleportHandler teleportHandler;
    public bool IsInputBlocked { get; private set; } = false;

    [Header("Continuous Footstep Settings")]
    public AudioSource footstepSource;
    public float fadeSpeed = 5f;
    public float maxVolume = 0.5f;

    [Header("Animations")]
    public Animator animator;

    private Vector2 currentSpeed;
    private Vector2 lastSpeed;

    [SerializeField] private GameObject deathUI;

    void Start()
    {
        if (footstepSource == null) footstepSource = GetComponent<AudioSource>();
        teleportHandler = GetComponent<PlayerTeleportHandler>();

        if (Camera.main != null)
        {
            footstepSource.rolloffMode = AudioRolloffMode.Linear;
            footstepSource.minDistance = 1f;
            footstepSource.maxDistance = 20f;
        }

        lastMoveInput = new Vector2(0, 0);
        lastSpeed = currentSpeed;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;

    }

    void OnMove(InputValue value)
    {
        if (!IsOwner || IsInputBlocked) return;
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (!IsOwner || IsInputBlocked) return;
        if (value.isPressed && Mathf.Abs(rb.linearVelocity.y) < 0.1f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void OnFastfall(InputValue value)
    {
        return;
        if (!IsOwner || IsInputBlocked) return;
        isFastFalling = value.isPressed;
    }



    void FixedUpdate()
    {
        if (!IsOwner || IsInputBlocked) return;
        HandleContinuousFootsteps();
        HandlePlayerAirAnimations();

        //rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        CalculateSpeed();


        if (isFastFalling && rb.linearVelocity.y < 0)
        {
            rb.gravityScale = fastFallGravity;
        }
        else
        {
            rb.gravityScale = defaultGravity;
        }
        lastMoveInput = moveInput;
    }

    public void CalculateSpeed()
    {
        Vector2 movementDirection = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        Vector2 magnetismDirection = GetComponent<Magnetic>().totalDirection;
        Vector2 totalSpeed = movementDirection + magnetismDirection + gravityscale;
        currentSpeed = totalSpeed;
        if (IsDeaccelerating(totalSpeed))
        {
            float n = 0;
            if(lastSpeed.x > 0) { 
                n = lastSpeed.x - deaccelrationSpeedX; }
            if(lastSpeed.x < 0) { 
                n = lastSpeed.x + deaccelrationSpeedX; }
            float x = currentSpeed.x - n;
            currentSpeed = new Vector2(x,0) + totalSpeed;
        }

        rb.linearVelocity = currentSpeed;
        lastSpeed = currentSpeed;
    }
    
    public bool IsDeaccelerating(Vector2 speed)
    {
        if(Mathf.Abs(lastSpeed.x) > Mathf.Abs(speed.x) && Mathf.Abs(lastSpeed.x) > Mathf.Abs(moveSpeed))
        {
            if((lastSpeed.x > 0 && speed.x > 0) || (lastSpeed.x < 0 && speed.x < 0))
            return true;
        }
        return false;
    }
    /*  public void CalculateSpeed()
      {
          if (lastMoveInput != moveInput)
          {
              if (lastMoveInput.x == 0)
              {
                  rb.AddForce(moveInput * moveSpeed, ForceMode2D.Impulse);
              }
              else
              {
                  if (rb.linearVelocity.x > 0.2f || rb.linearVelocity.x < -0.2f)
                  {
                      rb.AddForce((lastMoveInput * moveSpeed)*-1, ForceMode2D.Impulse);
                  }
              }
          }
      } */

    /* public void CalculateSpeed()
     {
         if(rb.linearVelocity.x > moveSpeed*-1 && rb.linearVelocity.x < moveSpeed)
         {
             rb.AddForce(moveInput * moveSpeed, ForceMode.VelocityChange);
         }
     }*/
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
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsRunning", isMoving);

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

    public void DieAndRespawn()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        StartCoroutine(DeathSequenceRoutine());
    }

    private IEnumerator DeathSequenceRoutine()
    {
        IsInputBlocked = true;
        GameObject spawnedUI = null;

        // 1. Find and Load the prefab from the 'Resources' folder
        // The name "DeathUI" must match your prefab's filename exactly
        GameObject deathUIPrefab = Resources.Load<GameObject>("DeathUI");

        if (deathUIPrefab != null)
        {
            spawnedUI = Instantiate(deathUIPrefab);
        }
        else
        {
            Debug.LogError("Could not find 'DeathUI' prefab in the Resources folder!");
        }

        // 2. Kill momentum
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // 3. The 2-second wait
        yield return new WaitForSeconds(2f);

        // 4. Remove the UI
        if (spawnedUI != null)
        {
            Destroy(spawnedUI);
        }

        // 5. Teleport Logic
        SpawnPointManager spawnManager = UnityEngine.Object.FindAnyObjectByType<SpawnPointManager>();
        if (spawnManager != null)
        {
            var bestPoint = spawnManager.Spawnpoints
                .Where(s => s.unlocked)
                .OrderByDescending(s => s.level)
                .FirstOrDefault();

            if (bestPoint != null && teleportHandler != null)
            {
                teleportHandler.PerformTeleport(bestPoint);
            }
        }

        IsInputBlocked = false;
    }

    private void TeleportToPosition(Vector3 targetPosition)
    {
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        transform.position = targetPosition;

        if (cc != null) cc.enabled = true;
    }

    private void HandlePlayerAirAnimations()
    {
        bool goingUp = rb.linearVelocityY > 0;

        animator.SetBool("GoingUp", goingUp);
    }
}