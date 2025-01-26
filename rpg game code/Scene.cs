using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scene : MonoBehaviour
{
    public int index = 0;
    public void rightLoadScene()
    {
        SceneManager.LoadScene(index + 1);
    }

    public void leftLoadScene()
    {
        SceneManager.LoadScene(index - 1);
    }
}
