using Unity.VisualScripting;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void DestroyObject()
    {
        Destroy(this.gameObject);
    }
}
