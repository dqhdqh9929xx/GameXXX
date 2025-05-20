using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueNode", menuName = "Dialogue/Node")]
public class DialogueNode : ScriptableObject
{
    public DialogueLine[] lines;
    public Choice[] choices;
    public string nextScene;
}

[System.Serializable]
public class Choice
{
    public string choiceText; // The text that will be displayed on the button
    public DialogueNode nextNode;
}
