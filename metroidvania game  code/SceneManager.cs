using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string targetScene;
    [SerializeField] Animator animator;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerPrefs.SetString("PreviousScene", SceneManager.GetActiveScene().name);
            StartCoroutine(LoadLevel());
        }
    }

    IEnumerator LoadLevel()
    {
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.SavePlayerData(); // Save player data including the animator state
        }

        animator.SetTrigger("FadeIn");
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(targetScene);

        // The PlayerHealth script will load the data in the new scene
        animator.SetTrigger("FadeOut");
    }
}
