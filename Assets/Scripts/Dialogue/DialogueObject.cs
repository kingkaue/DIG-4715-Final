using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject
{
    [System.Serializable]
    public class DialogueLine
    {
        [TextArea] public string text;
        public DialogueSpeaker speaker;
    }

    [SerializeField] private DialogueLine[] dialogue;
    [SerializeField] private Response[] responses;

    public DialogueLine[] Dialogue => dialogue;
    public bool HasResponses => Responses != null && Responses.Length > 0;
    public Response[] Responses => responses;
}