using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 8f;
    public float currentSpeed;
    public float airMoveSpeed = 10f;
    public float jumpForce = 16f;
    public float dashDistance = 5f;
    public float dashDuration = 0.3f;
    public float dashSpeed = 20f;
    public float dashCooldown = 1f;
    private float dashCooldownTimer;

    [Header("Wall Slide and Jump Settings")]
    public float wallSlidingSpeed = 1f;
    public float wallJumpingPowerX = 8f;
    public float wallJumpingPowerY = 16f;
    public float wallJumpingTime = 0.2f;
    public float wallJumpingDuration = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDirection;
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);

    [Header("Combat Settings")]
    public bool canShoot = true;
    public float fireRate = 0.5f;
    public int maxBullets = 10;
    private int currentBullets;
    public GameObject bulletPrefab;
    public GameObject muzzleFlashPrefab;
    public Transform bulletSpawnPoint;
    public Animator animator; // Animator for reloading animation

    [Header("UI Settings")]
    public TMP_Text bulletUIText; // TMP text for bullet count

    [Header("Crouch Settings")]
    public float sitSpeed = 4f;
    public bool isSitting;

    [Header("Camera Settings")]
    [SerializeField] private GameObject cameraFollow;

    [Header("Coyote Time Settings")]
    public float coyoteTime = 0.2f; // Duration of coyote time
    private float coyoteTimeCounter; // Timer to keep track of coyote time

    [Header("Other Settings")]
    public bool isAlive = true;
    public Sprite cursorSprite;
    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask environmentLayer; // Unified layer for both ground and wall
    public LayerMask enemyLayer;
    public LayerMask projectileLayer;
    public ParticleSystem dust;

    private GameObject cursorObject;
    private bool isWallJumping;
    private bool isDashing;
    private bool isCrouching = false;
    public bool isWallSliding { get; private set; }
    public bool isFacingRight { get; private set; } = true;
    private Rigidbody2D rb;
    private Animator anim;
    private float horizontal;
    private Vector2 knockbackDirection;

    // Reference to PlayerHealth script
    private PlayerHealth playerHealth;
    private SlashEffect slashEffect;
    private CameraFollowObject cameraFollowObject;
    private float fallSpeedYDampingChangeThreshold;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
        cameraFollowObject = cameraFollow.GetComponent<CameraFollowObject>();
        fallSpeedYDampingChangeThreshold = CameraManger.instance.fallSpeedYDampingChangeThreshold;
        slashEffect = FindObjectOfType<SlashEffect>(); // Ensure this finds the SlashEffect component

        if (slashEffect == null)
        {
            Debug.LogError("SlashEffect component not found in the scene!");
        }

        if (PlayerPrefs.HasKey("BulletUIText"))
        {
            bulletUIText.text = PlayerPrefs.GetString("BulletUIText");
        }
        else
        {
            UpdateBulletUI();
        }

        InitializeCursor();
        EnsureCorrectDirection();
        wallJumpingDirection = -1;

        // Load current bullets from PlayerPrefs if available, else set to maxBullets
        currentBullets = PlayerPrefs.HasKey("CurrentBullets") ? PlayerPrefs.GetInt("CurrentBullets") : maxBullets;

        UpdateBulletUI();
    }

    // Update bullet count and UI before the player leaves the scene
    private void OnDisable()
    {
        PlayerPrefs.SetInt("CurrentBullets", currentBullets);
        PlayerPrefs.SetString("BulletUIText", bulletUIText.text);
        PlayerPrefs.Save();
    }

    private void Update()
    {
        if (!isAlive) return; // Prevent any actions if the player is not alive

        horizontal = Input.GetAxisRaw("Horizontal");

        // Coyote time logic
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime; // Reset coyote time when grounded
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime; // Decrease coyote time if not grounded
        }

        // Camera damping logic based on fall speed
        if(rb.velocity.y < fallSpeedYDampingChangeThreshold && !CameraManger.instance.IsLerpingYDamping && !CameraManger.instance.LerpedFrontLayerFalling)
        {
            CameraManger.instance.LerpYDamping(true);
        }

        if(rb.velocity.y >= 0f && !CameraManger.instance.IsLerpingYDamping && CameraManger.instance.LerpedFrontLayerFalling)
        {
            CameraManger.instance.LerpedFrontLayerFalling = false;
            CameraManger.instance.LerpYDamping(false);
        }

        // Update animator based on player movement
        if (IsGrounded())
        {
            if (horizontal != 0)
            {
                anim.SetBool("isRunning", true);
                anim.SetBool("isIdle", false);
            }
            else
            {
                anim.SetBool("isRunning", false);
                anim.SetBool("isIdle", true);
            }
            anim.SetBool("isWallSliding", false);
            anim.SetBool("isJumping", false);
        }
        else
        {
            anim.SetBool("isRunning", false);
            anim.SetBool("isIdle", false);
            anim.SetBool("isJumping", true);
        }

        // Update animator based on crouching state
        if (isSitting)
        {
            anim.SetBool("isSitting", true);
            anim.SetBool("isRunning", false);
            anim.SetBool("isIdle", false);
        }
        else
        {
            anim.SetBool("isSitting", false);
        }

        // Jump logic with coyote time
        if (Input.GetButtonDown("Jump") && (IsGrounded() || coyoteTimeCounter > 0f) && !isCrouching)
        {
            dust.Play();
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            anim.SetBool("isRunning", false);
            anim.SetBool("isIdle", false);
            anim.SetBool("isJumping", true);
            coyoteTimeCounter = 0f; // Reset coyote time after jumping
        }

        // Reduce vertical velocity when jump button is released
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        // Dash logic
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0f)
        {
            StartCoroutine(Dash());
        }

        // Wall slide and jump logic
        WallSlide();
        WallJump();

        // Flip the player's direction based on movement
        if (!isWallJumping && !isDashing)
        {
            Flip();
        }

        // Reduce dash cooldown over time
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        // Shooting logic
        if (canShoot) Shoot();

        // Reload logic
        if (Input.GetKeyDown(KeyCode.R) && currentBullets < maxBullets)
        {
            StartCoroutine(Reload());
        }

        // Update custom cursor position
        UpdateCursor();

        // Crouching logic
        Crouch();
    }

    private void FixedUpdate()
    {
        if (!isAlive) return;

        if (!isWallJumping && !isDashing)
        {
            float moveSpeed = isSitting ? sitSpeed : speed;

            if (IsGrounded())
            {
                rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
                anim.SetBool("isJumping", false); // Stop jumping animation when grounded
                anim.SetBool("WallJumping", false); // Stop wall jumping animation when grounded
            }
            else if (!IsGrounded() && (!isWallSliding || !IsWalled()) && horizontal != 0)
            {
                rb.AddForce(new Vector2(airMoveSpeed * horizontal, 0));
                if (Mathf.Abs(rb.velocity.x) > moveSpeed)
                {
                    rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
                }
            }
        }
    }

    private void Crouch()
    {
        if (!isAlive) return; // Prevent crouching if the player is not alive

        if (Input.GetKeyDown(KeyCode.C) && IsGrounded())
        {
            SitDown();
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            StandUp();
        }
    }

    private void SitDown()
    {
        isSitting = true;
        currentSpeed = sitSpeed;
        anim.SetBool("isSitting", true); // Start sit animation
    }

    private void StandUp()
    {
        isSitting = false;
        currentSpeed = speed;
        anim.SetBool("isSitting", false); // End sit animation
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, environmentLayer);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, environmentLayer);
    }

    private void WallSlide()
    {
        if (!isAlive) return; // Prevent wall sliding if the player is not alive

        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            isWallSliding = true;
            anim.SetBool("isWallSliding", true);
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
            anim.SetBool("isWallSliding", false);
        }
    }

    private void WallJump()
{
    if (!isAlive) return; // Prevent wall jumping if the player is not alive

    if (isWallSliding)
    {
        isWallJumping = false;
        wallJumpingDirection = isFacingRight ? -1 : 1; // Adjust the direction based on rotation y
        wallJumpingCounter = wallJumpingTime;

        CancelInvoke(nameof(StopWallJumping));
    }
    else
    {
        wallJumpingCounter -= Time.deltaTime;
    }

    if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
    {
        isWallJumping = true;
        anim.Play("walljump"); // Play wall jumping animation by name
        rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
        wallJumpingCounter = 0f;

        if ((isFacingRight && wallJumpingDirection < 0f) || (!isFacingRight && wallJumpingDirection > 0f))
        {
            isFacingRight = !isFacingRight;
            float yRotation = isFacingRight ? 0f : 180f; // Rotate y instead of flipping scale.x
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }

        Invoke(nameof(StopWallJumping), wallJumpingDuration);
    }
}

    private void StopWallJumping()
    {
        isWallJumping = false;
        anim.SetBool("WallJumping", false); // Stop wall jumping animation after wall jumping ends
    }

    private void Flip()
    {
        if (!isAlive) return; // Prevent flipping if the player is not alive

        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            float yRotation = isFacingRight ? 0f : -180f;
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
            cameraFollowObject.CallTurn();

            if (IsGrounded())
                dust.Play();
        }
    }

    private void Shoot()
    {
        if (!isAlive) return; // Prevent shooting if the player is not alive

        if (Input.GetMouseButton(0) && canShoot && currentBullets > 0)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePosition - bulletSpawnPoint.position).normalized;

            // Instantiate and rotate the muzzle flash to face the mouse direction
            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, bulletSpawnPoint.position, Quaternion.identity);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            muzzleFlash.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            Destroy(muzzleFlash, 0.1f); // Destroy the muzzle flash after a short duration

            Fire(direction);
            StartCoroutine(FireCooldown());

            currentBullets--;
            UpdateBulletUI();

            if (currentBullets == 0)
            {
                StartCoroutine(Reload());
            }
        }
    }

    private IEnumerator FireCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(fireRate);
        canShoot = true;
    }

    private void Fire(Vector2 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>() ?? bullet.AddComponent<Rigidbody2D>();
        bulletRb.gravityScale = 0;
        bulletRb.velocity = direction * 10f;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        Destroy(bullet, 10f);
    }

    private IEnumerator Reload()
    {
        canShoot = false;
        animator.SetBool("isReload", true); // Set the reload animation to true
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); // Wait for the animation to finish
        currentBullets = maxBullets;
        UpdateBulletUI();
        animator.SetBool("isReload", false); // Set the reload animation to false
        canShoot = true;
    }

    private void UpdateBulletUI()
    {
        bulletUIText.text =  $"{currentBullets}/∞";
    }

    private IEnumerator Dash()
{
    if (!isAlive) yield break; // Prevent dashing if the player is not alive

    if (horizontal == 0f) yield break; // If no input, do not dash

    isDashing = true;

    Vector2 dashVelocity = new Vector2(horizontal * dashSpeed, 0f);
    rb.velocity = dashVelocity;

    yield return new WaitForSeconds(dashDuration);

    isDashing = false;
    dashCooldownTimer = dashCooldown;
}

    private void InitializeCursor()
    {
        if (cursorSprite != null)
        {
            cursorObject = new GameObject("CustomCursor");
            SpriteRenderer cursorRenderer = cursorObject.AddComponent<SpriteRenderer>();
            cursorRenderer.sprite = cursorSprite;
            Cursor.visible = false;
        }
    }

    private void UpdateCursor()
    {
        if (cursorObject != null)
        {
            Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cursorPosition.z = 0f;
            cursorObject.transform.position = cursorPosition;
        }
    }

    private void EnsureCorrectDirection()
    {
        Vector3 localScale = transform.localScale;
        if ((isFacingRight && localScale.x < 0f) || (!isFacingRight && localScale.x > 0f))
        {
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    public void Die()
    {
        isAlive = false;
        anim.Play("Death"); // Trigger the death animation

        // Stop all movement
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.isKinematic = true;

        // Disable player control
        this.enabled = false;
    }
}
