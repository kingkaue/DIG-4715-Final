using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CouchCutsceneActivator2 : MonoBehaviour
{
    [Header("Camera Setup")]
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject couchCloseupCamera;
    [SerializeField] private GameObject roomPanCamera;
    [SerializeField] private float cameraPanDuration = 3f;

    [Header("Animation")]
    [SerializeField] private string scaredAnimation = "playerscared";
    [SerializeField] private Transform couchPosition;

    [Header("Dialogue")]
    [SerializeField] private DialogueObject initialDialogue;
    [SerializeField] private DialogueObject panDialogue;
    [SerializeField] private float postPanDelay = 1f;

    [Header("Headset Spawn")]
    [SerializeField] private Transform headsetPosition;
    [SerializeField] private GameObject headsetPrefab;
    [SerializeField] private Transform holdPosition;

    [Header("Cutscene Tracking")]
    [SerializeField] private string cutsceneID = "CouchCutscene";

    [Header("Debug")]
    [SerializeField] private bool debugMode = true;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    // State variables
    private bool isInteractable = true;
    private bool playerInRange = false;
    private Animator playerAnimator;
    private PlayerMovement playerMovement;
    private Collider triggerCollider;
    private GameManager gameManager;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();

        try
        {
            gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
            if (gameManager == null) Debug.LogError("GameManager not found!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GameManager error: {e.Message}");
        }

        if (GameManager.Instance != null &&
            GameManager.Instance.HasCutscenePlayed(SceneManager.GetActiveScene().name, cutsceneID))
        {
            DisableCutsceneTrigger();
            SpawnHeadset();
            if (debugMode) Debug.Log("Cutscene already played - headset spawned");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (debugMode) Debug.Log("Player entered trigger");
            CheckPlayerProximity();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (debugMode) Debug.Log("Player exited trigger");
        }
    }

    private void Update()
    {
        if (isInteractable && playerInRange && Input.GetKeyDown(interactKey))
        {
            if (debugMode) Debug.Log($"{interactKey} pressed near couch");
            CheckPlayerProximity();
        }
    }

    private void CheckPlayerProximity()
    {
        // More reliable player detection
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            if (debugMode) Debug.LogError("No player found with tag!");
            return;
        }

        if (!player.TryGetComponent(out playerMovement))
        {
            if (debugMode) Debug.LogError("PlayerMovement component missing!");
            return;
        }

        if (!player.TryGetComponent(out playerAnimator))
        {
            if (debugMode) Debug.LogError("Animator component missing!");
            return;
        }

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance > 2f)
        {
            if (debugMode) Debug.Log($"Player too far: {distance}m");
            return;
        }

        if (debugMode) Debug.Log("All checks passed - starting cutscene");
        StartCutscene();
    }

    private void StartCutscene()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.MarkCutscenePlayed(SceneManager.GetActiveScene().name, cutsceneID);
        }

        isInteractable = false;
        StartCoroutine(RunCutsceneSequence());
    }

    private IEnumerator RunCutsceneSequence()
    {
        if (debugMode) Debug.Log("Cutscene sequence started");

        // Freeze player
        playerMovement.FreezeMovement(true);

        // Position player
        playerMovement.transform.SetPositionAndRotation(
            couchPosition.position,
            couchPosition.rotation
        );

        // Animation
        playerAnimator.Play(scaredAnimation);
        playerAnimator.SetBool("IsScared", true);

        // Camera sequence
        SetCameraState(CameraState.Closeup);
        yield return StartCoroutine(PlayDialogue(initialDialogue));

        SetCameraState(CameraState.Panning);
        yield return new WaitForSeconds(cameraPanDuration);

        SetCameraState(CameraState.RoomView);
        yield return new WaitForSeconds(postPanDelay);
        yield return StartCoroutine(PlayDialogue(panDialogue));

        // Cleanup
        playerAnimator.SetBool("IsScared", false);
        EndCutscene();
    }

    private void EndCutscene()
    {
        SetCameraState(CameraState.Player);
        playerMovement.FreezeMovement(false);
        DisableCutsceneTrigger();
        SpawnHeadset();

        if (gameManager != null)
        {
            gameManager.inbugscene = true;
        }

        if (debugMode) Debug.Log("Cutscene completed");
    }

    private void SpawnHeadset()
    {
        if (headsetPrefab == null)
        {
            Debug.LogError("Headset prefab not assigned!");
            return;
        }

        if (headsetPosition == null)
        {
            Debug.LogError("Headset spawn position not assigned!");
            return;
        }

        Instantiate(headsetPrefab, holdPosition.position, holdPosition.rotation, holdPosition);


        if (debugMode) Debug.Log($"Headset spawned at {headsetPosition.position}");
    }

    private void DisableCutsceneTrigger()
    {
        isInteractable = false;
        if (triggerCollider != null) triggerCollider.enabled = false;
    }

    private IEnumerator PlayDialogue(DialogueObject dialogue)
    {
        if (playerMovement.DialogueUI == null)
        {
            Debug.LogError("DialogueUI reference missing!");
            yield break;
        }

        playerMovement.DialogueUI.ShowDialogue(dialogue);
        yield return new WaitWhile(() => playerMovement.DialogueUI.IsOpen);
    }

    private void SetCameraState(CameraState state)
    {
        if (debugMode) Debug.Log($"Switching to camera: {state}");

        if (playerCamera != null) playerCamera.SetActive(state == CameraState.Player);
        if (couchCloseupCamera != null) couchCloseupCamera.SetActive(state == CameraState.Closeup);
        if (roomPanCamera != null) roomPanCamera.SetActive(state == CameraState.Panning || state == CameraState.RoomView);
    }

    private enum CameraState
    {
        Player,
        Closeup,
        Panning,
        RoomView
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 2f);

        if (couchPosition != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(couchPosition.position, Vector3.one * 0.5f);
        }
    }
}