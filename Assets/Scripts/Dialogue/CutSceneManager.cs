using UnityEngine;
using System.Collections;

public class CutSceneManager : MonoBehaviour
{
    [Header("Cameras")]
    public GameObject mainCameraObject; // Reference to the main camera GameObject
    public GameObject cutsceneCameraObject; // Reference to the cutscene camera GameObject

    [Header("Cutscene Settings")]
    public float cutsceneDuration = 5f; // Duration of the cutscene

    private Camera mainCamera;
    private Camera cutsceneCamera;
    private AudioListener mainAudioListener;
    private AudioListener cutsceneAudioListener;

    private void Awake()
    {
        // Ensure cameras are active before getting components
        mainCameraObject.SetActive(true);
        cutsceneCameraObject.SetActive(false);

        // Get components
        mainCamera = mainCameraObject.GetComponent<Camera>();
        cutsceneCamera = cutsceneCameraObject.GetComponent<Camera>();
        mainAudioListener = mainCameraObject.GetComponent<AudioListener>();
        cutsceneAudioListener = cutsceneCameraObject.GetComponent<AudioListener>();

        // Set initial state
        mainCamera.enabled = true;
        mainAudioListener.enabled = true;
        cutsceneCamera.enabled = false;
        cutsceneAudioListener.enabled = false;
    }

    // Start the cutscene
    public void StartCutscene()
    {
        StartCoroutine(PlayCutscene());
    }

    // Coroutine to handle the cutscene
    private IEnumerator PlayCutscene()
    {
        // Activate cutscene camera object
        cutsceneCameraObject.SetActive(true);

        // Switch to the cutscene camera
        mainCamera.enabled = false;
        mainAudioListener.enabled = false;

        cutsceneCamera.enabled = true;
        cutsceneAudioListener.enabled = true;

        // Perform cutscene actions
        Debug.Log("Cutscene started!");

        // Wait for the cutscene duration
        yield return new WaitForSeconds(cutsceneDuration);

        // Switch back to the main camera
        cutsceneCamera.enabled = false;
        cutsceneAudioListener.enabled = false;

        mainCamera.enabled = true;
        mainAudioListener.enabled = true;

        // Deactivate cutscene camera object if needed
        cutsceneCameraObject.SetActive(false);

        Debug.Log("Cutscene ended!");
    }
}