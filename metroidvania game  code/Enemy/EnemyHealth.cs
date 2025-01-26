using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        // ContinuousMovement 스크립트가 있을 때만 메서드를 호출
        ContinuousMovement continuousMovement = GetComponent<ContinuousMovement>();

        if (currentHealth > 0)
        {
            continuousMovement?.OnTakeDamage();
        }
        else
        {
            continuousMovement?.Die();
        }
    }
}
