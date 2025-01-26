using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gamemanager : MonoBehaviour
{
    public bool isLive;
    public result uiResult;
    public static gamemanager instance;

    void Awake()
    {
        instance = this;
    }
    
    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }


    IEnumerator GameOverRoutine()
    {
        isLive = false;

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
        Stop();
    }

    public void GameStart(int id)
    {
        Resume();
    }

    void Update() 
    {
        if (!isLive)
        return;
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
    }
}
