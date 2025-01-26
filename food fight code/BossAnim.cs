using UnityEngine;
using System.Collections;

public class BossAnimations : MonoBehaviour
{
    [System.Serializable]
    public class AnimationSync
    {
        public string mainTriggerName; // 메인 트리거 이름
        public string syncAnimationName; // 동기화할 애니메이션 이름
        public float delay; // 지연 시간
    }

    public Animator[] animatorsToSync; // 동기화할 애니메이터들
    public AnimationSync[] animationSyncs; // 트리거와 애니메이션 매핑 배열

    private Animator mainAnimator; // 메인 애니메이터

    void Start()
    {
        mainAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        foreach (var animationSync in animationSyncs)
        {
            // 메인 애니메이터에서 트리거가 활성화되었는지 확인
            if (mainAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationSync.mainTriggerName))
            {
                StartCoroutine(PlaySyncAnimationWithDelay(animationSync.syncAnimationName, animationSync.delay));
            }
        }
    }

    private IEnumerator PlaySyncAnimationWithDelay(string animationName, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // 다른 애니메이터들의 애니메이션을 실행
        foreach (Animator animator in animatorsToSync)
        {
            animator.Play(animationName);
        }
    }
}
