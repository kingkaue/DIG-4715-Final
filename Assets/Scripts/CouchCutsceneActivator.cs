using UnityEngine;
using System.Collections;

public class CouchCutsceneActivator : MonoBehaviour
{
    [Header("Camera Setup")]
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject couchCloseupCamera;
    [SerializeField] private GameObject roomPanCamera;
    [SerializeField] private float cameraPanDuration = 3f;

    [Header("Animation")]
    [SerializeField] private string layDownAnimation = "PlayerLayingDown";
    [SerializeField] private Transform couchPosition;

    [Header("Dialogue")]
    [SerializeField] private DialogueObject initialDialogue;
    [SerializeField] private DialogueObject panDialogue;
    [SerializeField] private float postPanDelay = 1f;

    private bool isInteractable = true;
    private Animator playerAnimator;
    private PlayerMovement playerMovement;
    private Collider triggerCollider;

    private void Update()
    {
        if (isInteractable && Input.GetKeyDown(KeyCode.L))
        {
            CheckPlayerProximity();
        }
    }

    


    private void Start()
    {
        triggerCollider = GetComponent<Collider>();

    }
    private void CheckPlayerProximity()
    {
        Collider[] nearbyPlayers = Physics.OverlapSphere(transform.position, 2f, LayerMask.GetMask("Player"));
        if (nearbyPlayers.Length > 0)
        {
            GameObject player = nearbyPlayers[0].gameObject;
            playerMovement = player.GetComponent<PlayerMovement>();
            playerAnimator = player.GetComponent<Animator>();

            if (playerMovement != null && playerAnimator != null)
            {
                StartCutscene();
            }
        }
    }

    private void StartCutscene()
    {
        isInteractable = false;
        playerMovement.FreezeMovement(true);
        StartCoroutine(RunCutsceneSequence());
    }

    private IEnumerator RunCutsceneSequence()
    {
        // Immediately position player on couch
        playerMovement.transform.position = couchPosition.position;
        playerMovement.transform.rotation = couchPosition.rotation;

        // Play lay down animation and lock state
        playerAnimator.Play(layDownAnimation, 0, 0f);
        playerAnimator.SetBool("IsLayingDown", true);

        // Switch to closeup camera
        SetCameraState(CameraState.Closeup);

        // Play initial dialogue
        yield return StartCoroutine(PlayDialogue(initialDialogue));

        // Pan camera to room view
        yield return StartCoroutine(PanCameraToRoomView());

        // Play second dialogue
        yield return new WaitForSeconds(postPanDelay);
        yield return StartCoroutine(PlayDialogue(panDialogue));

        // Clean up animation state
        playerAnimator.SetBool("IsLayingDown", false);

        // Return to player camera before ending
        SetCameraState(CameraState.Player);
        EndCutscene();
    }

    private IEnumerator PanCameraToRoomView()
    {
        SetCameraState(CameraState.Panning);
        yield return new WaitForSeconds(cameraPanDuration);
        SetCameraState(CameraState.RoomView);
    }

    private IEnumerator PlayDialogue(DialogueObject dialogue)
    {
        playerMovement.DialogueUI.ShowDialogue(dialogue);
        yield return new WaitWhile(() => playerMovement.DialogueUI.IsOpen);
    }

    private void SetCameraState(CameraState state)
    {
        playerCamera.SetActive(state == CameraState.Player);
        couchCloseupCamera.SetActive(state == CameraState.Closeup);
        roomPanCamera.SetActive(state == CameraState.Panning || state == CameraState.RoomView);
    }

    private void EndCutscene()
    {
        playerMovement.FreezeMovement(false);
        isInteractable = false;

        // Final safeguard to ensure player camera is active
        if (!playerCamera.activeSelf)
        {
            SetCameraState(CameraState.Player);
        }

        if (triggerCollider != null)
        {
            triggerCollider.enabled = false;
        }

    }

    private enum CameraState
    {
        Player,
        Closeup,
        Panning,
        RoomView
    }
}