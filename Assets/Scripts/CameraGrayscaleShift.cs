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

    void Start()
    {
        gameManagerObject = GameObject.FindGameObjectWithTag("GameController");
        cameraVolumeProfile.TryGet(out ca);
    }

    void Update()
    {
        if (gameManagerObject == null)
        {
            //Debug.Log("Game Manager not found");
        }
        else if (gameManagerObject.GetComponent<GameManager>().inColor == true)
        {
            Debug.Log("Changing Color");
            changeColorCoroutine = StartCoroutine(GrayToColor());
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
