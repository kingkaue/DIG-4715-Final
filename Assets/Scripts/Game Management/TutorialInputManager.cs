using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialInputManager : MonoBehaviour
{
    public bool inTutorial = false;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject cameraHandler;
    [SerializeField] private GameObject playerCam;

    void Update()
    {
        if (inTutorial)
        {
            player.SetActive(false);
            cameraHandler.SetActive(false);
            playerCam.SetActive(false);
        }
        else
        {
            player.SetActive(true);
            cameraHandler.SetActive(true);
            playerCam.SetActive(true);
        }
    }
}
