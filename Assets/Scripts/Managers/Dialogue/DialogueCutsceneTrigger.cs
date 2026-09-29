using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DialogueCutsceneTrigger : MonoBehaviour
{
    [Header("Dialogue Content")]
    [Tooltip("Reference to a DialogueSequence asset. If assigned, takes precedence over inline lines.")]
    [SerializeField] private DialogueSequence dialogueSequence;

    [Tooltip("Inline dialogue lines if no sequence asset is assigned.")]
    [SerializeField] private List<DialogueLine> inlineLines = new List<DialogueLine>();

    [Header("Trigger Settings")]
    [SerializeField] private bool triggerOnStart = false;
    [SerializeField] private bool triggerOnPlayerEnter = true;
    [SerializeField] private bool triggerWithInteractKey = false;
    [SerializeField] private Key interactKey = Key.F;
    [SerializeField] private bool triggerOnlyOnce = true;

    [Header("Events")]
    public UnityEvent onDialogueStarted;
    public UnityEvent onDialogueCompleted;

    private bool _hasTriggered = false;
    private bool _playerInZone = false;

    private void Start()
    {
        if (triggerOnStart)
        {
            TriggerDialogue();
        }
    }

    private void Update()
    {
        if (triggerWithInteractKey && _playerInZone && !_hasTriggered)
        {
            bool keyPressed = false;
            if (Keyboard.current != null && Keyboard.current[interactKey].wasPressedThisFrame)
            {
                keyPressed = true;
            }
            else
            {
                try
                {
                    if (Input.GetKeyDown(KeyCode.F)) keyPressed = true;
                }
                catch { }
            }

            if (keyPressed)
            {
                TriggerDialogue();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other))
        {
            _playerInZone = true;
            if (triggerOnPlayerEnter && !_hasTriggered)
            {
                TriggerDialogue();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (triggerOnPlayerEnter && !_hasTriggered && IsPlayer(other))
        {
            _playerInZone = true;
            TriggerDialogue();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsPlayer(other))
        {
            _playerInZone = false;
        }
    }

    private bool IsPlayer(Collider other)
    {
        if (other == null) return false;
        if (other.CompareTag("Player")) return true;
        if (other.transform.root != null && other.transform.root.CompareTag("Player")) return true;
        if (other.GetComponentInParent<CharacterController>() != null) return true;
        if (other.name.Contains("Noa") || other.name.Contains("Que")) return true;
        return false;
    }

    public void TriggerDialogue()
    {
        if (triggerOnlyOnce && _hasTriggered)
            return;

        if (DialogueCutsceneManager.Instance == null)
        {
            Debug.LogWarning("[DialogueCutsceneTrigger] DialogueCutsceneManager instance not found!");
            return;
        }

        if (DialogueCutsceneManager.Instance.IsDialogueActive)
            return;

        _hasTriggered = true;
        onDialogueStarted?.Invoke();

        if (dialogueSequence != null && dialogueSequence.lines != null && dialogueSequence.lines.Count > 0)
        {
            DialogueCutsceneManager.Instance.StartDialogue(dialogueSequence, OnCutsceneFinished);
        }
        else if (inlineLines != null && inlineLines.Count > 0)
        {
            DialogueCutsceneManager.Instance.StartDialogue(inlineLines, OnCutsceneFinished);
        }
    }

    private void OnCutsceneFinished()
    {
        onDialogueCompleted?.Invoke();
    }

    public void ResetTrigger()
    {
        _hasTriggered = false;
    }
}
