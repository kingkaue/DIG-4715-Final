using UnityEngine;

public class gravestoneactivate : MonoBehaviour
{
    private GameManager gameManager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "singlepoppy")
        {
            Debug.Log("Holy Shit You Win!");
        }

    }
}
