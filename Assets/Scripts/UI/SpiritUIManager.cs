using System.Numerics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SpiritUIManager : MonoBehaviour
{
    [SerializeField] private float spirit;
    [SerializeField] private float maxSpirit;
    [SerializeField] private float width;
    [SerializeField] private float height;
    [SerializeField] private RectTransform spiritBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMaxSpirit(float MaxSpirit)
    {
        maxSpirit = MaxSpirit;
    }

    public void SetSpirit(float Spirit)
    {
        spirit = Spirit;
        float newWidth = (spirit / maxSpirit) * width;

        spiritBar.sizeDelta = new UnityEngine.Vector2(newWidth, height);
    }
}
