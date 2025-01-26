using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class CameraManger : MonoBehaviour
{
    public static CameraManger instance;

    [SerializeField] private CinemachineVirtualCamera[] allVirtualCameras;

    [Header("Controls for lerping the Y Dampping during player jump/fall")]
    [SerializeField] private float fallPanAmount = 0.25f;
    [SerializeField] private float fallYPanTime = 0.35f;
    public float fallSpeedYDampingChangeThreshold = -15f;

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFrontLayerFalling { get; set; }

    private Coroutine lerpYPanCoroutine;
    private Coroutine panCameraCoroutine;

    private CinemachineVirtualCamera currentCamera;
    private CinemachineFramingTransposer framingTransposer;

    private float normYPanAmount;

    private Vector2 startingTrackerObjectsOffset;

    private void Awake() 
    {
        if(instance == null)
        {
            instance = this;
        }
        for(int i = 0; i < allVirtualCameras.Length; i++)
        {
            if(allVirtualCameras[i].enabled)
            {
                currentCamera = allVirtualCameras[i];

                framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
        }

        normYPanAmount = framingTransposer.m_YDamping;

        startingTrackerObjectsOffset = framingTransposer.m_TrackedObjectOffset;
    }

    #region Pan Camera

    public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        panCameraCoroutine = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPos));
    }

    private IEnumerator PanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        Vector2 endPos = Vector2.zero;
        Vector2 StartingPos = Vector2.zero;

        if(!panToStartingPos)
        {
            switch(panDirection)
            {
                case PanDirection.Up:
                    endPos = Vector2.up;
                    break;
                case PanDirection.Down:
                    endPos = Vector2.down;
                    break;
                case PanDirection.Left:
                    endPos = Vector2.right;
                    break;
                case PanDirection.Right:
                    endPos = Vector2.left;
                    break;
                default:
                    break;
            }

            endPos *= panDistance;
            StartingPos = startingTrackerObjectsOffset;
            endPos += StartingPos;

        }
        else
        {
            StartingPos = framingTransposer.m_TrackedObjectOffset;
            endPos = startingTrackerObjectsOffset;
        }

        float elapsedTime = 0f;
        while(elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;

            Vector3 panLerp = Vector3.Lerp(StartingPos, endPos, (elapsedTime / panTime));
            framingTransposer.m_TrackedObjectOffset = panLerp;

            yield return null;
        }
    }

    #endregion

    #region Lerp the Y Damping

    public void LerpYDamping(bool isPlayerFalling)
    {
        lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        float startDampAmount = framingTransposer.m_YDamping;
        float endDampAmount = 0f;

        if(isPlayerFalling)
        {
            endDampAmount = fallPanAmount;
            LerpedFrontLayerFalling = true;
        }
        else
        {
            endDampAmount = normYPanAmount;
        }

        float elapsedTime = 0f;
        while(elapsedTime < fallYPanTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, (elapsedTime / fallYPanTime));
            framingTransposer.m_YDamping = lerpedPanAmount;

            yield return null;
        }

        IsLerpingYDamping = false;
    }

    #endregion

    #region Swap camera

    public void SwapCamera(CinemachineVirtualCamera cameraFromLeft, CinemachineVirtualCamera cameraFromRight, Vector2 triggerExitDirection)
    {
        if(currentCamera == cameraFromLeft && triggerExitDirection.x > 0f)
        {
            cameraFromRight.enabled = true;
            cameraFromLeft.enabled = false;
            currentCamera = cameraFromRight;
            framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
        else if(currentCamera == cameraFromRight && triggerExitDirection.x < 0f)
        {
            cameraFromLeft.enabled = true;
            cameraFromRight.enabled = false;
            currentCamera = cameraFromLeft;
            framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
    }

    #endregion
}
