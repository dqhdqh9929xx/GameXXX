using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueLine", menuName = "Dialogue/Line")]
public class DialogueLine : ScriptableObject
{
    public string speakerName;
    [TextArea(3, 5)] public string text;
    public Sprite speakerSprite;

    public DialogTarget targetImage;

    [Header("Audio")]
    public AudioClip spokenText;
    public AudioClip moorOrEffect;

    [Header("Animation")]
    public float animationDuration;
    public DialogueAnimation animationType;
}

public enum DialogTarget
{
    LeftImage,
    RightImage,
    CentreImage
}

public enum DialogueAnimation
{
    None,
    EnteringScene,
    ExitingScene,
    Jumping,
    Shaking,
    Scaling,
    Rotating,
}
