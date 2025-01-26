using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    public Animator animator; // 애니메이터 컴포넌트를 드래그하여 연결
    public string animationName; // 실행할 애니메이션 이름
    public Collider2D colliderToRemove;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // 충돌체의 태그를 확인합니다
        {
            // 애니메이션 실행
            animator.Play(animationName);
            AudioManger.Instance.PlaySFX("ne");

            // 콜라이더 제거
            Destroy(colliderToRemove);
        }
    }
}
