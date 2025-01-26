using System.Collections;
using UnityEngine;

public class WallSpawner : MonoBehaviour
{
    public GameObject prefab; // 생성할 prefab
    public Transform[] spawnPoints; // 여러 위치
    public float spawnInterval = 1.0f; // 각 위치에서 생성 간격 (초 단위)
    public float destroyTimeAfterLastSpawn = 2.0f; // 모든 생성 후 사라지는 시간 (초 단위)
    public float respawnInterval = 5.0f; // 모든 오브젝트가 사라진 후 다시 생성되는 시간 간격 (초 단위)

    void Start()
    {
        StartCoroutine(SpawnAndDestroyPrefabs());
    }

    IEnumerator SpawnAndDestroyPrefabs()
    {
        while (true)
        {
            GameObject[] spawnedObjects = new GameObject[spawnPoints.Length];

            // 각 위치에 순서대로 생성
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                spawnedObjects[i] = Instantiate(prefab, spawnPoints[i].position, spawnPoints[i].rotation);
                yield return new WaitForSeconds(spawnInterval);
            }

            // 모든 생성이 완료된 후 대기 시간
            yield return new WaitForSeconds(destroyTimeAfterLastSpawn);

            // 생성된 모든 오브젝트 파괴
            for (int i = 0; i < spawnedObjects.Length; i++)
            {
                if (spawnedObjects[i] != null)
                {
                    Destroy(spawnedObjects[i]);
                }
            }

            // 다시 생성하기 전 대기 시간
            yield return new WaitForSeconds(respawnInterval);
        }
    }
}
