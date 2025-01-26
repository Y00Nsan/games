using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject startButton; // Start 버튼을 할당합니다.

    void Start()
    {
        // 게임 시작 시 일시정지 상태로 만듭니다.
        Time.timeScale = 0;
        startButton.SetActive(true); // Start 버튼을 활성화합니다.
    }

    public void StartGame()
    {
        Time.timeScale = 1; // 게임을 정상 속도로 진행합니다.
        startButton.SetActive(false); // Start 버튼을 비활성화합니다.
    }
}
