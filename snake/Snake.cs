using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D))]
public class Snake : MonoBehaviour
{
    public Transform segmentPrefab;
    public Vector2Int direction = Vector2Int.right;
    public float speed = 20f;
    public float speedMultiplier = 1f;
    public int initialSize = 4;
    public bool moveThroughWalls = false;
    public Button restartButton; // Restart 버튼을 연결할 public 변수

    private List<Transform> segments = new List<Transform>();
    private Vector2Int input;
    private float nextUpdate;
    private bool isGameOver = false; // 게임 오버 상태를 나타내는 변수
    private Vector2 touchStartPos;

    private void Start()
    {
        restartButton.gameObject.SetActive(false); // 시작할 때는 버튼 비활성화
        restartButton.onClick.AddListener(ResetState); // 버튼 클릭 리스너를 ResetState로 설정

        // 게임 시작 시 초기 설정
        ResetState();
    }

    private void Update()
    {
        if (isGameOver) return; // 게임 오버 상태에서는 Update를 실행하지 않음

        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼이 클릭될 때
        {
            touchStartPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0)) // 마우스 왼쪽 버튼이 해제될 때
        {
            Vector2 touchEndPos = Input.mousePosition;
            Vector2 swipeDirection = (touchEndPos - touchStartPos).normalized;

            // 수평 스와이프
            if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
            {
                if (swipeDirection.x > 0) // 오른쪽으로 스와이프
                {
                    input = Vector2Int.right;
                }
                else // 왼쪽으로 스와이프
                {
                    input = Vector2Int.left;
                }
            }
            // 수직 스와이프
            else
            {
                if (swipeDirection.y > 0) // 위로 스와이프
                {
                    input = Vector2Int.up;
                }
                else // 아래로 스와이프
                {
                    input = Vector2Int.down;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (isGameOver) return; // 게임 오버 상태에서는 FixedUpdate를 실행하지 않음

        // Wait until the next update before proceeding
        if (Time.time < nextUpdate)
        {
            return;
        }

        // Set the new direction based on the input
        if (input != Vector2Int.zero)
        {
            direction = input;
            input = Vector2Int.zero; // 방향 설정 후 input 초기화
        }

        // Set each segment's position to be the same as the one it follows. We
        // must do this in reverse order so the position is set to the previous
        // position, otherwise they will all be stacked on top of each other.
        for (int i = segments.Count - 1; i > 0; i--)
        {
            segments[i].position = segments[i - 1].position;
        }

        // Move the snake in the direction it is facing
        // Round the values to ensure it aligns to the grid
        int x = Mathf.RoundToInt(transform.position.x) + direction.x;
        int y = Mathf.RoundToInt(transform.position.y) + direction.y;
        transform.position = new Vector2(x, y);

        // Set the next update time based on the speed
        nextUpdate = Time.time + (1f / (speed * speedMultiplier));
    }

    public void Grow()
    {
        Transform segment = Instantiate(segmentPrefab);
        segment.position = segments[segments.Count - 1].position;
        segments.Add(segment);
    }

    public void ResetState()
    {
        direction = Vector2Int.right;
        transform.position = Vector3.zero;

        // Start at 1 to skip destroying the head
        for (int i = 1; i < segments.Count; i++)
        {
            Destroy(segments[i].gameObject);
        }

        // Clear the list but add back this as the head
        segments.Clear();
        segments.Add(transform);

        // -1 since the head is already in the list
        for (int i = 0; i < initialSize - 1; i++)
        {
            Grow();
        }

        isGameOver = false; // 게임 오버 상태 해제
        restartButton.gameObject.SetActive(false); // 버튼 비활성화
    }

    public bool Occupies(int x, int y)
    {
        foreach (Transform segment in segments)
        {
            if (Mathf.RoundToInt(segment.position.x) == x &&
                Mathf.RoundToInt(segment.position.y) == y)
            {
                return true;
            }
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision with: " + other.gameObject.tag); // 충돌 객체의 태그를 출력

        if (other.gameObject.CompareTag("Food"))
        {
            Grow();
        }
        else if (other.gameObject.CompareTag("Obstacle") || other.gameObject.CompareTag("Wall"))
        {
            if (moveThroughWalls && other.gameObject.CompareTag("Wall"))
            {
                Traverse(other.transform);
            }
            else
            {
                GameOver();
            }
        }
    }

    private void Traverse(Transform wall)
    {
        Vector3 position = transform.position;

        if (direction.x != 0f)
        {
            position.x = Mathf.RoundToInt(-wall.position.x + direction.x);
        }
        else if (direction.y != 0f)
        {
            position.y = Mathf.RoundToInt(-wall.position.y + direction.y);
        }

        transform.position = position;
    }

    private void GameOver()
    {
        isGameOver = true; // 게임 오버 상태 설정
        restartButton.gameObject.SetActive(true); // 버튼 활성화
    }
}
