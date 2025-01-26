using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveWorld : MonoBehaviour
{
    public float moveSpeed = 5f;
    private bool canMoveToNextScene = false;
    private string nextSceneName;
    public GameObject targetGameObject; // 충돌 시 활성화할 게임 오브젝트

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float lastMoveX = 0f;
    private float lastMoveY = 0f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found on this GameObject.");
        }
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on this GameObject.");
        }

        SceneManager sceneManager = FindObjectOfType<SceneManager>();
        if (sceneManager != null)
        {
            nextSceneName = sceneManager.nextSceneName;
        }
        else
        {
            Debug.LogError("SceneManager not found in the scene.");
        }

        if (targetGameObject != null)
        {
            targetGameObject.SetActive(false); // 게임 시작 시 비활성화 상태로 설정
        }
        else
        {
            Debug.LogError("Target GameObject is not assigned.");
        }
    }

    private void Update()
    {
        Move();
        if (canMoveToNextScene && Input.GetKeyDown(KeyCode.F))
        {
            LoadNextScene();
        }
    }

    void Move()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (moveX != 0 || moveY != 0)
        {
            lastMoveX = moveX;
            lastMoveY = moveY;
        }

        Vector3 movement = new Vector3(moveX, moveY, 0f).normalized * moveSpeed * Time.deltaTime;
        transform.Translate(movement);

        UpdateAnimation(moveX, moveY);
    }

    void UpdateAnimation(float moveX, float moveY)
    {
        animator.SetFloat("MoveX", moveX);
        animator.SetFloat("MoveY", moveY);

        bool isIdle = (moveX == 0 && moveY == 0);
        SetIdleAnimations(isIdle);
    }

    void SetIdleAnimations(bool isIdle)
    {
        animator.SetBool("IdleRight", isIdle && lastMoveX > 0 && lastMoveY == 0);
        animator.SetBool("IdleLeft", isIdle && lastMoveX < 0 && lastMoveY == 0);
        animator.SetBool("IdleUp", isIdle && lastMoveY > 0 && lastMoveX == 0);
        animator.SetBool("IdleDown", isIdle && lastMoveY < 0 && lastMoveX == 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Stage"))
        {
            canMoveToNextScene = true;
            if (targetGameObject != null)
            {
                targetGameObject.SetActive(true); // 충돌 시 게임 오브젝트 활성화
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Stage"))
        {
            canMoveToNextScene = false;
            if (targetGameObject != null)
            {
                targetGameObject.SetActive(false); // 충돌 끝났을 때 게임 오브젝트 비활성화
            }
        }
    }

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("Next scene name is not set.");
        }
    }
}
