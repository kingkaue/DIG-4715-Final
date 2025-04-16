using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ResponseHandler : MonoBehaviour
{
    [Header("Response UI")]
    [SerializeField] private RectTransform responseBox;
    [SerializeField] private RectTransform responseButtonTemplate;
    [SerializeField] private RectTransform responseContainer;

    [Header("Name Bar")]
    [SerializeField] private TMP_Text responseNameLabel;
    [SerializeField] private Image responseNameBar;
    [SerializeField] private Image responseSpeakerIcon; // Optional

    private DialogueUI dialogueUI;
    private List<GameObject> tempResponseButtons = new List<GameObject>();

    private void Start()
    {
        dialogueUI = GetComponent<DialogueUI>();
        responseButtonTemplate.gameObject.SetActive(false);
        HideResponseNameBar();
    }

    public void ShowResponses(Response[] responses)
    {
        float responseBoxHeight = 0;
        float buttonHeight = responseButtonTemplate.sizeDelta.y;
        float spacing = 10f;

        foreach (Response response in responses)
        {
            GameObject responseButton = Instantiate(responseButtonTemplate.gameObject, responseContainer);
            responseButton.gameObject.SetActive(true);

            // Format response text with speaker name if available
            string responseText = FormatResponseText(response);
            responseButton.GetComponent<TMP_Text>().text = responseText;

            // Set up button colors based on speaker
            SetupResponseButtonColors(responseButton, response);

            responseButton.GetComponent<Button>().onClick.AddListener(() => OnPickedResponse(response));

            tempResponseButtons.Add(responseButton);
            responseBoxHeight += buttonHeight + (tempResponseButtons.Count < responses.Length ? spacing : 0);
        }

        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeight);
        responseBox.gameObject.SetActive(true);
    }

    private string FormatResponseText(Response response)
    {
        if (response.Speaker != null)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(response.Speaker.nameColor)}>" +
                   $"{response.Speaker.speakerName}:</color> {response.ResponseText}";
        }
        return response.ResponseText;
    }

    private void SetupResponseButtonColors(GameObject button, Response response)
    {
        var colors = button.GetComponent<Button>().colors;

        if (response.Speaker != null)
        {
            colors.normalColor = response.Speaker.nameColor * 0.2f;
            colors.highlightedColor = response.Speaker.nameColor * 0.4f;
            colors.pressedColor = response.Speaker.nameColor * 0.6f;
        }
        else
        {
            // Default colors if no speaker specified
            colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 0.2f);
            colors.highlightedColor = new Color(0.4f, 0.4f, 0.4f, 0.4f);
            colors.pressedColor = new Color(0.6f, 0.6f, 0.6f, 0.6f);
        }

        button.GetComponent<Button>().colors = colors;
    }

    private void OnPickedResponse(Response response)
    {
        responseBox.gameObject.SetActive(false);
        HideResponseNameBar();

        foreach (GameObject button in tempResponseButtons)
        {
            Destroy(button);
        }
        tempResponseButtons.Clear();

        // Show speaker info before displaying the response dialogue
        if (response.Speaker != null)
        {
            ShowResponseNameBar(response.Speaker);
        }

        dialogueUI.ShowDialogue(response.DialogueObject);
    }

    private void ShowResponseNameBar(DialogueSpeaker speaker)
    {
        if (responseNameLabel != null)
        {
            responseNameLabel.text = speaker.speakerName;
            responseNameLabel.color = speaker.nameColor;
        }

        if (responseNameBar != null)
        {
            responseNameBar.color = speaker.nameColor * 0.8f;
            responseNameBar.gameObject.SetActive(true);
        }

        if (responseSpeakerIcon != null)
        {
            responseSpeakerIcon.sprite = speaker.speakerIcon;
            responseSpeakerIcon.gameObject.SetActive(speaker.speakerIcon != null);
        }
    }

    private void HideResponseNameBar()
    {
        if (responseNameLabel != null) responseNameLabel.text = "";
        if (responseNameBar != null) responseNameBar.gameObject.SetActive(false);
        if (responseSpeakerIcon != null) responseSpeakerIcon.gameObject.SetActive(false);
    }
}