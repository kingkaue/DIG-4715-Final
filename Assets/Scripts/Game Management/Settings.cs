using System.Linq.Expressions;
using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] Button backButton;
    [SerializeField] GameObject pauseMenuUI;

    [Header ("Volume Settings")]
    [SerializeField] Slider volumeSlider;

    [Header ("Sensitivity Settings")]
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] private float minSensitivity;
    [SerializeField] private float maxSensitivity;
    [SerializeField] private float defaultSensitivity;
    [SerializeField] private InputActionAsset inputActionAsset;
    private const string mousePath = "<Pointer>";

    public void SetScale(InputAction action, string bindingPathStart, Vector2 scale)
    {
        var bindings = action.bindings;
        for (int i = 0; i < bindings.Count; i++)
        {
            if (bindings[i].isPartOfComposite || !bindings[i].path.StartsWith(bindingPathStart)) continue;
            action.ApplyBindingOverride(i, new InputBinding {overrideProcessors = $"ScaleVector2(x={scale.x},y={scale.y})"});
            return;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var action = inputActionAsset.FindAction("Look");
        backButton.onClick.AddListener(BackToPause);
        sensitivitySlider.onValueChanged.AddListener(ChangeSensitivity);
        AudioListener.volume = volumeSlider.value;

        sensitivitySlider.minValue = minSensitivity;
        sensitivitySlider.maxValue = maxSensitivity;
        sensitivitySlider.value = defaultSensitivity;

        SetScale(action, mousePath, new Vector2(defaultSensitivity, defaultSensitivity));
    }
    public void ChangeSensitivity(float newValue)
    {
        var action = inputActionAsset.FindAction("Look");
        SetScale(action, mousePath, new Vector2(newValue, newValue));
    }

    private void BackToPause()
    {
        pauseMenuUI.SetActive(true);
        gameObject.SetActive(false);
    }

    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }
}
