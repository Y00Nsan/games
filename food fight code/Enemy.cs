using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 3; // 최대 체력
    public int currentHealth; // 현재 체력
    public SpriteRenderer spriteRenderer; // 스프라이트 렌더러
    public float damageDuration = 0.5f; // 데미지를 받을 때 변경된 투명도가 유지되는 시간
    public float damagedAlpha = 0.5f; // 데미지를 받을 때 변경할 투명도

    private Animator anim;
    private bool isDead = false;
    private Color originalColor; // 원래 색상 저장

    private void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // 원래 색상 저장
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Enemy Damaged");
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
        // else
        // {
        //     StartCoroutine(ShowDamageEffect());
        // }
    }

    // private IEnumerator ShowDamageEffect()
    // {
    //     anim.enabled = false; // 애니메이션 일시 정지
    //     Color damagedColor = new Color(originalColor.r, originalColor.g, originalColor.b, damagedAlpha); // 투명도를 낮춘 색상
    //     spriteRenderer.color = damagedColor; // 데미지 색상으로 변경
    //     yield return new WaitForSeconds(damageDuration); // 일정 시간 대기
    //     spriteRenderer.color = originalColor; // 원래 색상으로 복귀
    //     anim.enabled = true; // 애니메이션 재개
    // }

    private void Die()
    {
        isDead = true;
        anim.SetTrigger("Die");
        Debug.Log("Enemy Died");
        GetComponent<Collider2D>().enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CupheadController player = collision.gameObject.GetComponent<CupheadController>();
            if (player != null)
            {
                player.TakeDamage(1); // 플레이어에게 데미지 주기
            }
        }
    }
}
