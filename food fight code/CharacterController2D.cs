using System.Collections;
using TMPro;
using UnityEngine;

public class CupheadController : MonoBehaviour
{
    public static CupheadController instance;
    public bool isAttacking = false;
    private Rigidbody2D rb;
    public float moveSpeed = 7f;
    public float Sitspeed = 3.5f;
    private float currentSpeed;
    public float jumpForce = 10f;
    public float doubleJumpForce = 8f;
    public float dashDistance = 5f;
    public float dashCooldown = 1f;
    public float dashDuration = 0.3f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public Collider2D standingCollider;

    private bool isGrounded;
    private bool canDash = true;
    private bool isDashing;
    private bool isSitting;
    private bool canDoubleJump;
    private float moveInput;
    public Animator anim;
    private bool isFacingRight = true;
    private bool crouchHeld;

    // Health variables
    public int maxHealth = 3;
    public int currentHealth;
    private bool isInvincible = false; // Invincibility flag

    // Time manipulation variables
    private float originalFixedDeltaTime;

    // Attack variables
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 1;

    public TMP_Text health;

    // Combo attack variables
    private int comboStep = 0;
    private float comboCooldown = 0.5f; // Cooldown between combo steps
    private float lastAttackTime;
    public Animator externalAnimator; // 외부 게임 오브젝트의 애니메이터
    public string[] playerAnimationNames; // 플레이어 애니메이션 이름 배열
    public string[] externalAnimationNames; // 외부 애니메이션 이름 배열

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentSpeed = moveSpeed;
        currentHealth = maxHealth;
        originalFixedDeltaTime = Time.fixedDeltaTime;
        SetMaxHealth(maxHealth);
        SetHealthText(currentHealth);
    }

    private void Update()
    {
        HandleInput();
        HandleMovement();
        HandleDash();
        HandleJump();
        HandleCrouch();
        Flip();
        UpdateAnimations();

        // Handle attack cooldown
        if (Time.time - lastAttackTime > comboCooldown)
        {
            comboStep = 0;
        }

        Attack();
    }

    private void HandleInput()
    {
        if (!isDashing)
        {
            moveInput = Input.GetAxisRaw("Horizontal");
        }
        crouchHeld = Input.GetKey(KeyCode.S);
    }

    private void HandleMovement()
    {
        if (!anim.GetBool("isGroundDashing") && !anim.GetBool("isAirDashing"))
        {
            if (!isSitting)
            {
                if (isAttacking)
                {
                    rb.velocity = new Vector2(moveInput * currentSpeed, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(moveInput * currentSpeed, rb.velocity.y);
                }
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
    }

    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isSitting)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashDistance / dashDuration, 0f);

        if (isGrounded)
        {
            anim.SetBool("isGroundDashing", true);
        }
        else
        {
            anim.SetBool("isAirDashing", true);
        }

        yield return new WaitForSeconds(dashDuration);
        rb.gravityScale = originalGravity;
        anim.SetBool("isGroundDashing", false);
        anim.SetBool("isAirDashing", false);
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void HandleJump()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if (isGrounded && !wasGrounded && crouchHeld)
        {
            SitDown();
        }

        if (isGrounded)
        {
            canDoubleJump = true;
        }

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        else if (!isGrounded && canDoubleJump && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
            canDoubleJump = false;
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.S) && isGrounded)
        {
            SitDown();
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            StandUp();
        }
    }

    private void SitDown()
    {
        isSitting = true;
        standingCollider.enabled = false;
        currentSpeed = Sitspeed;
    }

    private void StandUp()
    {
        isSitting = false;
        standingCollider.enabled = true;
        currentSpeed = moveSpeed;
    }

    private void Flip()
    {
        if (!isDashing)
        {
            if (isFacingRight && moveInput < 0f || !isFacingRight && moveInput > 0f)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
        }
    }

    private void UpdateAnimations()
    {
        anim.SetBool("isRunning", moveInput != 0 && isGrounded && !isSitting && !anim.GetBool("isGroundDashing") && !anim.GetBool("isAirDashing"));
        anim.SetBool("isJumping", !isGrounded && !anim.GetBool("isAirDashing"));
        anim.SetBool("isCrouching", isSitting);
    }

    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (Time.time - lastAttackTime <= comboCooldown)
            {
                comboStep = Mathf.Clamp(comboStep + 1, 1, playerAnimationNames.Length);
            }
            else
            {
                comboStep = 1; // Reset comboStep if it's a new attack
            }

            lastAttackTime = Time.time;
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;

        string playerAnimationName = "";
        string externalAnimationName = "";

        // 플레이어 애니메이션과 외부 애니메이션을 가져옴
        if (comboStep - 1 < playerAnimationNames.Length && comboStep - 1 < externalAnimationNames.Length)
        {
            playerAnimationName = playerAnimationNames[comboStep - 1];
            externalAnimationName = externalAnimationNames[comboStep - 1];
        }

        if (!string.IsNullOrEmpty(playerAnimationName))
        {
            anim.Play(playerAnimationName);
        }

        if (!string.IsNullOrEmpty(externalAnimationName))
        {
            externalAnimator.Play(externalAnimationName); // 외부 애니메이터의 애니메이션 실행
        }

        // Simulate attack duration (adjust this to match your animation length)
        yield return new WaitForSeconds(0.2f);

        isAttacking = false;

        // Add delay after the attack
        yield return new WaitForSeconds(comboCooldown); // 딜레이 추가
    }

    // Health methods
    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        Debug.Log("damaged");
        SetHealthText(currentHealth);

        if (currentHealth > 0)
        {
            StartCoroutine(HandleDamageEffects());
        }
        else
        {
            Die();
        }
    }

    private IEnumerator HandleDamageEffects()
    {
        isInvincible = true;

        // Time stop effect
        Time.timeScale = 0f;
        Time.fixedDeltaTime = originalFixedDeltaTime * Time.timeScale;
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = originalFixedDeltaTime;

        // Invincibility period
        StartCoroutine(Invincibility());

        // Move in the direction faced
        float direction = isFacingRight ? 1f : -1f;
        rb.velocity = new Vector2(direction * dashDistance / dashDuration, 0f);
        yield return new WaitForSeconds(dashDuration);

        isInvincible = false;
    }

    private IEnumerator Invincibility()
    {
        // Optionally, add visual feedback for invincibility
        yield return new WaitForSeconds(2f);
    }

    private void Die()
    {
        // Handle death (e.g., play animation, disable controls, etc.)
        this.enabled = false;
        rb.velocity = Vector2.zero;
        SetHealthText(currentHealth);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is with an object on the Enemy layer
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // Take damage from the enemy (damage amount can be adjusted as needed)
            TakeDamage(1); // Example damage amount
        }
    }

    private void SetMaxHealth(int maxHealth)
    {
        if (health != null)
        {
            health.text = "" + maxHealth.ToString();
        }
    }

    private void SetHealthText(int healthValue)
    {
        if (health != null)
        {
            health.text = Mathf.Max(healthValue, 0).ToString();
        }
    }
}
