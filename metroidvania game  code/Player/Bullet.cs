using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 10;
    public float lifeTime = 2.0f;
    public float speed = 10f;

    void Start()
    {
        Destroy(gameObject, lifeTime); // 일정 시간 후 총알을 제거
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime); // 속도에 따라 총알 이동
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            Destroy(gameObject); // 총알이 적에게 맞으면 제거
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            Destroy(gameObject); // 총알이 환경과 충돌하면 제거
        }
    }
}
