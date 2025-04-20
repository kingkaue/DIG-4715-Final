using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private PlayerMovement playerMovement;
    public Dictionary<string, int> flowers;
    public bool inColor = false;
    public bool inbugscene = false;
    [SerializeField] private InputActionAsset inputActionAsset;
    [SerializeField] private GameObject deerinthehub;

    // Track which cutscenes have been played (key is sceneName + cutsceneID)
    private HashSet<string> playedCutscenes = new HashSet<string>();

    public float deerscore = 0f;


    private void Awake()
    {
        Settings settings = new Settings();
        settings.SetScale(inputActionAsset.FindAction("Look"), "<Pointer>", new Vector2(5, 5));
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        flowers = new Dictionary<string, int>();
        LoadCutsceneData();
    }

    public bool HasCutscenePlayed(string sceneName, string cutsceneID)
    {
        string key = $"{sceneName}_{cutsceneID}";
        return playedCutscenes.Contains(key);
    }

    public void MarkCutscenePlayed(string sceneName, string cutsceneID)
    {
        string key = $"{sceneName}_{cutsceneID}";
        playedCutscenes.Add(key);
        SaveCutsceneData();
    }

    private void SaveCutsceneData()
    {
        // Convert to list for serialization
        List<string> cutsceneList = new List<string>(playedCutscenes);
        PlayerPrefs.SetString("PlayedCutscenes", string.Join(",", cutsceneList));
        PlayerPrefs.Save();
    }

    private void LoadCutsceneData()
    {
        if (PlayerPrefs.HasKey("PlayedCutscenes"))
        {
            string savedData = PlayerPrefs.GetString("PlayedCutscenes");
            string[] cutsceneArray = savedData.Split(',');
            playedCutscenes = new HashSet<string>(cutsceneArray);
        }
    }

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

    public void IncreaseDeerScore(float amount)
    {
        deerscore += amount;
        Debug.Log($"Deer score increased! New score: {deerscore}");
        // - Trigger events when score reaches certain values
        // - Save the score
        if(deerscore >= 2)
        {
            deerinthehub.SetActive(true);
        }
    }
}