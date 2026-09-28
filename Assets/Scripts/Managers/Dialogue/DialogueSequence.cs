using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueSequence", menuName = "Dialogue/Dialogue Sequence")]
public class DialogueSequence : ScriptableObject
{
    [Tooltip("Descriptive title for this dialogue scene.")]
    public string sequenceTitle = "New Cutscene Dialogue";

    [Tooltip("List of dialogue lines in this sequence.")]
    public List<DialogueLine> lines = new List<DialogueLine>();
}
