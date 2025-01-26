using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public SceneSpawnPair[] sceneSpawnPairs; // Array to manage scenes and spawn positions

    private void Start()
    {

        // Check if the player came from a previous scene
        string savedPreviousScene = PlayerPrefs.GetString("PreviousScene", "");

        foreach (var pair in sceneSpawnPairs)
        {
            if (savedPreviousScene == pair.previousScene)
            {
                // Move the player to the corresponding spawn position
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null && pair.spawn != null)
                {
                    player.transform.position = pair.spawn.transform.position;
                }
                break; // Exit the loop once the correct spawn position is found
            }
        }
    }
}

[System.Serializable]
public class SceneSpawnPair
{
    public string previousScene; // Name of the previous scene
    public GameObject spawn;     // Spawn position corresponding to the scene
}
