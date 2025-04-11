using System.Collections;
using UnityEngine;
using TMPro;
using Unity.Cinemachine;
public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TMP_Text textLabel;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject cutsceneCamera;
    [SerializeField] private GameObject playerCamera;


    public bool IsOpen {  get; private set; }  

    private ResponseHandler responseHandler;
    private TypeWriterEffect typewriterEffect;
    private void Start()
    {
        typewriterEffect = GetComponent<TypeWriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();
        CloseDialogeBox();
    }

    public void ShowDialogue(DialogueObject dialogueObject)
    {
        IsOpen = true;
        dialogueBox.SetActive(true);
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        for (int i = 0; i < dialogueObject.Dialogue.Length; i++)
        {
            string dialogue = dialogueObject.Dialogue[i];
            yield return typewriterEffect.Run(dialogue, textLabel);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        }

        if (dialogueObject.HasResponses)
        {
            responseHandler.ShowResponses(dialogueObject.Responses);
        }
        else
        {
            CloseDialogeBox(); // This will now handle cameras IMMEDIATELY
        }
    }

    private void CloseDialogeBox()
    {
        IsOpen = false;
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;

        // INSTANT camera switch (no coroutine)
        if (cutsceneCamera != null) cutsceneCamera.SetActive(false);
        if (playerCamera != null) playerCamera.SetActive(true);

        Debug.Log("Camera switched at EXACT moment dialogue ended");
    }

    private GameObject currentCutsceneCamera;
    private GameObject currentPlayerCamera;

    public void SetCutsceneCameras(GameObject cutsceneCam, GameObject playerCam)
    {
        currentCutsceneCamera = cutsceneCam;
        currentPlayerCamera = playerCam;
    }

    private IEnumerator SwitchCamerasAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (currentCutsceneCamera != null)
            currentCutsceneCamera.SetActive(false);

        if (currentPlayerCamera != null)
            currentPlayerCamera.SetActive(true);

        Debug.Log($"Camera switched at {Time.time}");
    }
}
