using System.Collections;
using UnityEngine;

public class ChangeColorOverTime : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // 색상을 변경할 스프라이트 렌더러
    public float interval = 1.0f; // 색상 변경 간격 (초 단위)
    public float fadeDuration = 0.5f; // 페이드 인/아웃 시간 (초 단위)
    public Color[] colors; // 변경할 색상 목록

    private Color currentColor;
    private Color nextColor;
    private int currentColorIndex = 0;

    void Start()
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component is not assigned.");
            return;
        }

        if (colors == null || colors.Length == 0)
        {
            Debug.LogError("No colors assigned in the colors array.");
            return;
        }

        // 시작할 때 첫 번째 색상 설정
        currentColor = colors[currentColorIndex];
        spriteRenderer.color = currentColor;

        StartCoroutine(ChangeColorRoutine());
    }

    IEnumerator ChangeColorRoutine()
    {
        while (true)
        {
            // 다음 색상 설정
            currentColorIndex = (currentColorIndex + 1) % colors.Length;
            nextColor = colors[currentColorIndex];

            // 현재 색상에서 다음 색상으로 페이드 인/아웃
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                spriteRenderer.color = Color.Lerp(currentColor, nextColor, t / fadeDuration);
                yield return null;
            }

            // 완전히 다음 색상으로 변경
            spriteRenderer.color = nextColor;

            // 현재 색상 업데이트
            currentColor = nextColor;

            // 지정된 시간 동안 대기
            yield return new WaitForSeconds(interval);
        }
    }
}
