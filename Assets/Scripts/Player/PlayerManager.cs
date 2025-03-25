using UnityEngine;

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
        
    }
}
