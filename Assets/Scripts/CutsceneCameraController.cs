using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CutsceneCameraController : MonoBehaviour
{
    private void Awake()
    {
        // Ensure the tag is set correctly
        if (!gameObject.CompareTag("CutsceneCamera"))
        {
            gameObject.tag = "CutsceneCamera";
            Debug.LogWarning($"Cutscene camera {name} was not tagged correctly. Fixed.");
        }

        // Automatically disable on start if not in cutscene
        gameObject.SetActive(false);
    }

    // Call this when starting a cutscene
    public void ActivateForCutscene()
    {
        gameObject.SetActive(true);

        // Optional: Find and disable player camera
        GameObject playerCam = GameObject.FindGameObjectWithTag("MainCamera");
        if (playerCam != null) playerCam.SetActive(false);
    }

    // Call this when ending a cutscene
    public void DeactivateCutscene()
    {
        gameObject.SetActive(false);

        // Optional: Re-enable player camera
        GameObject playerCam = GameObject.FindGameObjectWithTag("MainCamera");
        if (playerCam != null) playerCam.SetActive(true);
    }
}