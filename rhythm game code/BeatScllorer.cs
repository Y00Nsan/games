using UnityEngine;

public class BeatScllorer : MonoBehaviour
{
    public float BeatTempo;

    public bool hasStarted;
    void Start()
    {
        BeatTempo = BeatTempo;
    }

    // Update is called once per frame
    void Update()
    {
        if(!hasStarted)
        {
            // if(Input.anyKeyDown)
            // {
            //     hasStarted = true;
            // }
        } else
        {
            transform.position -= new Vector3(0f, BeatTempo * Time.deltaTime, 0f);
        }
    }
}
