using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
public class PlayerManager : MonoBehaviour
{
    [Header("Spirit")]
    [SerializeField] private float spirit;
    [SerializeField] private float maxSpirit;
    [SerializeField] private SpiritUIManager spiritBar;
    private float colorThreshold;
    private GameObject gameManager;

    void Start()
    {
        spiritBar.SetMaxSpirit(maxSpirit);
        gameManager = GameObject.FindGameObjectWithTag("GameController");
        colorThreshold = maxSpirit * 0.3f;
    }

    // Update is called once per frame
    void Update()
    {
        // Implememt spirit managing controls here
        if (Input.GetKeyDown(KeyCode.P))
        {
            SetSpirit(-10f);
            Debug.Log("Subtracting Spirit");
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            SetSpirit(10f);
            Debug.Log("Adding Spirit");
        }

        if (spirit >= colorThreshold)
        {
            gameManager.GetComponent<GameManager>().inColor = true;
        }
    }

    public void SetSpirit(float spiritChange)
    {
        spirit += spiritChange;
        spirit = Mathf.Clamp(spirit, 0, maxSpirit);

        spiritBar.SetSpirit(spirit);
    }

    public void Save(ref PlayerSaveData data)
    {
        data.Spirit = spirit;
        data.MaxSpirit = maxSpirit;
    }

    public void Load(PlayerSaveData data)
    {
        spirit = data.Spirit;
        maxSpirit = data.MaxSpirit;
        spiritBar.SetMaxSpirit(maxSpirit);
        spiritBar.SetSpirit(spirit);
    }
}
