using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class tutorialcutscene : MonoBehaviour
{
    [SerializeField] private Scene currentScene;

    private ObjectGrabable objectgrabable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        objectgrabable = GetComponent<ObjectGrabable>();
        currentScene = SceneManager.GetActiveScene();
    }

    // Update is called once per frame
    void Update()
    {
        if (objectgrabable.isGrabbed == true && Input.GetKeyDown("t"))
        {
            if (currentScene.name == "Therapy Scene")
            {
                StartCoroutine(LoadTutorialScene());
            }
            else if (currentScene.name == "tutorial")
            {
                StartCoroutine(ReturnToTherapy());
            }
        }
    }

    private IEnumerator LoadTutorialScene()
    {
        // Load the new scene additively
        yield return SceneManager.LoadSceneAsync("tutorial", LoadSceneMode.Additive);

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
    }
}
