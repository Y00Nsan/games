using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class singleLevel : MonoBehaviour
{
    public int levelIndex;
    public void BackButton()
    {
        SceneManager.LoadScene("Map");
    }

    // public void PressStarButton(int starNum)
    // {
    //     currentStarNum = starNum;
    //     if(currentStarNum > PlayerPrefs.GetInt("Lv" + levelIndex))
    //     {
    //         PlayerPrefs.SetInt("Lv" + levelIndex, starNum);
    //     }

    //     Debug.Log(PlayerPrefs.GetInt("Lv" + levelIndex, starNum));
    // }
}
