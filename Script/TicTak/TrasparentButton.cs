using UnityEngine;
using UnityEngine.UI;

public class TransparentButton : MonoBehaviour
{
    void Start()
    {
        // 버튼의 Image 컴포넌트를 투명하게 설정
        Button button = GetComponent<Button>();
        if (button != null)
        {
            Image image = button.GetComponent<Image>();
            if (image != null)
            {
                Color tempColor = image.color;
                tempColor.a = 0f; // Alpha 값을 0으로 설정
                image.color = tempColor;
            }
        }
    }
}
