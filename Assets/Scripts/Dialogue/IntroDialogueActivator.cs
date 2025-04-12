using UnityEngine;
using System.Collections;

public class IntroDialogueActivator : MonoBehaviour
{
    [Header("Camera Control")]
    [SerializeField] private GameObject cutsceneCamera;
    [SerializeField] private GameObject playerCamera;

    [Header("Dialogue")]
    [SerializeField] private DialogueObject introDialogue;
    private bool hasTriggered;
    private Animator playerAnimator;
    private Collider triggerCollider; // Store the collider reference

    private void Start()
    {
        // Cache the collider component at Start
        triggerCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out PlayerMovement player))
            {
                playerAnimator = other.GetComponent<Animator>();
                if (playerAnimator != null)
                {
                    playerAnimator.SetBool("IsSitting", true);
                }

                // Switch cameras
                if (cutsceneCamera != null) cutsceneCamera.SetActive(true);
                if (playerCamera != null) playerCamera.SetActive(false);

                StartCoroutine(StartDialogueAfterDelay(player));
                hasTriggered = true;
            }
        }
    }

    private IEnumerator StartDialogueAfterDelay(PlayerMovement player)
    {
        yield return null; // Wait one frame
        player.DialogueUI.ShowDialogue(introDialogue);
        player.DialogueUI.SetCutsceneCameras(cutsceneCamera, playerCamera);

        // Wait for dialogue to finish
        yield return new WaitWhile(() => player.DialogueUI.IsOpen);

        // Reset animation and disable collider (instead of destroying)
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsSitting", false);
        }

        if (triggerCollider != null)
        {
            triggerCollider.enabled = false; // Disable the collider
        }
    }
}