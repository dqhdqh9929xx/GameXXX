using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueNode", menuName = "Dialogue/Node")]
public class DialogueNode : ScriptableObject
{
    public DialogueLine[] lines;
    public Choise[] choises;
    public string nextScene;
}

[System.Serializable]
public class Choise
{
    public string choiseText;
    public DialogueNode nextNode;
}
