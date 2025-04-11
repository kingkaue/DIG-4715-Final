using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public string objectID;

    void Awake()
    {
        objectID = name + transform.position;   
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
