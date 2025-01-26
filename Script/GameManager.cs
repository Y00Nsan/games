using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(ChangeSceneRoutine(sceneName));
    }

    private IEnumerator ChangeSceneRoutine(string sceneName)
    {
        // Fade out
        FadeManager.Instance.FadeOut(() =>
        {
            // Load the new scene
            SceneManager.LoadScene(sceneName);
        });

        // Wait for the scene to load
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == sceneName);

        // Fade in
        FadeManager.Instance.FadeIn();
    }
}
