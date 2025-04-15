using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private static GameObject[] persistentObjects = new GameObject[10];
    public int objectindex;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    
}
