using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private TMP_Text textLabel;
    [SerializeField] private GameObject dialogueBox;

    [Header("Name Bar")]
    [SerializeField] private TMP_Text nameLabel;
    [SerializeField] private Image nameBar;
    [SerializeField] private Image speakerIcon; // Optional

    [Header("Cameras")]
    [SerializeField] private GameObject cutsceneCamera;
    [SerializeField] private GameObject playerCamera;

    public bool IsOpen { get; private set; }

    private ResponseHandler responseHandler;
    private TypeWriterEffect typewriterEffect;

    private void Start()
    {
        typewriterEffect = GetComponent<TypeWriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();
        CloseDialogueBox();
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
            var dialogueLine = dialogueObject.Dialogue[i];

            // Update name bar and speaker info
            UpdateSpeakerInfo(dialogueLine.speaker);

            yield return typewriterEffect.Run(dialogueLine.text, textLabel);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        }

        if (dialogueObject.HasResponses)
        {
            responseHandler.ShowResponses(dialogueObject.Responses);
        }
        else
        {
            CloseDialogueBox();
        }
    }

    private void UpdateSpeakerInfo(DialogueSpeaker speaker)
    {
        if (speaker != null)
        {
            nameLabel.text = speaker.speakerName;
            nameLabel.color = speaker.nameColor;
            nameBar.color = speaker.nameColor * 0.8f; // Slightly darker than text

            if (speakerIcon != null)
            {
                speakerIcon.sprite = speaker.speakerIcon;
                speakerIcon.gameObject.SetActive(speaker.speakerIcon != null);
            }
        }
        else
        {
            nameLabel.text = "";
            nameBar.color = Color.clear;
            if (speakerIcon != null) speakerIcon.gameObject.SetActive(false);
        }
    }

    private void CloseDialogueBox()
    {
        IsOpen = false;
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;
        nameLabel.text = string.Empty;
        nameBar.color = Color.clear;
        if (speakerIcon != null) speakerIcon.gameObject.SetActive(false);

        // Switch cameras
        if (cutsceneCamera != null) cutsceneCamera.SetActive(false);
        if (playerCamera != null) playerCamera.SetActive(true);
    }

    public void SetCameras(GameObject cutsceneCam, GameObject playerCam)
    {
        cutsceneCamera = cutsceneCam;
        playerCamera = playerCam;
    }
}