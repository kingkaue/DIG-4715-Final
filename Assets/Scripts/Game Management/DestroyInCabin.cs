using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyInCabin : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Cabin Scene")
        {
            Destroy(this.gameObject);
        }
    }
}
