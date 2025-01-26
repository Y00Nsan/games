using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TicTacToe : MonoBehaviour
{
    public GameObject xPrefab;
    public GameObject oPrefab;
    public Button[] buttons; // 각 버튼들을 배열로 저장
    public TextMeshProUGUI winText; // 승리 메시지를 표시할 Text UI
    public GameObject gameCanvas; // 캔버스
    [SerializeField] private Button retryButton; // 다시하기 버튼
    private bool isXTurn = true; // true = X's turn, false = O's turn
    private int[,] board = new int[3, 3]; // 0 = empty, 1 = X, 2 = O
    private bool gameEnded = false;
    private float shrinkSpeed = 0.03f; // 축소 속도
    private float minScale = 0.1f; // 최소 스케일
    private GameObject winningLine;

    void Start()
    {
        if (winText != null)
        {
            winText.gameObject.SetActive(false); // 승리 메시지 비활성화
        }
        else
        {
            Debug.LogError("winText is not assigned in the Inspector.");
        }

        if (retryButton != null)
        {
            retryButton.gameObject.SetActive(false); // 다시하기 버튼 비활성화
            retryButton.onClick.AddListener(RestartGame); // 다시하기 버튼 클릭 이벤트 연결
        }
        else
        {
            Debug.LogError("retryButton is not assigned in the Inspector.");
        }

        ClearBoard(); // 보드 초기화
    }

    public void OnCellClicked(int cellIndex)
    {
        if (gameEnded) return; // 게임이 끝나면 클릭 이벤트를 무시

        if (buttons == null || buttons.Length == 0)
        {
            Debug.LogError("buttons array is not assigned or empty in the Inspector.");
            return;
        }

        int x = cellIndex / 3;
        int y = cellIndex % 3;

        if (board[x, y] == 0) // Check if the cell is empty
        {
            GameObject newMark;
            if (isXTurn)
            {
                if (xPrefab != null)
                {
                    newMark = Instantiate(xPrefab, buttons[cellIndex].transform);
                    board[x, y] = 1;
                }
                else
                {
                    Debug.LogError("xPrefab is not assigned in the Inspector.");
                    return;
                }
            }
            else
            {
                if (oPrefab != null)
                {
                    newMark = Instantiate(oPrefab, buttons[cellIndex].transform);
                    board[x, y] = 2;
                }
                else
                {
                    Debug.LogError("oPrefab is not assigned in the Inspector.");
                    return;
                }
            }

            if (CheckWin(out Vector2 start, out Vector2 end))
            {
                GameOver();
                DrawWinningLine(start, end); // 승리 라인 그리기
                return;
            }
            else if (CheckDraw())
            {
                PlayDrawAnimation();
                return;
            }

            isXTurn = !isXTurn; // Toggle turn
        }
    }

    bool CheckWin(out Vector2 start, out Vector2 end)
    {
        start = Vector2.zero;
        end = Vector2.zero;

        // Check rows
        for (int i = 0; i < 3; i++)
        {
            if (board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2] && board[i, 0] != 0)
            {
                start = buttons[i * 3].transform.position;
                end = buttons[i * 3 + 2].transform.position;
                return true;
            }
        }
        // Check columns
        for (int i = 0; i < 3; i++)
        {
            if (board[0, i] == board[1, i] && board[1, i] == board[2, i] && board[0, i] != 0)
            {
                start = buttons[i].transform.position;
                end = buttons[i + 6].transform.position;
                return true;
            }
        }
        // Check diagonals
        if (board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2] && board[0, 0] != 0)
        {
            start = buttons[0].transform.position;
            end = buttons[8].transform.position;
            return true;
        }
        if (board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0] && board[0, 2] != 0)
        {
            start = buttons[2].transform.position;
            end = buttons[6].transform.position;
            return true;
        }

        return false;
    }

    bool CheckDraw()
    {
        foreach (int cell in board)
        {
            if (cell == 0)
                return false;
        }
        return true;
    }

    void PlayDrawAnimation()
    {
        gameEnded = true; // 게임 종료 플래그 설정

        foreach (Button button in buttons)
        {
            foreach (Transform child in button.transform)
            {
                Animator animator = child.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.enabled = false; // 애니메이션을 중지합니다.
                }
            }
        }

        InvokeRepeating("ShrinkMarks", 0.0f, 0.05f); // 일정 시간 간격으로 축소 함수 호출
        Invoke("RestartGame", 1.5f); // 1.5초 후 게임 재시작
    }

    void ShrinkMarks()
    {
        foreach (Button button in buttons)
        {
            foreach (Transform child in button.transform)
            {
                Vector3 scale = child.localScale;
                scale -= new Vector3(shrinkSpeed, shrinkSpeed, shrinkSpeed); // 축소
                scale = new Vector3(Mathf.Max(minScale, scale.x), Mathf.Max(minScale, scale.y), Mathf.Max(minScale, scale.z)); // 최소 스케일 이상 유지
                child.localScale = scale;
            }
        }
    }

    void GameOver()
    {
        if (winText != null)
        {
            winText.text = isXTurn ? "X Wins!" : "O Wins!";
            winText.gameObject.SetActive(true);
        }
        retryButton.gameObject.SetActive(true); // 다시하기 버튼 활성화
        gameEnded = true;
    }

    public void RestartGame()
    {
        CancelInvoke(); // InvokeRepeating 중지
        ClearBoard();
        if (winText != null)
        {
            winText.gameObject.SetActive(false);
        }
        if (retryButton != null)
        {
            retryButton.gameObject.SetActive(false); // 다시하기 버튼 비활성화
        }
        if (winningLine != null)
        {
            Destroy(winningLine); // 승리 라인 제거
            winningLine = null; // 변수 초기화
        }
        gameEnded = false;
    }

    void ClearBoard()
    {
        foreach (Button button in buttons)
        {
            foreach (Transform child in button.transform)
            {
                Destroy(child.gameObject);
            }
        }
        board = new int[3, 3];
        isXTurn = true;
    }

    void DrawWinningLine(Vector2 start, Vector2 end)
    {
        if (winningLine != null)
        {
            Destroy(winningLine); // 기존 승리 라인이 있으면 제거
        }

        winningLine = new GameObject("WinningLine");
        winningLine.transform.SetParent(gameCanvas.transform, false); // 캔버스의 자식으로 설정

        RectTransform rectTransform = winningLine.AddComponent<RectTransform>();
        Image image = winningLine.AddComponent<Image>();
        image.color = Color.white; // 하얀색으로 설정

        Vector2 direction = end - start;
        float distance = direction.magnitude;

        // 수직 라인 또는 수평 라인인 경우
        if (start.y == end.y || start.x == end.x)
        {
            rectTransform.sizeDelta = new Vector2(1225f, 60f);
        }
        // 대각선 라인인 경우
        else
        {
            rectTransform.sizeDelta = new Vector2(1725f, 60f);
        }

        rectTransform.pivot = new Vector2(0, 0.5f);
        rectTransform.position = start;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);

        // 팝업 애니메이션
        StartCoroutine(PopUpAnimation(rectTransform));
    }

    System.Collections.IEnumerator PopUpAnimation(RectTransform rectTransform)
    {
        Vector3 originalScale = rectTransform.localScale;
        rectTransform.localScale = Vector3.zero;

        float elapsedTime = 0f;
        float duration = 0.5f; // 팝업 애니메이션 지속 시간

        while (elapsedTime < duration)
        {
            rectTransform.localScale = Vector3.Lerp(Vector3.zero, originalScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.localScale = originalScale;
    }
}
