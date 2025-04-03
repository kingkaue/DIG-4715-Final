using System.Collections;
using System.Numerics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SpiritUIManager : MonoBehaviour
{
    [Header("Bar Properties")]
    [SerializeField] private float spirit;
    [SerializeField] private float maxSpirit;
    [SerializeField] private float width;
    [SerializeField] private float height;
    [SerializeField] private RectTransform spiritBar;

    [Header("Smoothness")]
    [SerializeField] private float timeToDrain = 0.25f;
    private float targetWidth;
    private float currentWidth;
    private Coroutine drainSpiritCoroutine;

    [Header("Color Change")]
    [SerializeField] private Image image;
    [SerializeField] private Gradient spiritGradient;
    private Color newSpiritColor;


    void Start()
    {
        Debug.Log(maxSpirit);
        maxSpirit = 1;
        image.color = spiritGradient.Evaluate(spirit / maxSpirit);
        currentWidth = (spirit / maxSpirit) * width;
        spiritBar.sizeDelta = new UnityEngine.Vector2(currentWidth, height);
    }

    public void SetMaxSpirit(float MaxSpirit)
    {
        maxSpirit = MaxSpirit;
    }

    public void SetSpirit(float Spirit)
    {
        spirit = Spirit;
        targetWidth = (spirit / maxSpirit) * width;

        // Smooth transition
        drainSpiritCoroutine = StartCoroutine(DrainSpirit());
        CheckSpiritGradientAmount();
    }

    // Coroutine to smoothly show spirit gain and loss
    private IEnumerator DrainSpirit()
    {
        float startWidth = currentWidth;
        Color currentColor = image.color;

        float elapsedTime = 0f;
        while (elapsedTime < timeToDrain)
        {
            elapsedTime += Time.deltaTime;

            // Lerp length of spirit bar
            spiritBar.sizeDelta = new UnityEngine.Vector2(Mathf.Lerp(startWidth, targetWidth, (elapsedTime / timeToDrain)), height);

            // Lerp color of spirit bar
            image.color = Color.Lerp(currentColor, newSpiritColor, (elapsedTime / timeToDrain));
            yield return null;
        }

        currentWidth = targetWidth;
        spiritBar.sizeDelta = new UnityEngine.Vector2(currentWidth, height);
        drainSpiritCoroutine = null;
    }

    private void CheckSpiritGradientAmount()
    {
        // Gets the new spirit color
        newSpiritColor = spiritGradient.Evaluate(spirit / maxSpirit);
    }
}
