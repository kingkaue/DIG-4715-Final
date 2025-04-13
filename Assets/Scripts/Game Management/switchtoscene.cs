using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class switchtoscene : MonoBehaviour
{
    [SerializeField] private string scene;
    void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene("Forest Trail");
    }
}
