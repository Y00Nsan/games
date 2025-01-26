using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ShowUIAndChangeScene : MonoBehaviour
{
    public GameObject uiObject; // UI 요소
    public Image uiImage; // UI 요소 내 이미지
    public Text uiText1;  // UI 요소 내 첫 번째 텍스트
    public Text uiText2;  // UI 요소 내 두 번째 텍스트
    public Sprite[] images;  // 변경할 이미지들
    public string[] texts1;  // 첫 번째 텍스트 변경할 내용들
    public string[] texts2;  // 두 번째 텍스트 변경할 내용들
    public Button[] buttons; // 버튼들
    public Button changeSceneButton; // 씬 변경 버튼
    public string[] sceneNames; // 변경할 씬 이름들
    public Scrollbar scroll;

    private int currentIndex = -1; // 현재 선택된 버튼 인덱스

    void Start()
    {
        scroll.value = 1;
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // 로컬 변수로 인덱스 저장
            buttons[i].onClick.AddListener(() => OnButtonClicked(index));
        }

        // 씬 변경 버튼에 리스너 추가
        if (changeSceneButton != null)
        {
            changeSceneButton.onClick.AddListener(ChangeSceneWithFade);
        }
    }

    public void OnButtonClicked(int index)
    {
        // UI 요소를 활성화하고 이미지와 텍스트를 변경
        uiObject.SetActive(true);
        uiImage.sprite = images[index];
        uiText1.text = texts1[index];
        uiText2.text = texts2[index];
        currentIndex = index; // 현재 인덱스 저장
    }

    public void ChangeSceneWithFade()
    {
        if (currentIndex >= 0 && currentIndex < sceneNames.Length)
        {
            string sceneName = sceneNames[currentIndex];
            StartCoroutine(ChangeSceneRoutine(sceneName));
        }
        else
        {
            Debug.LogError("Invalid scene index");
        }
    }

    private IEnumerator ChangeSceneRoutine(string sceneName)
    {
        // Fade out
        FadeManager.Instance.FadeOut(() =>
        {
            // Load the new scene
            SceneManager.LoadScene(sceneName);
        });

        // Wait for the scene to load
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == sceneName);

        // Fade in
        FadeManager.Instance.FadeIn();
    }
}
