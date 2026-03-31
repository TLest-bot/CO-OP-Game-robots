using System;
using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public float fastFallGravity = 4f;
    public float deaccelrationSpeedX = 0.1f;
    public float deaccelrationSpeedY = 0.1f;
    public float maxYSpeed = 0.1f;
    public Vector2 gravityscale;
    public float coyoteTime = 0.2f;
    public float rotateSpeed = 1f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastMoveInput;
    private bool isFastFalling;
    private float defaultGravity;
    private PlayerTeleportHandler teleportHandler;
    private float coyoteTimeTimer = 0f;
    public bool IsInputBlocked { get; private set; } = false;
    private SpriteRenderer playerSprite;
    public GameObject tinyExplosionPrefab;

    [Header("Continuous Footstep Settings")]
    public AudioSource footstepSource;
    public float fadeSpeed = 5f;
    public float maxVolume = 0.5f;
    public AudioSource Jumpsound;

    [Header("Animations")]
    public Animator animator;

    private Vector2 currentSpeed;
    private Vector2 lastSpeed;
    private bool lastGroundState = true;
    private bool usedCoyoteJump = false;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D myCollider;
    private bool isFacingLeft = true;
    private Quaternion originalRotation;
    private bool IsOwner = true;
    public bool isPlayer1;
    [SerializeField] private GameObject deathUI;

    void Start()
    {
        if (footstepSource == null) footstepSource = GetComponent<AudioSource>();
        teleportHandler = GetComponent<PlayerTeleportHandler>();
        playerSprite = GetComponentInChildren<SpriteRenderer>();
        myCollider = GetComponentInChildren<BoxCollider2D>();

        if (Camera.main != null)
        {
            footstepSource.rolloffMode = AudioRolloffMode.Linear;
            footstepSource.minDistance = 1f;
            footstepSource.maxDistance = 20f;
        }

        lastMoveInput = new Vector2(0, 0);
        lastSpeed = currentSpeed;

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalRotation = transform.rotation;

        if(GetComponent<PlayerReversePolarity>() == null)
        {
            CameraFollow.Instance.SetTarget2(this.transform);
            isPlayer1 = false;
        }
        else
        {
            CameraFollow.Instance.SetTarget(this.transform);
            isPlayer1 = true;
        }
        animator.SetBool("Player1", isPlayer1);
        if(GetComponent<PlayerReversePolarity>() != null)
        {
            GetComponent<PlayerReversePolarity>().isPlayer1 = isPlayer1;
        }


        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;

        foreach (GameObject platform in GameObject.FindGameObjectsWithTag("Platform"))
        {
            platform.GetComponent<Platform>().players.Add(gameObject);
        }

        Scene currentScene = SceneManager.GetActiveScene();
        Activate(currentScene.name != "Level 0");
    }


    void OnMove(InputValue value)
    {
        if (!IsOwner || IsInputBlocked) return;
        moveInput = value.Get<Vector2>();
        if(moveInput.x > 0.5) { moveInput.x = 1;  }
        else if (moveInput.x < -0.5) { moveInput.x = -1; }
        else { moveInput.x = 0; }

        if (moveInput.y > 0.5) { moveInput.y = 1; }
        else if (moveInput.y < -0.5) { moveInput.y = -1; }
        else { moveInput.y = 0; }

        if(moveInput.x == -1) { isFacingLeft = true; }
        if(moveInput.x == 1) { isFacingLeft = false; }

        spriteRenderer.flipX = isFacingLeft;
    }
    
    void OnJump(InputValue value)
    {
        if (!IsOwner || IsInputBlocked) return;
        Jumpsound.Play();
        if (value.isPressed && (IsGrounded() || CanCoyoteTime()))
        {
            usedCoyoteJump = true;
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

            if (lastSpeed.x > 0)
            {
                lastSpeed.x -= deaccelrationSpeedX;
            }

            if (lastSpeed.x < 0)
            {
                lastSpeed.x += deaccelrationSpeedX;
            }
            rb.linearVelocity = lastSpeed;
            lastSpeed = currentSpeed;
            return;
        }

        rb.linearVelocity = currentSpeed;
        lastSpeed = currentSpeed;
    }
    
    public bool IsDeaccelerating(Vector2 speed)
    {
        //Debug.Log((Mathf.Abs(lastSpeed.x) > Mathf.Abs(speed.x)) + " + " + (Mathf.Abs(lastSpeed.x) > Mathf.Abs(moveSpeed)));
        if (Mathf.Abs(lastSpeed.x) > Mathf.Abs(speed.x) && Mathf.Abs(lastSpeed.x) > Mathf.Abs(moveSpeed))
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
    /*public override void OnNetworkSpawn()
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
    }*/
    private void Update()
    {
        CoyoteJump();
        if(IsGrounded() && moveInput.x != 0)
        {
            RotatePlayerBackUp();
        }
    }

    void HandleContinuousFootsteps()
    {
        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        bool isGrounded = IsGrounded();
        animator.SetBool("IsGrounded", IsGrounded());
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

    public bool IsGrounded()
    {
        int mask = LayerMask.GetMask("Walls");
        bool grounded = Physics2D.Raycast(transform.position, Vector2.down, 0.33f, mask);

        int playerMask = LayerMask.GetMask("Player");
        Vector2 playerpos = transform.position;
        bool onPlayer = Physics2D.Raycast(playerpos - new Vector2(0,0.30f), Vector2.down, 0.01f, playerMask);

        if (grounded || onPlayer)
        {
            return true;
        }
        return false;
    }

    public void DieAndRespawn()
    {
        if(!IsInputBlocked)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            StartCoroutine(DeathSequenceRoutine());
        }
    }

    private IEnumerator DeathSequenceRoutine()
    {
        IsInputBlocked = true;
        GameObject spawnedUI = null;

        GameObject deathUIPrefab = Resources.Load<GameObject>("DeathUI");

        if (deathUIPrefab != null)
        {
            spawnedUI = Instantiate(deathUIPrefab);
        }
        else
        {
            Debug.LogError("Could not find 'DeathUI' prefab in the Resources folder!");
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        SetPlayerVisibility(false);
        Instantiate(tinyExplosionPrefab, transform.position, transform.rotation);
        yield return new WaitForSeconds(2f);

        if (spawnedUI != null)
        {
            Destroy(spawnedUI);
        }

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
        SetPlayerVisibility(true);

        IsInputBlocked = false;
    }

    public void SetPlayerVisibility(bool isVisible)
    {
        if (playerSprite != null)
        {
            playerSprite.enabled = isVisible;
            myCollider.enabled = isVisible;
        }
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
    public void ReceiveExplosionForce(Vector2 direction, float force)
    {
        if (IsInputBlocked) return;
        direction.y += 0.5f;
        direction = direction.normalized;
        Vector2 explosionImpulse = direction * force;
        rb.AddForce(explosionImpulse, ForceMode2D.Impulse);

        lastSpeed = rb.linearVelocity;
    }

    public bool CanCoyoteTime()
    {
        if(coyoteTimeTimer > 0 && usedCoyoteJump == false)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CoyoteJump()
    {
        if (coyoteTimeTimer > 0) { coyoteTimeTimer -= Time.deltaTime; }
        if ((IsGrounded() == false && lastGroundState))
        {
            coyoteTimeTimer = coyoteTime;
        }
        else if (IsGrounded() && lastGroundState == false)
        {
            usedCoyoteJump = false;
        }
        lastGroundState = IsGrounded();
    }

    public void RotatePlayerBackUp()
    {
        Quaternion targetAngle = originalRotation;
        Quaternion currentAngle = transform.rotation;
        transform.rotation = Quaternion.Lerp(currentAngle, targetAngle, rotateSpeed);
    }

    public void Activate(bool active)
    {
        animator.SetBool("IsActivated", active);
        GetComponent<Magnetic>().activated = active;
    }
}