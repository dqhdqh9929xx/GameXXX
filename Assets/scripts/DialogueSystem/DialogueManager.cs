using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;

public class DialogueManager : MonoBehaviour
{

    [Header("UI Elements")]
    [SerializeField] TMP_Text speakerNametext;
    [SerializeField] TMP_Text dialogueText;
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] GameObject choicePanel;
    [SerializeField] Button choiceButtonPrefab;
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
    public AudioSource effectAudioSource;

    [Header("Setting")]
    [SerializeField] float textSpeed = 0.05f; // Speed of Text
    [SerializeField] float typingSpeed = 0.05f;
    DialogueNode currentNode;
    int currentLineIndex = 0;
    bool isTyping = false;


    void Start()
    {
        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);
        // Add listener to the progress button
        progressButton.onClick.AddListener(OnClickAdvance);
        // Hide the progress button
        if (leftImage != null && deActivateLeftImage) leftImage.color = new Color32(255, 255, 255, 0);
        if (rightImage != null && deActivateRightImage) rightImage.color = new Color32(255, 255, 255, 0);
        if (centreImage != null && deActivateCentreImage) centreImage.color = new Color32(255, 255, 255, 0);
    }

    public void StartDialogue(DialogueNode startNode)
    {
        dialoguePanel.SetActive(true);
        currentNode = startNode;
        currentLineIndex = 0;
        DisplayCurrentLine();
    }

    void DisplayCurrentLine()
    {
        if (currentNode == null || currentNode.lines.Length == 0)
        {
            // End Dialogue Here
            return;
        }

        // Check If There Are Any Lines To Display
        if (currentLineIndex < currentNode.lines.Length)
        {
            DialogueLine line = currentNode.lines[currentLineIndex];
            speakerNametext.text = line.speakerName;

            // Target Image to be placed from line
            Image targetImage = GetTargetImage(line.targetImage);

            if (targetImage != null && line.characterSprites!=null)
            {
                targetImage.sprite = line.characterSprites;
                targetImage.color = Color.white;
            }
            // Play Audio Clips
            PlayAudio(line);

            // Start Coroutine Animate Character And Type Text Afterwards
            StartCoroutine(AnimateAndType(line, targetImage));
        }
        else
        {
            // End of Lines, Display Choises
            DisplayChoises();
        }
    }

    Image GetTargetImage(DialogTarget targetImage)
    {
        switch (targetImage)
        {
            case DialogTarget.LeftImage: return leftImage;
            case DialogTarget.RightImage: return rightImage;
            case DialogTarget.CentreImage: return centreImage;
            default: return null;

        }
    }

    IEnumerator AnimateAndType(DialogueLine line, Image targetImage)
    {
        // Perform Ana animation based on line settings
        if (line.animationType != DialogueAnimation.None)
        {
            // Play Animation
            yield return new WaitForSeconds(line.animationDuration);
        }

        // Start Writing Text
        yield return StartCoroutine(TypeText(line.text));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = ""; // Clear old text
        int visibleCharCount = 0; // Track Visible Characters For TextMeshPro

        for (int i = 0; i < text.Length; i++)
        {
            // Filter Rick Text
            if (text[i] == '<')
            {
                int closingTagIndex = text.IndexOf('>', i);
                if (closingTagIndex != -1)
                {
                    // Add The Enter Tag To the Dialogue Text Instandly
                    dialogueText.text += text.Substring(i, closingTagIndex - i + 1);
                    i = closingTagIndex; // Skip To The End Of The Tag
                    continue;
                }
            }
            // Add The Next Visible Charater
            dialogueText.text += text[i];
            visibleCharCount++;
            // Ensure Textmeshpro Updates Properly
            dialogueText.maxVisibleCharacters = visibleCharCount;
            yield return new WaitForSeconds(textSpeed);
        }
        // "<color=red>name></color>" <b></b> <i></i> <u></u>
        
        isTyping = false; // Mark Typing As Finished
    }

    void PlayAudio(DialogueLine line)
    {

        // If There is something playing - stop it
        if (voiceAudioSource.isPlaying)
        {
            voiceAudioSource.Stop();
        }
        // Play Voice Text
        if (line.spokenText != null)
        {
            voiceAudioSource.clip = line.spokenText;
            voiceAudioSource.Play();
        }
        // Play mood or effect
        if (line.moorOrEffect != null)
        {
            effectAudioSource.clip = line.moorOrEffect;
            effectAudioSource.Play();
        }
    }

    public void OnClickAdvance() // This will be on the little chat button
    {
        if (isTyping)
        {
            // Skip Typing
            StopAllCoroutines ();
            // Show Full Text Line
            DialogueLine currentLine = currentNode.lines[currentLineIndex];
            dialogueText.text = currentLine.text; // Assign the full text
            dialogueText.maxVisibleCharacters = currentLine.text.Length;
            isTyping = false;
            // Stop Audio of the voice 
            if (voiceAudioSource.isPlaying)
            {
                voiceAudioSource.Stop();
            }
        }
        else
        {
            currentLineIndex++;
            DisplayCurrentLine();
        }
    }

    void DisplayChoises()
    {
        // Clear All Existing Buttons
        foreach (Transform child in choicePanel.transform)
        {
            Destroy(child.gameObject);
        }
        choicePanel.SetActive(true);
        // Create All Buttons from choises
        foreach (Choice choice in currentNode.choices)
        {
            Button choiceButton = Instantiate(choiceButtonPrefab, choicePanel.transform);
            Debug.Log("Đã tạo nút lựa chọn: " + choice.choiceText);
            choiceButton.GetComponentInChildren<TMP_Text>().text = choice.choiceText;
            choiceButton.onClick.AddListener(() => SelectChoice(choice));

        }
    }

    void SelectChoice(Choice choice)
    {
        if (choice.nextNode != null)
        {
            choicePanel.SetActive(false);
            StartDialogue(choice.nextNode);
        }
    }

}




