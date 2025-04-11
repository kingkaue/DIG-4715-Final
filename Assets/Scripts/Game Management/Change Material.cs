using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;

public class ChangeMaterial : MonoBehaviour
{
    [Header ("Material")]
    private Renderer objectRenderer;
    private Material objectMaterial;
    [SerializeField] Texture2D objectTexture;

    private Coroutine changeMaterialCoroutine;
    private bool canChange = true;
    private Vector3 playerPosition;
    private GameObject gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        objectMaterial = objectRenderer.material;
        objectMaterial.SetTexture("_MainTex", objectTexture);
        gameManager = GameObject.FindGameObjectWithTag("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        float distanceFromPlayer = Vector3.Distance(playerPosition, transform.position);

        if (gameManager == null)
        {
            Debug.Log("Game Manager not found");
        }
        else if (gameManager.GetComponent<GameManager>().inColor == true)
        {
            Debug.Log("Changing Color");
            canChange = false;
            changeMaterialCoroutine = StartCoroutine(GrayToColor());
        }
    }

    private IEnumerator GrayToColor()
    {
        float elapsedTime = 0f;
        float timeToChange = 2f;

        while (elapsedTime < timeToChange)
        {
            elapsedTime += Time.deltaTime;
            objectMaterial.SetFloat("_saturation", Mathf.Lerp(0, 1, elapsedTime / timeToChange));
            yield return null;
        }
        Debug.Log("Done Changing");
        changeMaterialCoroutine = null;
    }

    private void ChangeColor()
    {
        Debug.Log("Changing Color");
        canChange = false;
        changeMaterialCoroutine = StartCoroutine(GrayToColor());
    }

}
