using UnityEngine;
using System.Collections;
public class CutSceneManager : MonoBehaviour
{
    [Header("Cameras")]
    public Camera mainCamera; // Reference to the main camera
    public Camera cutsceneCamera; // Reference to the cutscene camera

    [Header("Cutscene Settings")]
    public float cutsceneDuration = 5f; // Duration of the cutscene

    // Start the cutscene
    public void StartCutscene()
    {
        StartCoroutine(PlayCutscene());
    }

    // Coroutine to handle the cutscene
    private IEnumerator PlayCutscene()
    {
        // Switch to the cutscene camera
        mainCamera.enabled = false;
        cutsceneCamera.enabled = true;

        // Perform cutscene actions (e.g., move objects, play animations)
        Debug.Log("Cutscene started!");

        // Wait for the cutscene duration
        yield return new WaitForSeconds(cutsceneDuration);

        // Switch back to the main camera
        cutsceneCamera.enabled = false;
        mainCamera.enabled = true;

        // Clean up (e.g., destroy the cutscene camera if needed)
        Debug.Log("Cutscene ended!");
    }
}
