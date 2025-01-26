using UnityEngine;

public class DestroyTogether : MonoBehaviour
{
    public GameObject otherObject;

    // 이 오브젝트가 파괴될 때 호출되는 메서드
    private void OnDestroy()
    {
        // otherObject가 null이 아니라면 파괴
        if (otherObject != null)
        {
            Destroy(otherObject);
        }
    }
}
