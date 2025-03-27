using System.Collections;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    [SerializeField] Material material;
    private Color originalColor;
    private Color grayscaleColor;
    private Coroutine changeColorCoroutine;
    private bool canChange = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalColor = material.color;
        grayscaleColor = new Color(0.299f * originalColor.r, 0.587f * originalColor.g, 0.114f * originalColor.b);
        material.color = grayscaleColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canChange)
        {
            canChange = false;
            changeColorCoroutine = StartCoroutine(GrayToColor());
        }
    }

    private IEnumerator GrayToColor()
    {
        float elapsedTime = 0f;
        float timeToChange = 2.0f;

        while (elapsedTime < timeToChange)
        {
            elapsedTime += Time.deltaTime;
            material.color = Color.Lerp(grayscaleColor, originalColor, (elapsedTime / timeToChange));
            yield return null;
        }

        changeColorCoroutine = null;
    }

}
