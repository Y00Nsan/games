using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    public GameObject prefab; // 생성할 프리팹
    public float spawnInterval = 3f; // 생성 간격
    public Vector2 moveDirection = Vector2.right; // 이동 방향
    public float moveSpeed = 2f; // 이동 속도

    void Start()
    {
        // 일정 시간마다 프리팹을 생성
        InvokeRepeating("SpawnPrefab", 0f, spawnInterval);
    }

    void SpawnPrefab()
    {
        GameObject newPrefab = Instantiate(prefab, transform.position, Quaternion.identity);
        // 프리팹에 이동 및 삭제 스크립트 추가
        newPrefab.AddComponent<MoveAndDestroy>().Initialize(moveDirection, moveSpeed);
        AudioManger.Instance.PlaySFX("shot");
    }

    // 내부 클래스 정의
    private class MoveAndDestroy : MonoBehaviour
    {
        private Vector2 moveDirection;
        private float moveSpeed;

        public void Initialize(Vector2 direction, float speed)
        {
            moveDirection = direction;
            moveSpeed = speed;
            // 5초 후에 객체를 파괴
            Destroy(gameObject, 8f);
        }

        void Update()
        {
            // 프리팹 이동
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
        }
    }
}
