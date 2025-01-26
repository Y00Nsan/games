using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] public float initialHealth = 100f;
    [SerializeField] public float healthThreshold75 = 75f; // 피가 75일 때
    [SerializeField] public float healthThreshold50 = 50f; // 피가 50일 때
    // [SerializeField] public float healthThreshold25 = 25f; // 피가 25일 때

    [SerializeField] public float spawnHealth75 = 50f; // 75 피일 때 생성될 적의 체력
    [SerializeField] public float spawnHealth50 = 25f; // 50 피일 때 생성될 적의 체력
    // [SerializeField] public float spawnHealth25 = 10f; // 25 피일 때 생성될 적의 체력

    public  float health;
    Rigidbody2D rb;
    private Animator anim;
    public GameObject bloodEffect;
    public GameObject enemyPrefab; // 새로운 적을 복제하기 위한 프리팹

    private bool hasSpawnedAt75 = false;
    private bool hasSpawnedAt50 = false;
    // private bool hasSpawnedAt25 = false;
    private bool isClone = false; // 복제 여부를 확인하는 변수

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();   
        health = initialHealth; // 초기 피를 첫 번째 피로 설정

    }

    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void EnemyHit(float damage)
    {
        Instantiate(bloodEffect, transform.position, Quaternion.identity);
        health -= damage;

        if (!isClone)
        {
            // 피가 75 이하일 때 (한 번만 복제)
            if (health <= healthThreshold75 && !hasSpawnedAt75)
            {
                hasSpawnedAt75 = true;
                SpawnEnemy(spawnHealth75); // 지정된 체력을 복제본에 전달
            }
            // 피가 50 이하일 때 (한 번만 복제)
            else if (health <= healthThreshold50 && !hasSpawnedAt50)
            {
                hasSpawnedAt50 = true;
                SpawnEnemy(spawnHealth50); // 지정된 체력을 복제본에 전달
            }
            // 피가 25 이하일 때 (한 번만 복제)
            // else if (health <= healthThreshold25 && !hasSpawnedAt25)
            // {
            //     hasSpawnedAt25 = true;
            //     SpawnEnemy(spawnHealth25); // 지정된 체력을 복제본에 전달
            // }
        }
    }

    // 체력을 전달하여 적을 생성하는 메서드
    private void SpawnEnemy(float spawnHealth)
    {
        GameObject clone = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        Enemy enemyScript = clone.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.initialHealth = spawnHealth; // 복제본의 초기 체력을 설정
            enemyScript.health = spawnHealth; // 현재 체력도 설정
            enemyScript.isClone = true; // 복제된 적으로 표시
        }
    }

    // 체력을 설정하는 메서드
    public void SetHealth(float newHealth)
    {
        health = newHealth;
    }
}
