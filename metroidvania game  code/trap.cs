using System.Collections;
using UnityEngine;

public class TeleportOnCollision : MonoBehaviour
{
    [SerializeField] private Animator animator; // FadeIn/FadeOut 애니메이션을 제어할 Animator
    [SerializeField] private Transform teleportTarget; // 텔레포트 위치
    [SerializeField] private float delay = 0.2f; // 충돌 후 대기 시간

    private bool isTeleporting = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isTeleporting && other.CompareTag("Player")) // 플레이어와 충돌 시
        {
            StartCoroutine(TeleportPlayer(other.gameObject));
        }
    }

    private IEnumerator TeleportPlayer(GameObject player)
    {
        isTeleporting = true;

        // 충돌 후 0.2초 대기
        yield return new WaitForSeconds(delay);

        // FadeOut 애니메이션 실행
        animator.SetTrigger("FadeIn");

        // FadeOut이 끝날 때까지 대기 (애니메이션 시간에 맞춰 조정)
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // 플레이어를 지정한 위치로 이동
        player.transform.position = teleportTarget.position;

        // FadeIn 애니메이션 실행
        animator.SetTrigger("FadeOut");

        // FadeIn이 끝날 때까지 대기 (애니메이션 시간에 맞춰 조정)
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        isTeleporting = false;
    }
}
