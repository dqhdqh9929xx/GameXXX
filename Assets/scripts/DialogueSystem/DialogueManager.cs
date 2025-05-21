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


    [Header("Audio")]
    public AudioSource voiceAudioSource;
    public AudioSource musicAudioSource;
    public AudioSource effectAudioSource;

    [Header("Setting")]
    [SerializeField] float textSpeed = 0.05f; // Speed of Text
    //[SerializeField] float typingSpeed = 0.05f;
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
            EndDialogue();
            return;
        }

        // Check If There Are Any Lines To Display
        if (currentLineIndex < currentNode.lines.Length)
        {
            DialogueLine line = currentNode.lines[currentLineIndex];
            speakerNametext.text = line.speakerName;

            // Nếu đúng node 1 và dòng thứ 2:
            if ( currentLineIndex == 1)
            {
                // Chạy animation "rotate" cho rightImage
                Playanimation(DialogueAnimation.Rotating, rightImage);
                Debug.Log("Đã chạy animation 'rotate' cho rightImage");
            }

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
            Playanimation(line.animationType, targetImage);
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
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);
        dialogueText.text = "";
        speakerNametext.text = "";
        if (voiceAudioSource.isPlaying)
        {
            voiceAudioSource.Stop();
        }
        if (musicAudioSource.isPlaying)
        {
            musicAudioSource.Stop();
        }
        Debug.Log("End of Dialogue");
        // Transition to another scene if needed
        if (!string.IsNullOrEmpty(currentNode.nextScene))
        {
            SceneTransitionManager.Instance.LoadSceneWithFade(currentNode.nextScene);
        }
    }

    void Playanimation (DialogueAnimation animationType, Image targetImage)
    {
        if (targetImage == null) return;
        // Get the Animator component
        Animator animator = targetImage.GetComponent<Animator>();
        if (animator != null) return;

        // Trigger the correct animation
        switch (animationType)
        {
            case DialogueAnimation.LeftEnteringScene:
                animator.SetTrigger("leftenter");
                break;
            case DialogueAnimation.LeftExitingScene:
                animator.SetTrigger("leftexit");
                break;
            case DialogueAnimation.RightEnteringScene:
                animator.SetTrigger("rightenter");
                break;
            case DialogueAnimation.RightExitingScene:
                animator.SetTrigger("rightexit");
                break;
            case DialogueAnimation.Jumping:
                animator.SetTrigger("jumping");
                break;
            case DialogueAnimation.Shaking:
                animator.SetTrigger("shake");
                break;
            case DialogueAnimation.Scaling:
                animator.SetTrigger("scale");
                break;
            case DialogueAnimation.Rotating:
                animator.SetTrigger("rotate");
                break;
            case DialogueAnimation.Floating:
                animator.SetTrigger("float");
                break;
            default:
                Debug.LogWarning("No animation found for this type: " + animationType);
                break;
        }

    }

}




