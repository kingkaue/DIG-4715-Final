using UnityEngine;
using UnityEngine.SceneManagement;
public class tutorialcutscene : MonoBehaviour
{

    private ObjectGrabable objectgrabable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        objectgrabable = GetComponent<ObjectGrabable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (objectgrabable.isGrabbed == true && Input.GetKeyDown("t"))
        {
            Debug.Log("Transition Cutscene Started");
            loadlevel("tutorial");
        }
    }

    public void loadlevel(string level)
    {
        SceneManager.LoadScene(level);
    }
}
