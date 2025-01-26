using System.Collections;
using UnityEngine;

public class PrefabSpawner1 : MonoBehaviour
{
    public GameObject prefab; // 생성할 프리팹
    public GameObject spawnObject; // 프리팹이 생성될 위치의 게임 오브젝트
    public GameObject targetObject; // 프리팹이 이동할 목표 위치의 게임 오브젝트
    public float spawnInterval = 2.0f; // 프리팹이 생성될 시간 간격
    public float moveSpeed = 5.0f; // 프리팹이 목표 위치로 이동할 속도

    void Start()
    {
        // 지정된 시간 간격으로 프리팹을 생성하는 코루틴 시작
        StartCoroutine(SpawnPrefab());
    }

    IEnumerator SpawnPrefab()
    {
        while (true)
        {
            // 생성 위치에서 프리팹 생성
            GameObject spawnedPrefab = Instantiate(prefab, spawnObject.transform.position, Quaternion.identity);
            // 프리팹을 목표 위치로 이동시키는 코루틴 시작
            StartCoroutine(MoveToTargetAndDestroy(spawnedPrefab));
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator MoveToTargetAndDestroy(GameObject spawnedPrefab)
    {
        while (Vector3.Distance(spawnedPrefab.transform.position, targetObject.transform.position) > 0.1f)
        {
            // 프리팹을 목표 위치로 이동
            spawnedPrefab.transform.position = Vector3.MoveTowards(spawnedPrefab.transform.position, targetObject.transform.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
        // 목표 위치에 도달하면 프리팹을 제거
        Destroy(spawnedPrefab);
    }
}
