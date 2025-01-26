using UnityEngine;

public class SlashEffect : MonoBehaviour
{
    public float damage = 2.0f; // Changed from int to float
    public float upwardForce = 10f;
    public float bulletReflectForce = 5f;

    private PlayerMovement playerController;
    private Rigidbody2D playerRigidbody;

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<PlayerMovement>();
            playerRigidbody = player.GetComponent<Rigidbody2D>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(Mathf.RoundToInt(damage)); // Convert float to int

                // Apply upward force to the player if the attack was downward
                if (Input.GetKey(KeyCode.S))
                {
                    playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 0); // Reset current vertical velocity
                    playerRigidbody.AddForce(Vector2.up * upwardForce, ForceMode2D.Impulse);
                }
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            Rigidbody2D bulletRigidbody = collision.GetComponent<Rigidbody2D>();
            if (bulletRigidbody != null)
            {
                EnemyBullet bullet = collision.GetComponent<EnemyBullet>();
                if (bullet != null)
                {
                    bullet.isReflected = true;
                }

                Vector2 reflectDirection = (bulletRigidbody.transform.position - playerController.transform.position).normalized;
                bulletRigidbody.velocity = reflectDirection * bulletReflectForce;
            }
        }
    }
}
