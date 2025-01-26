using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public int damage = 10;
    public float lifetime = 3f;
    public bool isReflected = false; // 튕겨진 총알 여부

    void Start()
    {
        Destroy(gameObject, lifetime); // 총알이 3초 후 사라짐
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerHealth player = hitInfo.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Destroy(gameObject); // 총알이 플레이어와 충돌하면 사라짐
            }
        }
        else if (hitInfo.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (isReflected) // 총알이 튕겨진 경우에만 적에게 피해를 줌
            {
                EnemyHealth enemy = hitInfo.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    Destroy(gameObject); // 총알이 적과 충돌하면 사라짐
                }
            }
        }
        else if (hitInfo.gameObject.layer == LayerMask.NameToLayer("Wall")) // 벽과 충돌하면 총알이 파괴됨
        {
            Destroy(gameObject);
        }
    }
}
