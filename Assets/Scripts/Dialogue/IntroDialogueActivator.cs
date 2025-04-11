using UnityEngine;

public class IntroDialogueActivator : MonoBehaviour
{
    [Header("Camera Control")]
    [SerializeField] private GameObject cutsceneCamera;
    [SerializeField] private GameObject playerCamera;

    [Header("Dialogue")]
    [SerializeField] private DialogueObject introDialogue;
    private bool hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out PlayerMovement player))
            {
                // Activate cutscene camera
                if (cutsceneCamera != null) cutsceneCamera.SetActive(true);
                if (playerCamera != null) playerCamera.SetActive(false);

                // Start dialogue
                player.DialogueUI.ShowDialogue(introDialogue);
                hasTriggered = true;

                // Pass camera references to DialogueUI
                player.DialogueUI.SetCutsceneCameras(cutsceneCamera, playerCamera);
            }
        }
    }

    public void DestroyTrigger()
    {
        Destroy(gameObject);
    }
}