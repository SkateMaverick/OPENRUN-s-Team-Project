using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DungeonDialogueManager : MonoBehaviour
{
    [Header("Dungeon Dialogue Sequence")]
    [SerializeField] private DialogueSequence dungeonIntroDialogue;
    [SerializeField] private List<DialogueLine> inlineDialogueLines = new List<DialogueLine>();

    [Header("Trigger Options")]
    [SerializeField] private bool playOnStart = false;

    [Header("Events")]
    public UnityEvent onDialogueFinished;

    private void Start()
    {
        if (playOnStart)
        {
            PlayDungeonDialogue();
        }
    }

    public void PlayDungeonDialogue()
    {
        if (DialogueCutsceneManager.Instance == null)
        {
            Debug.LogWarning("[DungeonDialogueManager] DialogueCutsceneManager.Instance is null!");
            return;
        }

        if (dungeonIntroDialogue != null)
        {
            DialogueCutsceneManager.Instance.StartDialogue(dungeonIntroDialogue, () => onDialogueFinished?.Invoke());
        }
        else if (inlineDialogueLines != null && inlineDialogueLines.Count > 0)
        {
            DialogueCutsceneManager.Instance.StartDialogue(inlineDialogueLines, () => onDialogueFinished?.Invoke());
        }
    }
}

