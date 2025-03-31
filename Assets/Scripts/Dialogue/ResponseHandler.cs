using UnityEngine;
using TMPro;
using UnityEngine.UI;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
public class ResponseHandler : MonoBehaviour
{
    [SerializeField] private RectTransform responseBox;
    [SerializeField] private RectTransform responseButtonTemplate;
    [SerializeField] private RectTransform responseContainer;

    private DialogueUI dialogueUI;

    private List<GameObject> tempResponseButtons = new List<GameObject>();


    private void Start()
    {
        dialogueUI = GetComponent<DialogueUI>();
        responseButtonTemplate.gameObject.SetActive(false);
    }

    public void ShowResponses(Response[] responses)
    {
        float responseBoxHeight = 0;
        float buttonHeight = responseButtonTemplate.sizeDelta.y;
        float spacing = 10f; // Add some spacing between buttons if needed

        foreach (Response response in responses)
        {
            GameObject responseButton = Instantiate(responseButtonTemplate.gameObject, responseContainer);
            responseButton.gameObject.SetActive(true);
            responseButton.GetComponent<TMP_Text>().text = response.ResponseText;
            responseButton.GetComponent<Button>().onClick.AddListener(() => OnPickedResponse(response));

            tempResponseButtons.Add(responseButton);

            // Calculate total height including spacing (except after last button)
            responseBoxHeight += buttonHeight + (tempResponseButtons.Count < responses.Length ? spacing : 0);
        }

        // Apply the calculated height
        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeight);
        responseBox.gameObject.SetActive(true);
    }

    private void OnPickedResponse(Response response)
    {
        responseBox.gameObject.SetActive(false);

        foreach (GameObject button in tempResponseButtons)
        {
            Destroy(button);
        }

        tempResponseButtons.Clear();

        dialogueUI.ShowDialogue(response.DialogueObject);
    }
}
