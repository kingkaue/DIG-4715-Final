using UnityEngine;

[System.Serializable]
public class Response
{
    [SerializeField] private string responseText;
    [SerializeField] private DialogueObject dialogueObject;
    [SerializeField] private DialogueSpeaker speaker; // Add this field

    public string ResponseText => responseText;
    public DialogueObject DialogueObject => dialogueObject;
    public DialogueSpeaker Speaker => speaker; // Add this property
}