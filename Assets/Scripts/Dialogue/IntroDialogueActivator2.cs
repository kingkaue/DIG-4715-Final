using UnityEngine;
using System.Collections;

public class IntroDialogueActivator2 : MonoBehaviour
{
    [Header("Camera Control")]
    [SerializeField] private GameObject cutsceneCamera;
    [SerializeField] private GameObject playerCamera;

    [Header("Dialogue")]
    [SerializeField] private DialogueObject introDialogue;
    private bool hasTriggered;
    private Animator playerAnimator;
    private Collider triggerCollider;

    //Therapist animation properties
    public Animator therapistanimator;
    [SerializeField] private GameObject therapistObject;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        StartCoroutine(ForceSittingNextFrame());
        therapistanimator = therapistObject.GetComponent<Animator>();
    }

    private IEnumerator ForceSittingNextFrame()
    {
        // Wait one frame to ensure all objects are loaded
        yield return null;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerAnimator = player.GetComponent<Animator>();
            if (playerAnimator != null)
            {
                // Directly play the sitting animation state
                playerAnimator.Play("playersurprised", 0, 0f);
                Debug.Log("Forced sitting pose immediately");
                therapistanimator.Play("THERAPISTTALKING", 0, 0f);
                therapistanimator.SetBool("IsTalking", true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out PlayerMovement player))
            {
                // Ensure we have the animator reference
                if (playerAnimator == null)
                {
                    playerAnimator = other.GetComponent<Animator>();
                }

                // Switch to cutscene camera
                SwitchCameras(true);
                StartCoroutine(StartDialogueAfterDelay(player));
                hasTriggered = true;
            }
        }
    }

    private IEnumerator StartDialogueAfterDelay(PlayerMovement player)
    {
        yield return null; // Wait one frame

        // Show dialogue and pass camera references
        player.DialogueUI.ShowDialogue(introDialogue);
        player.DialogueUI.SetCutsceneCameras(cutsceneCamera, playerCamera);

        // Wait for dialogue to finish
        yield return new WaitWhile(() => player.DialogueUI.IsOpen);

        // Switch back to player camera when dialogue ends
        SwitchCameras(false);

        // Reset animation states
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsWalking", false);
        }

        if (therapistanimator != null)
        {
            therapistanimator.SetBool("IsSurprised", false);
        }

        // Disable the trigger collider
        if (triggerCollider != null)
        {
            triggerCollider.enabled = false;
        }
    }

    private void SwitchCameras(bool toCutscene)
    {
        if (cutsceneCamera != null)
        {
            cutsceneCamera.SetActive(toCutscene);
            Debug.Log($"Cutscene camera {(toCutscene ? "activated" : "deactivated")}");
        }

        if (playerCamera != null)
        {
            playerCamera.SetActive(!toCutscene);
            Debug.Log($"Player camera {(!toCutscene ? "activated" : "deactivated")}");
        }

        if (cutsceneCamera == null || playerCamera == null)
        {
            Debug.LogWarning("Camera references not set in inspector!");
        }
    }
}