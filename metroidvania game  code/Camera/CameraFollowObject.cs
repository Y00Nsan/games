using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform PlayerTransform;

    [Header("References")]
    [SerializeField] private float flipYRotationTime = 0.5f;

    private Coroutine turnCoroutine;

    private PlayerMovement player;

    private bool isFacingRight;

    private void Awake() {
        player = PlayerTransform.gameObject.GetComponent<PlayerMovement>();
        isFacingRight = player.isFacingRight;
    }

    private void Update() {
        transform.position = PlayerTransform.position;
    }

    public void CallTurn()
    {
        LeanTween.rotateY(gameObject, DetermineEndRotation(), flipYRotationTime).setEaseInOutSine();
    }

    private IEnumerator FlipYLerp()
    {
        float startRotation = transform.localEulerAngles.y;
        float endRotationAmount = DetermineEndRotation();
        float yRotation = 0f;

        float elapsedTime = 0f;
        while(elapsedTime < flipYRotationTime)
        {
            elapsedTime += Time.deltaTime;

            yRotation = Mathf.Lerp(startRotation, endRotationAmount, (elapsedTime / flipYRotationTime));
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

            yield return null;
        }
    }

    private float DetermineEndRotation()
    {
        isFacingRight = !isFacingRight;

        if(isFacingRight)
        {
            return 180f;
        }
        else
        {
            return 0f;
        }
    }
}
