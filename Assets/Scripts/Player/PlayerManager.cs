using UnityEngine;
using UnityEngine.Rendering;

public class PlayerManager : MonoBehaviour
{
    [Header ("Spirit")]
    [SerializeField] private float spirit;
    [SerializeField] private float maxSpirit;
    [SerializeField] private SpiritUIManager spiritBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spiritBar.SetMaxSpirit(maxSpirit);
    }

    // Update is called once per frame
    void Update()
    {
        // Implememt spirit managing controls here
        if(Input.GetKeyDown(KeyCode.P))
        {
            SetSpirit(-10f);
        }

        if(Input.GetKeyDown(KeyCode.O))
        {
            SetSpirit(10f);
        }
    }

    public void SetSpirit(float spiritChange)
    {
        spirit += spiritChange;
        spirit = Mathf.Clamp(spirit, 0, maxSpirit);

        spiritBar.SetSpirit(spirit);
    }
}
