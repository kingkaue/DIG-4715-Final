using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GravestoneCutsceneActivator : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private GameObject _playerCamera;
    [SerializeField] private GameObject _closeupCamera; // Gravestone close-up
    [SerializeField] private GameObject _wideShotCamera; // Player + gravestone wide shot

    [Header("Player Animation")]
    [SerializeField] private string _kneelAnimation = "PlayerKneel"; // Must match Animator

    [Header("Effects/Dialogue")]
    [SerializeField] private DialogueObject _graveDialogue; // Optional
    [SerializeField] private ParticleSystem _poppyEffect; // Optional

    [Header("Scene Transition")]
    [SerializeField] private string _gameOverSceneName = "Game End Screen"; // Name of your game over scene
    [SerializeField] private float _delayBeforeGameOver = 3f; // Time after dialogue before transition

    private Animator _playerAnimator;
    private PlayerMovement _playerMovement;
    private bool _hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (!_hasTriggered && other.gameObject.name == "singlepoppy")
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
                if (playerMovement != null)
                {
                    Debug.Log("Cutscene started!");
                    _hasTriggered = true;
                    StartCutscene(playerMovement);
                }
                else
                {
                    Debug.LogError("PlayerMovement missing on Player!");
                }
            }
        }
    }

    private void StartCutscene(PlayerMovement player)
    {
        _playerMovement = player;
        _playerAnimator = player.GetComponent<Animator>();

        if (_playerAnimator == null)
        {
            Debug.LogError("Player Animator missing!");
            return;
        }

        // Freeze player but keep current position/rotation
        _playerMovement.FreezeMovement(true);

        // Start the cutscene
        StartCoroutine(CutsceneSequence());
    }

    private IEnumerator CutsceneSequence()
    {
        _playerAnimator.Play(_kneelAnimation, 0, 0f);
        // ---- PHASE 1: Close-up on gravestone ----
        SetCameraActive(_closeupCamera);
        // Play from start
        if (_poppyEffect != null) _poppyEffect.Play();

        yield return new WaitForSeconds(2.5f); // Dramatic pause

        // ---- PHASE 2: Wide shot with player kneeling ----
        SetCameraActive(_wideShotCamera);

        // Optional: Dialogue
        if (_graveDialogue != null && _playerMovement.DialogueUI != null)
        {
            _playerMovement.DialogueUI.ShowDialogue(_graveDialogue);
            yield return new WaitWhile(() => _playerMovement.DialogueUI.IsOpen);
        }

        // Wait additional time before game over
        yield return new WaitForSeconds(_delayBeforeGameOver);

        // Load the game over scene
        SceneManager.LoadScene(_gameOverSceneName);
    }

    private void SetCameraActive(GameObject activeCamera)
    {
        _playerCamera.SetActive(false);
        _closeupCamera.SetActive(false);
        _wideShotCamera.SetActive(false);
        activeCamera.SetActive(true);
    }

    // Optional: Manually trigger cutscene with T key (debugging)
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !_hasTriggered)
        {
            PlayerMovement player = FindObjectOfType<PlayerMovement>();
            if (player != null) StartCutscene(player);
        }
    }
}