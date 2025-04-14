using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    public static Spawnpoint Instance { get; private set; }

    private void Awake()
    {
        Instance = this;   
    }
}
