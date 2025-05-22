using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDialogue : MonoBehaviour
{
    [SerializeField] DialogueManager manager;
    [SerializeField] DialogueNode startNode;
    void Start()
    {
        Invoke("StartThis", 1f);
    }

    void StartThis()
    {
        manager.StartDialogue(startNode);
    }
}

