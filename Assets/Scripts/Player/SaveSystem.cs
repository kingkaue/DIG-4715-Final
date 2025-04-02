using System.IO;
using UnityEngine;

public class SaveSystem
{
    private static SaveData _saveData = new SaveData();

    [System.Serializable]
    public struct SaveData
    {
        public PlayerSaveData PlayerData;
    }

    public static string SaveFileName()
    {
        return Path.Combine(Application.persistentDataPath, "save.save");
    }

    public static void Save(PlayerManager playerManager, PlayerMovement playerMovement)
    {
        HandleSaveData(playerManager, playerMovement);
        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(_saveData, true));
    }

    private static void HandleSaveData(PlayerManager playerManager, PlayerMovement playerMovement)
    {
        playerManager.Save(ref _saveData.PlayerData);
        playerMovement.Save(ref _saveData.PlayerData);
    }

    public static bool Load(PlayerManager playerManager, PlayerMovement playerMovement)
    {
        if (!File.Exists(SaveFileName())) return false;

        string saveContent = File.ReadAllText(SaveFileName());
        _saveData = JsonUtility.FromJson<SaveData>(saveContent);

        HandleLoadData(playerManager, playerMovement);
        return true;
    }

    private static void HandleLoadData(PlayerManager playerManager, PlayerMovement playerMovement)
    {
        playerManager.Load(_saveData.PlayerData);
        playerMovement.Load(_saveData.PlayerData);
    }
}