using System;
using UnityEngine;

[Serializable]
public class DialogueLine
{
    [Tooltip("Name of the speaker. Leave blank for narration.")]
    public string speakerName = "Paimon";

    [Tooltip("Color of the speaker's name text.")]
    public Color speakerColor = new Color(1.0f, 0.79f, 0.22f, 1.0f); // Genshin Warm Gold

    [TextArea(2, 5)]
    [Tooltip("The dialogue text to display.")]
    public string dialogueText = "";

    [Tooltip("Optional voiceover or sound effect for this line.")]
    public AudioClip voiceClip;

    [Tooltip("Optional camera focus target or cutscene virtual camera during this line.")]
    public Transform cameraFocus;

    public DialogueLine() { }

    public DialogueLine(string speaker, string text, Color? color = null)
    {
        speakerName = speaker;
        dialogueText = text;
        if (color.HasValue)
        {
            speakerColor = color.Value;
        }
    }
}
