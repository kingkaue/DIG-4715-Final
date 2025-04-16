using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
public class tutorialcutscene : MonoBehaviour
{
    [SerializeField] private Scene currentScene;
    private GameObject therapySceneManager;
    public bool inTutorial = false;
    private ObjectGrabable objectgrabable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        objectgrabable = GetComponent<ObjectGrabable>();
        currentScene = SceneManager.GetActiveScene();
    }

    public void OnUse(InputAction.CallbackContext context)
    {
        if (objectgrabable.isGrabbed == true)
        {
            if (currentScene.name == "Therapy Scene")
            {
                if (!inTutorial)
                {
                    StartCoroutine(LoadTutorialScene());
                    inTutorial = true;
                }
            }
            else if (currentScene.name == "tutorial")
            {
                inTutorial = true;
                Debug.Log("In Tutorial");
                if (inTutorial)
                {
                    Debug.Log("Going back to therapy");
                    StartCoroutine(ReturnToTherapy());
                    inTutorial = false;
                }
            }
        }
    }

    private IEnumerator LoadTutorialScene()
    {
        // Load the new scene additively
        yield return SceneManager.LoadSceneAsync("tutorial", LoadSceneMode.Additive);
        therapySceneManager = GameObject.FindGameObjectWithTag("TherapyManager");
        therapySceneManager.GetComponent<TutorialInputManager>().inTutorial = true;

        Scene newScene = SceneManager.GetSceneByName("tutorial");
          
        SceneManager.SetActiveScene(newScene);
    }

    private IEnumerator ReturnToTherapy()
    {
        Scene therapyScene = SceneManager.GetSceneByName("Therapy Scene");

        // Loads in hub scene if not already loaded in
        if (!therapyScene.IsValid())
        {
            Debug.LogError("Therapy scene not found!");
            yield break;
        }

        // Unload current scene if it's not the hub
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == "tutorial")
        {
            Debug.Log("Unloading Scene");
            yield return SceneManager.UnloadSceneAsync(currentScene);
        }

        SceneManager.SetActiveScene(therapyScene);
        therapySceneManager = GameObject.FindGameObjectWithTag("TherapyManager");
        therapySceneManager.GetComponent<TutorialInputManager>().inTutorial = false;
    }
}
