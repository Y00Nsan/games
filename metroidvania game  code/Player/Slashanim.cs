using System.Collections;
using UnityEngine;

public class Slashanim : MonoBehaviour
{
    private Animator anim;
    private PlayerMovement playerController;
    private bool isWallSliding;
    private bool isSlashing;
    private bool isFacingRight;

    public float slashDuration = 0.5f;
    public GameObject slashPrefab; // Prefab to spawn
    public float spawnDistance = 1f; // Distance to spawn the prefab from the central point
    public Transform centralPoint; // Central point around which the prefabs will be spawned

    private void Start()
    {
        anim = GetComponent<Animator>();
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            playerController = player.GetComponent<PlayerMovement>();
            if (playerController == null)
            {
                Debug.LogError("PlayerMovement script not found on the Player GameObject.");
            }
        }
        else
        {
            Debug.LogError("Player GameObject with tag 'Player' not found.");
        }

        if (centralPoint == null)
        {
            Debug.LogError("Central point Transform is not assigned.");
        }
    }

    private void Update()
    {
        if (playerController == null)
        {
            // Optionally, you could attempt to re-find the player here
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerController = player.GetComponent<PlayerMovement>();
            }
            if (playerController == null)
            {
                return; // Early exit if playerController is still null
            }
        }

        // Update wall sliding status and facing direction from playerController
        isWallSliding = playerController.isWallSliding;
        isFacingRight = playerController.isFacingRight;

        if (Input.GetKeyDown(KeyCode.F) && !isSlashing && !isWallSliding)
        {
            StartSlashing();
        }
    }

    private IEnumerator PerformDirectionalSlash()
    {
        isSlashing = true;

        string slashBool = "";
        Vector3 spawnPosition = centralPoint.position;
        Quaternion spawnRotation = Quaternion.identity;

        if (Input.GetKey(KeyCode.W)) 
        {
            slashBool = "SlashUp";
            spawnPosition += Vector3.up * spawnDistance;
            spawnRotation = Quaternion.Euler(0, 0, 90); // Rotate 90 degrees up
        }
        else if (Input.GetKey(KeyCode.S)) 
        {
            slashBool = "SlashDown";
            spawnPosition += Vector3.down * spawnDistance;
            spawnRotation = Quaternion.Euler(0, 0, -90); // Rotate 90 degrees down
        }
        else if (Input.GetKey(KeyCode.A)) 
        {
            slashBool = "SlashRight";
            spawnPosition += Vector3.left * spawnDistance;
            spawnRotation = Quaternion.Euler(0, 0, 180); // Rotate 180 degrees left
        }
        else if (Input.GetKey(KeyCode.D)) 
        {
            slashBool = "SlashRight";
            spawnPosition += Vector3.right * spawnDistance;
            spawnRotation = Quaternion.Euler(0, 0, 0); // No rotation needed for right
        }
        else 
        {
            slashBool = isFacingRight ? "SlashRight" : "SlashRight";
            if (isFacingRight)
            {
                spawnPosition += Vector3.right * spawnDistance;
                spawnRotation = Quaternion.Euler(0, 0, 0); // No rotation needed for right
            }
            else
            {
                spawnPosition += Vector3.left * spawnDistance;
                spawnRotation = Quaternion.Euler(0, 0, 180); // Rotate 180 degrees left
            }
        }

        if (!string.IsNullOrEmpty(slashBool))
        {
            anim.SetBool(slashBool, true);
        }

        // Instantiate the prefab at the calculated position with the calculated rotation
        Instantiate(slashPrefab, spawnPosition, spawnRotation);

        yield return new WaitForSeconds(slashDuration);

        if (!string.IsNullOrEmpty(slashBool))
        {
            anim.SetBool(slashBool, false);
        }

        isSlashing = false;
    }

    public void StartSlashing()
    {
        StartCoroutine(PerformDirectionalSlash());
    }
}
