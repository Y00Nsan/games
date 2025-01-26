using System;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public string nextSceneName;  // 이동할 씬 이름을 저장할 변수
    public Sprite defaultSprite; // 기본 스프라이트
    public Sprite changedSprite; // 변경할 스프라이트
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer가 존재하지 않습니다!");
        }
        else
        {
            spriteRenderer.sprite = defaultSprite; // 기본 스프라이트 설정
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            spriteRenderer.sprite = changedSprite; // 플레이어와 충돌 시 스프라이트 변경
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            spriteRenderer.sprite = defaultSprite; // 플레이어와의 충돌이 끝나면 기본 스프라이트로 변경
        }
    }

    internal static void LoadScene(string v)
    {
        throw new NotImplementedException();
    }
}
