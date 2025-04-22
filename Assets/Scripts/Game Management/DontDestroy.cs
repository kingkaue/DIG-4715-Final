using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroy : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            DestroyObject();
        }   
    }


    public void DestroyObject()
    {
        Destroy(this.gameObject);
    }
}
