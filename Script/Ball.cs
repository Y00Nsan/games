using System.Collections;
using UnityEngine;

public class ColorChangingSprite : MonoBehaviour
{
    public SpriteRenderer spriteRenderer1; // 첫 번째 스프라이트 렌더러
    public SpriteRenderer spriteRenderer2; // 두 번째 스프라이트 렌더러
    public float interval = 1.0f; // 색상 변경 간격 (초 단위)
    public float fadeDuration = 0.5f; // 페이드 인/아웃 시간 (초 단위)
    public float colorVariation = 0.1f; // 색상 변화 범위

    private Color baseColor;
    private Color currentColor;
    private Color nextColor;

    void Start()
    {
        baseColor = GetRandomComfortableColor();
        currentColor = baseColor;
        spriteRenderer1.color = currentColor;

        StartCoroutine(ChangeColorRoutine());
    }

    IEnumerator ChangeColorRoutine()
    {
        while (true)
        {
            // 다음 색상 랜덤으로 설정
            nextColor = GetVariatedColor(baseColor);

            // 현재 색상에서 다음 색상으로 페이드 인/아웃
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                Color lerpedColor = Color.Lerp(currentColor, nextColor, t / fadeDuration);
                spriteRenderer1.color = lerpedColor;
                spriteRenderer2.color = AdjustSecondColor(lerpedColor);
                yield return null;
            }

            // 완전히 다음 색상으로 변경
            spriteRenderer1.color = nextColor;
            spriteRenderer2.color = AdjustSecondColor(nextColor);
            currentColor = nextColor;

            // 지정된 시간 동안 대기
            yield return new WaitForSeconds(interval);
        }
    }

    // 기준 색상에서 약간의 변화를 가하여 새로운 색상 생성
    Color GetVariatedColor(Color baseColor)
    {
        float hueVariation = Random.Range(-colorVariation, colorVariation);
        float saturationVariation = Random.Range(-colorVariation, colorVariation);
        float valueVariation = Random.Range(-colorVariation, colorVariation);

        float h, s, v;
        Color.RGBToHSV(baseColor, out h, out s, out v);

        h = Mathf.Clamp01(h + hueVariation);
        s = Mathf.Clamp01(s + saturationVariation);
        v = Mathf.Clamp01(v + valueVariation);

        return Color.HSVToRGB(h, s, v);
    }

    // 눈에 덜 부담되는 랜덤 색상 생성 함수
    Color GetRandomComfortableColor()
    {
        float hue = Random.Range(0f, 1f); // 0~1 사이의 랜덤 값
        float saturation = Random.Range(0.2f, 0.5f); // 낮은 채도 값
        float value = Random.Range(0.8f, 1f); // 밝은 값

        return Color.HSVToRGB(hue, saturation, value);
    }

    // 첫 번째 색상에 따라 두 번째 색상을 조정하는 함수
    Color AdjustSecondColor(Color firstColor)
    {
        float r = firstColor.r;
        float g = firstColor.g;
        float b = firstColor.b;

        if (r <= 0.45f) // RGB 값은 0~1 사이의 값이므로 115/255 = 0.45
        {
            r = Mathf.Clamp01(r - 20f / 255f);
        }
        else
        {
            r = Mathf.Clamp01(r + 20f / 255f);
        }

        return new Color(r, g, b);
    }
}
