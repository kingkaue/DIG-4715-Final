using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraGrayscaleShift : MonoBehaviour
{
    private GameObject gameManagerObject;
    private Coroutine changeColorCoroutine;
    [SerializeField] private VolumeProfile cameraVolumeProfile;
    private ColorAdjustments ca;
    private PlayerManager playerManager;

    void Start()
    {
        gameManagerObject = GameObject.FindGameObjectWithTag("GameController");
        cameraVolumeProfile.TryGet(out ca);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            playerManager = player.GetComponent<PlayerManager>();
        }
        else
        {
            Debug.Log("could not find player");
        }
    }

    void Update()
    {
        if (playerManager != null && ca != null)
        {
            float spiritPercent = playerManager.GetSpirit() / playerManager.GetMaxSpirit();
            ca.saturation.value = Mathf.Lerp(-100, 0f, spiritPercent);
        }
    }

    private IEnumerator GrayToColor()
    {
        float elapsedTime = 0f;
        float timeToChange = 2f;

        while (elapsedTime < timeToChange)
        {
            elapsedTime += Time.deltaTime;
            ca.saturation.value = Mathf.Lerp(-100, 0, elapsedTime / timeToChange);
            yield return null;
        }

        Debug.Log("Done Changing");
        changeColorCoroutine = null;
    }
}
