using UnityEngine;
using System.Collections;

public class SecondaryCutsceneActivator : MonoBehaviour
{
    [Header("Camera Control")]
    [SerializeField] private GameObject cutsceneCamera;
    [SerializeField] private GameObject playerCamera;

    [Header("Dialogue")]
    [SerializeField] private DialogueObject introDialogue;
    [SerializeField] private string lookAnimationState = "PlayerLookingAround";
    [SerializeField] private string idleAnimationState = "idle";

    private bool hasTriggered;
    private Animator playerAnimator;
    private Collider triggerCollider;
    private bool wasPlayerCameraActive;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out PlayerMovement player))
            {
                hasTriggered = true;
                playerAnimator = other.GetComponent<Animator>();
                StartCoroutine(RunCutscene(player));
            }
        }
    }

    private IEnumerator RunCutscene(PlayerMovement player)
    {
        // Freeze controls immediately
        player.FreezeMovement(true);
        player.FreezeRotation(true);

        // Store initial states
        wasPlayerCameraActive = playerCamera.activeSelf;
        var charController = player.GetComponent<CharacterController>();
        bool hadRootMotion = false;

        // Disable character controller if exists
        if (charController != null)
        {
            charController.enabled = false;
        }

        // Handle animation
        if (playerAnimator != null)
        {
            hadRootMotion = playerAnimator.applyRootMotion;
            playerAnimator.applyRootMotion = false;
            playerAnimator.Play(lookAnimationState, 0, 0f);
            playerAnimator.SetBool("IsLooking", true);
            StartCoroutine(MonitorAnimationState(player));
        }

        // Switch cameras
        SetCameraState(true);

        // Start dialogue
        yield return StartCoroutine(PlayDialogue(player));

        // Clean up
        ResetCutscene();

        // Restore animation
        if (playerAnimator != null)
        {
            playerAnimator.applyRootMotion = hadRootMotion;
        }

        // Restore character controller
        if (charController != null)
        {
            charController.enabled = true;
        }

        // Restore controls
        player.FreezeMovement(false);
        player.FreezeRotation(false);

        // Restore original camera state
        if (wasPlayerCameraActive)
        {
            SetCameraState(false);
        }
    }

    private IEnumerator MonitorAnimationState(PlayerMovement player)
    {
        while (!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName(lookAnimationState))
        {
            yield return null;
        }

        float animLength = playerAnimator.GetCurrentAnimatorStateInfo(0).length;
        float timer = 0f;

        while (timer < animLength)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        playerAnimator.Play(idleAnimationState, 0, 0f);
        playerAnimator.SetBool("IsLooking", false);
    }

    private IEnumerator PlayDialogue(PlayerMovement player)
    {
        player.DialogueUI.ShowDialogue(introDialogue);
        player.DialogueUI.SetCameras(cutsceneCamera, playerCamera);
        yield return new WaitWhile(() => player.DialogueUI.IsOpen);
    }

    private void SetCameraState(bool cutsceneActive)
    {
        if (cutsceneCamera != null) cutsceneCamera.SetActive(cutsceneActive);
        if (playerCamera != null) playerCamera.SetActive(!cutsceneActive);
    }

    private void ResetCutscene()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsLooking", false);
        }
        triggerCollider.enabled = false;
    }
}