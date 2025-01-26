using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class result : MonoBehaviour
{
    public GameObject titles;
    
    public void Lose()
    {
        titles.SetActive(true);
    }
}
