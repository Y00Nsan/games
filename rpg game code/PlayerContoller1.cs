using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Transactions;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;

public class PlayerContoller1 : MonoBehaviour
{
    public static PlayerContoller1 Instance;

    public Collider2D standingCollider;
    private Rigidbody2D rb;
    public float speed = 7f;
    public float Sitspeed = 3.5f;
    private float currentSpeed;
    private bool isSitting = false; 
    public float jumpForce;
    private float moveInput;
    private bool isFacingRight = true;
    private Animator anim;
    private float xAxis, yAxis;

    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;

    private bool isGrounded;
    public float checkRadius;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private TrailRenderer tr;


    private bool isWallSliding;
    private float wallSlidingSpeed = 0.2f;

    private bool isWallJumping;
    private float wallJumpingDir;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDurtion = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);

    private float jumpTimeCounter;
    public float jumpTime;
    private bool isJumping;

    bool attack = false;
    float timeSinceAttack;
    private float timeBetweenAttack;
    private float StarttimeBetweenAttack;
    public LayerMask whatIsEnemy;
    [SerializeField] Transform sideAttackTransform, UpAttackTransform, DownAttackTransform;
    [SerializeField] Vector2 sideAttackArea, UpAttackArea, DownAttackArea;
    [SerializeField] LayerMask AttackableLayer;
    [SerializeField] float damage;
    [SerializeField] GameObject slashEffect;
    public ParticleSystem dust;
    internal static readonly PlayerContoller1 Instantce;

    

    void Start()
    {
        
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = speed;
    }

    private void Awake()
    {
        if (Instance == null)
                Instance = this;
    }
    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetKeyDown(KeyCode.J);
    }

    void FixedUpdate() 
    {
        bool wasGrounded = isGrounded;
		isGrounded = false;

        if(!isWallJumping && !isSitting)
        {
            rb.velocity = new Vector2(moveInput * currentSpeed, rb.velocity.y);
        }

        if(isDashing)
        {
            return;
        }
    }

    void Update()
    {
        GetInputs();
        Attack();
        WallJump();
        WallSlide();
        Move();
        
        if(!isWallJumping)
        {
            Flip();
        }

        if(isDashing)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            SitDown();
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            StandUp();
        }

        moveInput = Input.GetAxisRaw("Horizontal"); 

        if(Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        if(Input.GetButtonDown("Jump") && IsGrounded())
        {
            dust.Play();
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if(Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        if(isGrounded == true && Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("TakeOff");
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.velocity = Vector2.up * jumpForce;
        }

        if(isGrounded == true && Input.GetKeyDown(KeyCode.W))
        {
            anim.SetTrigger("TakeOff");
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.velocity = Vector2.up * jumpForce;
        }

        if(isGrounded == true)
        {
            anim.SetBool("isJumping", false);
        }
        else
        {
            anim.SetBool("isJumping", true);
        }

        if(Input.GetKey(KeyCode.Space) && isJumping == true)
        {
            if(jumpTimeCounter > 0){
                rb.velocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            } else {
                isJumping = false;
            }
        }

        if(Input.GetKey(KeyCode.W) && isJumping == true)
        {
            if(jumpTimeCounter > 0){
                rb.velocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            } else {
                isJumping = false;
            }
        }

        if(Input.GetKeyUp(KeyCode.W))
        {
            isJumping = false;
        }

        if(Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }

        if(moveInput ==0)
        {
            anim.SetBool("isRunning", false);
        }
        else
        {
            anim.SetBool("isRunning", true);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if(isFacingRight && moveInput < 0f || !isFacingRight && moveInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;

            if(IsGrounded())
            {
                dust.Play();
            }
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && moveInput != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDir = -transform.localScale.x;
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
            rb.velocity = new Vector2(wallJumpingDir * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDir)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDurtion);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;  
    }

    void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        if(attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            anim.SetTrigger("attack");

            if(yAxis == 0 || yAxis < 0 && IsGrounded())
            {
                Hit(sideAttackTransform, sideAttackArea);
                Instantiate(slashEffect, sideAttackTransform);
            }
            else if(yAxis > 0)
            {
                Hit(UpAttackTransform, UpAttackArea);
                SlashEffectAtAngle(slashEffect, 80, UpAttackTransform);
            }
            else if(yAxis < 0 && !IsGrounded())
            {
                Hit(DownAttackTransform, DownAttackArea);
                SlashEffectAtAngle(slashEffect, -90, DownAttackTransform);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(sideAttackTransform.position, sideAttackArea);
        Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
        Gizmos.DrawWireCube(DownAttackTransform.position, DownAttackArea);
    }

    void Hit(Transform attackTransform, Vector2 attackArea)
    {
        Collider2D[] objectToHit = Physics2D.OverlapBoxAll(attackTransform.position, attackArea, 0, AttackableLayer);

        if(objectToHit.Length > 0)
        {
            Debug.Log("Hit");
        }
        for(int i = 0; i < objectToHit.Length; i++)
        {
            if(objectToHit[i].GetComponent<Enemy>() != null)
            {
                objectToHit[i].GetComponent<Enemy>().EnemyHit(damage);
            }
        }
    }

    void SlashEffectAtAngle(GameObject slashEffect, int effectangle, Transform attackTransform)
    {
        slashEffect = Instantiate(slashEffect, attackTransform);
        slashEffect.transform.eulerAngles = new Vector3(0, 0, effectangle);
        slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }

    void SitDown()
    {
        isSitting = true;
        anim.SetBool("Sit", true);
        standingCollider.enabled = false;
        currentSpeed = Sitspeed;
    }

    void StandUp()
    {
        isSitting = false;
        anim.SetBool("Sit", false);
        standingCollider.enabled = true;
        currentSpeed = speed;
    }

    void Move()
    {
        float move = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(move * currentSpeed, rb.velocity.y);
    }
}