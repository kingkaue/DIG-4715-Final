using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public int objectindex;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    
}
