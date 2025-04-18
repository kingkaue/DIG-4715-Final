using UnityEngine;

public class CutSceneTrigger : MonoBehaviour
{
    public CutSceneManager cutsceneManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && cutsceneManager != null)
        {
            cutsceneManager.StartCutscene();
        }
    }
}