using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    [Header("UI Elements")]
    [SerializeField] TMP_Text speakerNametext;
    [SerializeField] TMP_Text dialogueText;
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] GameObject choisePanel;
    [SerializeField] Button choiseButtonPrefabs;
    [SerializeField] Button progressButton;


    [Header("Protagnists")]
    [SerializeField] Image leftImage;
    [SerializeField] Image rightImage;
    [SerializeField] Image centreImage;
    [SerializeField] bool deActivateLeftImage; // for Debug
    [SerializeField] bool deActivateRightImage; // for Debug
    [SerializeField] bool deActivateCentreImage; // for Debug

    [Header("Audio")]
    public AudioSource voiceAudioSource;
    public AudioSource musicAudioSource;

    [Header("Setting")]
    [SerializeField] float typingSpeed = 0.05f;
    DialogueNode currentNode;
    int currentDialogueIndex = 0;
    bool isTyping = false;

    void Start()
    {

    }


}
