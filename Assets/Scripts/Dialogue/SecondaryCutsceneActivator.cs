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
    [SerializeField] private string idleAnimationState = "Idle";

    private bool hasTriggered;
    private Animator playerAnimator;
    private Collider triggerCollider;

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
        // Freeze player movement immediately
        player.FreezeMovement(true);

        // Start looking animation
        if (playerAnimator != null)
        {
            playerAnimator.Play(lookAnimationState, 0, 0f);
            playerAnimator.SetBool("IsLooking", true);

            // Start monitoring animation state in parallel
            StartCoroutine(MonitorAnimationState(player));
        }

        // Switch to cutscene camera
        SetCameraState(true);

        // Start dialogue
        yield return StartCoroutine(PlayDialogue(player));

        // Clean up
        ResetCutscene();
        player.FreezeMovement(false); // Restore movement

        // Ensure we switch back to player camera
        SetCameraState(false);
    }

    private IEnumerator MonitorAnimationState(PlayerMovement player)
    {
        // Wait until animation starts playing
        while (!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName(lookAnimationState))
        {
            yield return null;
        }

        // Get the normalized time of the animation
        float animLength = playerAnimator.GetCurrentAnimatorStateInfo(0).length;
        float timer = 0f;

        // Wait for animation to complete
        while (timer < animLength)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Return to idle when animation finishes
        playerAnimator.Play(idleAnimationState, 0, 0f);
        playerAnimator.SetBool("IsLooking", false);
    }

    private IEnumerator PlayDialogue(PlayerMovement player)
    {
        player.DialogueUI.ShowDialogue(introDialogue);
        player.DialogueUI.SetCutsceneCameras(cutsceneCamera, playerCamera);
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