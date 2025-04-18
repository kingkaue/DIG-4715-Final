using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player References")]
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Game State")]
    public Dictionary<string, int> flowers = new Dictionary<string, int>();
    public bool inColor = false;
    public bool inbugscene = false;

    [Header("Cutscene Tracking")]
    private HashSet<string> playedCutscenes = new HashSet<string>();

    #region Initialization
    private void Awake()
    {
        InitializeSingleton();
        InitializeFlowerDictionary();
        LoadCutsceneData();
    }

    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeFlowerDictionary()
    {
        flowers = new Dictionary<string, int>();
        // Initialize with default flower types if needed
        // flowers.Add("rose", 0);
        // flowers.Add("poppy", 0);
    }
    #endregion

    #region Flower Inventory System
    public void AddFlowerToInventory(string flowerName)
    {
        if (!flowers.ContainsKey(flowerName))
        {
            flowers.Add(flowerName, 1);
        }
        else
        {
            flowers[flowerName]++;
        }

        // Special flower effects
        if (flowerName == "poppy")
        {
            playerManager?.SetSpirit(10);
        }
    }

    public bool TryUseFlower(string flowerName)
    {
        if (flowers.ContainsKey(flowerName) && flowers[flowerName] > 0)
        {
            flowers[flowerName]--;
            return true;
        }
        return false;
    }
    #endregion

    #region Cutscene Management
    public bool HasCutscenePlayed(string sceneName, string cutsceneID)
    {
        return playedCutscenes.Contains($"{sceneName}_{cutsceneID}");
    }

    public void MarkCutscenePlayed(string sceneName, string cutsceneID)
    {
        string key = $"{sceneName}_{cutsceneID}";
        if (!playedCutscenes.Contains(key))
        {
            playedCutscenes.Add(key);
            SaveCutsceneData();
        }
    }

    private void SaveCutsceneData()
    {
        PlayerPrefs.SetString("PlayedCutscenes", string.Join(",", playedCutscenes));
        PlayerPrefs.Save();
    }

    private void LoadCutsceneData()
    {
        if (PlayerPrefs.HasKey("PlayedCutscenes"))
        {
            string[] cutsceneArray = PlayerPrefs.GetString("PlayedCutscenes").Split(',');
            playedCutscenes = new HashSet<string>(cutsceneArray);
        }
    }
    #endregion

    #region Save/Load System
    private void Update()
    {
        HandleDebugInput();
    }

    private void HandleDebugInput()
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
    #endregion

    #region Scene Management
    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion
}