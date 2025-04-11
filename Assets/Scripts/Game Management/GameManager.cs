using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private PlayerMovement playerMovement;
    public Dictionary<string, int> flowers;
    public bool inColor = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveSystem.Save(playerManager, playerMovement);
            Debug.Log("Game saved!");
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            if (SaveSystem.Load(playerManager, playerMovement))
            {
                Debug.Log("Game loaded!");
            }
            else
            {
                Debug.Log("No save file found!");
            }
        }
    }
}