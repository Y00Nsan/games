using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;
    public float fadeDuration = 1f;

    private Image fadeImage;
    private Canvas fadeCanvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            CreateFadeUI();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void CreateFadeUI()
    {
        // Create Canvas
        fadeCanvas = new GameObject("FadeCanvas").AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 100; // Ensure the canvas is on top

        // Create Image
        fadeImage = new GameObject("FadeImage").AddComponent<Image>();
        fadeImage.transform.SetParent(fadeCanvas.transform, false);
        fadeImage.rectTransform.anchorMin = Vector2.zero;
        fadeImage.rectTransform.anchorMax = Vector2.one;
        fadeImage.rectTransform.sizeDelta = Vector2.zero;
        fadeImage.color = new Color(0, 0, 0, 0); // Start transparent
    }

    public void FadeOut(System.Action onFadeComplete)
    {
        StartCoroutine(FadeOutRoutine(onFadeComplete));
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeOutRoutine(System.Action onFadeComplete)
    {
        float timer = 0f;
        while (timer <= fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(1f);
        onFadeComplete?.Invoke();
    }

    private IEnumerator FadeInRoutine()
    {
        float timer = 0f;
        while (timer <= fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(0f);
    }

    private void SetAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }
}
