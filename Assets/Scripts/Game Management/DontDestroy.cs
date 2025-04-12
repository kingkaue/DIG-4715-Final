using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private static GameObject[] persistentObjects = new GameObject[10];
    public int objectindex;

    void Awake()
    {
        if(persistentObjects[objectindex] == null)
        {
            persistentObjects[objectindex] = gameObject;
            DontDestroyOnLoad(gameObject);
        }
        else if (persistentObjects[objectindex] != gameObject)
        {
            Destroy(gameObject);
        }


    }

    
}
