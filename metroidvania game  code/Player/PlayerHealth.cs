using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 9;
    public int currentHealth;
    public Animator animator; // Assign your animator with the health triggers
    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;
    private PlayerMovement playerMovement;
    public int maxRecoveries = 3; // Maximum number of times recovery can be used
    public int recoveryCount = 0; // To keep track of the number of times recovery is used
    private bool canRecover = true; // To track if recovery is allowed

    public TMP_Text recoveryText; // Reference to the TMP Text component

    private void Start()
{
    LoadPlayerData(); // Load data when the scene starts

    spriteRenderer = GetComponent<SpriteRenderer>();
    playerMovement = GetComponent<PlayerMovement>();

    if (playerMovement == null)
    {
        Debug.LogError("PlayerMovement component not found on the player!");
    }

    if (spriteRenderer == null)
    {
        Debug.LogError("SpriteRenderer component not found on the player!");
    }
    if (animator == null)
    {
        Debug.LogError("Animator not assigned in the HealthManager!");
    }

    if (recoveryText == null)
    {
        Debug.LogError("TMP_Text component not assigned in the HealthManager!");
    }

    UpdateRecoveryText();
}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canRecover && recoveryCount < maxRecoveries && currentHealth < maxHealth)
        {
            RecoverHealth();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            TakeDamage(1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
{
    if (isInvincible) return;

    currentHealth -= damage;

    if (currentHealth > 0)
    {
        SetDamageTrigger(currentHealth); // Set the corresponding trigger based on current health
        StartCoroutine(HandleDamageEffects());
    }
    else
    {
        Die();
    }
}
    private IEnumerator HandleDamageEffects()
    {
        isInvincible = true;
        float invincibilityDuration = 1f;
        float blinkInterval = 0.2f;

        for (float i = 0; i < invincibilityDuration; i += blinkInterval)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);
            yield return new WaitForSeconds(blinkInterval / 2);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
            yield return new WaitForSeconds(blinkInterval / 2);
        }

        isInvincible = false;
    }

    private void Die()
    {
        Debug.Log("Player died!");
        playerMovement.isAlive = false; // Set isAlive to false when the player dies
    }

    private void SetDamageTrigger(int health)
{
    string triggerName = (maxHealth - health).ToString(); // Calculate the trigger name based on the health lost
    animator.SetTrigger(triggerName); // Set the trigger
}

    private void RecoverHealth()
    {
        currentHealth = maxHealth;
        recoveryCount++; // Increment recovery count
        canRecover = false; // Prevent further recovery for 2 seconds
        StartCoroutine(RecoveryCooldown());
        Debug.Log("Health fully recovered!");
        animator.SetTrigger("H"); // Trigger the "H" animation

        UpdateRecoveryText(); // Update the recovery text after recovering
    }

    private IEnumerator RecoveryCooldown()
    {
        yield return new WaitForSeconds(2f);
        if (recoveryCount < maxRecoveries)
        {
            canRecover = true; // Allow recovery again after 2 seconds if recovery limit is not reached
        }
    }

    private void OnDestroy()
    {
        if (animator != null)
        {
            SavePlayerData(); // Only save data if the animator still exists
        }
    }

    public void SavePlayerData()
    {
        PlayerPrefs.SetInt("PlayerHealth", currentHealth);
        PlayerPrefs.SetInt("RecoveryCount", recoveryCount);

        if (animator != null) // Check if the animator still exists
        {
            PlayerPrefs.SetFloat("AnimatorStateTime", animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            PlayerPrefs.SetInt("AnimatorStateHash", animator.GetCurrentAnimatorStateInfo(0).fullPathHash);
        }

        PlayerPrefs.Save(); // Ensure the data is saved
    }

    private void LoadPlayerData()
{
    if (PlayerPrefs.HasKey("PlayerHealth"))
    {
        currentHealth = PlayerPrefs.GetInt("PlayerHealth");
    }
    else
    {
        currentHealth = maxHealth;
    }

    if (PlayerPrefs.HasKey("RecoveryCount"))
    {
        recoveryCount = PlayerPrefs.GetInt("RecoveryCount");
    }

    // Load and set the animator stateif (PlayerPrefs.HasKey("AnimatorStateHash") && PlayerPrefs.HasKey("AnimatorStateTime"))
    {
        int stateHash = PlayerPrefs.GetInt("AnimatorStateHash");
        float stateTime = PlayerPrefs.GetFloat("AnimatorStateTime");
        animator.Play(stateHash, 0, stateTime); // 현재 상태에서 애니메이션이 이어지도록 설정
    }

    UpdateRecoveryText(); // Update the UI with the loaded values
}

    private void UpdateRecoveryText()
    {
        recoveryText.text = $"X{maxRecoveries - recoveryCount}";
    }
}