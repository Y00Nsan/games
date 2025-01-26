using System.Collections;
using UnityEngine;

public class ContinuousMovement : MonoBehaviour
{
    public Transform[] movement;
    public float moveSpeed = 2f;
    public float rotationSpeed = 50f;
    public float TakeDamageDuration = 0.5f;
    public GameObject deathPrefab;
    public float prefabDestroyDelay = 2f;
    private int currentTargetIndex = 0;
    private bool isTakingDamage = false;
    private Animator animator;

    private void Start()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the enemy!");
        }
    }

    private void Update()
    {
        if (movement.Length == 0 || isTakingDamage)
            return;

        Transform target = movement[currentTargetIndex];
        Vector3 direction = target.position - transform.position;

        float targetRotationAngle = GetRotationAngle(currentTargetIndex, (currentTargetIndex + 1) % movement.Length);
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetRotationAngle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentTargetIndex = (currentTargetIndex + 1) % movement.Length;
        }
    }

    public void OnTakeDamage()
    {
        if (!isTakingDamage)
        {
            StartCoroutine(TakeDamageCoroutine());
        }
    }

    private IEnumerator TakeDamageCoroutine()
    {
        isTakingDamage = true;
        animator.SetBool("TakeDamage", true);

        yield return new WaitForSeconds(TakeDamageDuration);

        animator.SetBool("TakeDamage", false);
        isTakingDamage = false;
    }

    public void Die()
    {
        animator.SetTrigger("Dead");
        // Instantiate deathPrefab with the same position and rotation as the enemy
        GameObject deathEffect = Instantiate(deathPrefab, transform.position, transform.rotation);
        Destroy(deathEffect, prefabDestroyDelay);
        Destroy(gameObject);
    }

    private float GetRotationAngle(int fromIndex, int toIndex)
    {
        if (fromIndex == 3 && toIndex == 0)
            return 0f;
        else if (fromIndex == 0 && toIndex == 1)
            return -90f;
        else if (fromIndex == 1 && toIndex == 2)
            return 180f;
        else if (fromIndex == 2 && toIndex == 3)
            return 90f;
        else
            return 0f;
    }
}
