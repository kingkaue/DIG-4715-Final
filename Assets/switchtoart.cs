using UnityEngine;
using UnityEngine.SceneManagement;

public class switchtoart : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OggerEnter(Collider other)
    {
        SceneManager.LoadScene("Prototype Scene");
    }
}
