using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float groundJumpForce = 10f;
    public float airJumpForce = 5f;
    public Transform targetPosition;
    public Transform retryPosition;
    public Collider2D specialCollider; // 기존 콜라이더
    public Collider2D timerStartCollider; // 새로운 콜라이더
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool canJump = true;
    private bool movingRight = true;
    private bool isBeingPulled = false;

    public Camera mainCamera;
    public TextMeshProUGUI timeText;
    public Button actionButton; // 통합된 버튼
    private float playTime;
    private bool stopTime = true;

    public Camera[] otherCameras;
    public float pullSpeed = 10f; // 끌어당기는 속도 변수 추가

    private EventSystem eventSystem;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera.gameObject.SetActive(false);
        actionButton.onClick.AddListener(OnActionButtonClicked);

        eventSystem = EventSystem.current; // EventSystem 초기화
    }

    void Update()
    {
        if (!isBeingPulled)
        {
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && canJump)
            {
                Jump();
            }
        }

        if (!stopTime)
        {
            playTime += Time.deltaTime;
            int minutes = Mathf.FloorToInt(playTime / 60);
            int seconds = Mathf.FloorToInt(playTime % 60);
            timeText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }

    private void Move()
    {
        float horizontalVelocity = movingRight ? moveSpeed : -moveSpeed;
        rb.velocity = new Vector2(horizontalVelocity, rb.velocity.y);
    }

    private void StopMoving()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    private void Jump()
    {
        float jumpForce = isGrounded ? groundJumpForce : airJumpForce;
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);

        if (!isGrounded)
        {
            Move();
        }

        if (isGrounded)
        {
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;
            canJump = true;
            StopMoving();
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            movingRight = !movingRight;
            if (!isGrounded)
            {
                Move();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            canJump = false;
        }

        if (collider.gameObject.layer == LayerMask.NameToLayer("Goal"))
        {
            StartCoroutine(MagnetEffect());
        }

        if (collider == specialCollider) // 기존 콜라이더와 충돌 시 이동
        {
            transform.position = targetPosition.position;
            StopPlayerAndCamera();
            stopTime = true;
        }

        if (collider == timerStartCollider) // 새로운 콜라이더와 충돌 시 타이머 시작
        {
            StartTimer();
        }
    }

    private IEnumerator MagnetEffect()
    {
        isBeingPulled = true;
        while (Vector2.Distance(transform.position, targetPosition.position) > 0.1f)
        {
            transform.position = Vector2.Lerp(transform.position, targetPosition.position, pullSpeed * Time.deltaTime);
            yield return null;
        }
        isBeingPulled = false;
        StopPlayerAndCamera();
        stopTime = true;
    }

    private void StopPlayerAndCamera()
    {
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        foreach (Camera cam in otherCameras)
        {
            cam.gameObject.SetActive(false);
        }

        mainCamera.gameObject.SetActive(true);
        timeText.gameObject.SetActive(true);

        mainCamera.transform.position = new Vector3(targetPosition.position.x, targetPosition.position.y, mainCamera.transform.position.z);

        actionButton.gameObject.SetActive(true);
    }

    private void OnActionButtonClicked()
    {
        if (stopTime)
        {
            // Start 버튼 기능
            playTime = 0; // 타이머 0초로 초기화
            timeText.text = "Time: 00:00";
            stopTime = false; // 타이머 시작
            eventSystem.SetSelectedGameObject(null);
        }
        else
        {
            // Retry 버튼 기능
            transform.position = retryPosition.position;

            rb.velocity = Vector2.zero;
            rb.isKinematic = false;
            isGrounded = true;
            canJump = true;
            movingRight = true;
            isBeingPulled = false;
            stopTime = true; // 타이머 멈춤

            playTime = 0; // 타이머 초기화
            timeText.text = "Time: 00:00";

            // 버튼과 타이머를 숨기지 않도록 수정
            // actionButton.gameObject.SetActive(false);
            mainCamera.gameObject.SetActive(false);
            // timeText.gameObject.SetActive(false);
        }
    }

    private void StartTimer()
    {
        stopTime = false;
    }
}
