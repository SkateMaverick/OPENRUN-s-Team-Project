using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;

public class DialogueCutsceneManager : MonoBehaviour
{
    public static DialogueCutsceneManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private CanvasGroup dialogueCanvasGroup;
    [SerializeField] private GameObject speakerContainer;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private Image speakerDividerLine;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private RectTransform nextIndicator;
    [SerializeField] private Button screenClickCatcher;

    [Header("Auto Mode UI")]
    [SerializeField] private Button autoPlayButton;
    [SerializeField] private TextMeshProUGUI autoPlayText;
    [SerializeField] private TextMeshProUGUI autoPlayIconTmp;
    [SerializeField] private Image autoPlayIcon;
    [SerializeField] private Color autoActiveColor = new Color(1.0f, 0.79f, 0.22f, 1.0f); // #FFCA38
    [SerializeField] private Color autoInactiveColor = new Color(0.85f, 0.85f, 0.85f, 0.8f);

    [Header("Debug / Testing")]
    [SerializeField] private bool enableDebugHotkey = true;
    [SerializeField] private Key debugKey = Key.F5;
    [SerializeField] private DialogueSequence testSequence;

    [Header("Typewriter Settings")]
    [Tooltip("Characters revealed per second.")]
    [SerializeField] private float charactersPerSecond = 35f;
    [SerializeField] private bool enableTypewriter = true;

    [Header("Auto Play Settings")]
    [SerializeField] private bool autoPlayEnabled = false;
    [SerializeField] private float baseAutoDelay = 1.6f;
    [SerializeField] private float perCharacterAutoDelay = 0.02f;

    [Header("HUD Elements to Hide During Cutscene")]
    [SerializeField] private GameObject topMenuBar;
    [SerializeField] private GameObject partySidebar;
    [SerializeField] private GameObject playerHUD;
    [SerializeField] private GameObject minimapFrame;
    [SerializeField] private GameObject quickPotionSlot;

    [Header("Audio (Optional)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip nextLineSound;

    [Header("Events")]
    public UnityEvent onDialogueStart;
    public UnityEvent<int> onDialogueLineChanged;
    public UnityEvent onDialogueEnd;

    // Runtime state
    private const string UI_KEY = "DialogueCutscene";
    private readonly List<DialogueLine> _currentLines = new List<DialogueLine>();
    private int _currentLineIndex = -1;
    private Coroutine _typewriterCoroutine;
    private Coroutine _autoAdvanceCoroutine;
    private bool _isTyping = false;
    private bool _isDialogueActive = false;
    private int _lastInputFrame = -1;
    private float _dialogueStartTime = 0f;
    private Action _onDialogueCompleteCallback;

    // Next indicator animation
    private Vector2 _indicatorInitialPos;
    private Coroutine _indicatorAnimCoroutine;

    public bool IsDialogueActive => _isDialogueActive;
    public bool IsAutoPlay => autoPlayEnabled;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (nextIndicator != null)
        {
            _indicatorInitialPos = nextIndicator.anchoredPosition;
        }

        if (screenClickCatcher != null)
        {
            screenClickCatcher.onClick.RemoveListener(OnScreenClick);
            screenClickCatcher.onClick.AddListener(OnScreenClick);
        }

        if (autoPlayButton != null)
        {
            autoPlayButton.onClick.RemoveListener(ToggleAutoPlay);
            autoPlayButton.onClick.AddListener(ToggleAutoPlay);
        }

        UpdateAutoUI();
    }

    private void Start()
    {
        FindHUDReferencesIfNull();

        if (dialoguePanel != null && !_isDialogueActive)
        {
            dialoguePanel.SetActive(false);
        }
    }

    private void Update()
    {
        // Debug test hotkey (F5 key to trigger test dialogue cutscene)
        if (enableDebugHotkey && !_isDialogueActive)
        {
            bool debugHotkeyPressed = false;
            if (Keyboard.current != null && Keyboard.current[debugKey].wasPressedThisFrame)
                debugHotkeyPressed = true;
            else
            {
                try { if (Input.GetKeyDown(KeyCode.F5)) debugHotkeyPressed = true; } catch { }
            }

            if (debugHotkeyPressed)
            {
                if (testSequence != null)
                {
                    StartDialogue(testSequence);
                }
                else
                {
                    TestGenshinDialogue();
                }
            }
        }

        if (!_isDialogueActive)
            return;

        // Animate next prompt indicator (gentle bobbing)
        if (nextIndicator != null && nextIndicator.gameObject.activeSelf)
        {
            float bobOffset = Mathf.Sin(Time.unscaledTime * 5f) * 3f;
            nextIndicator.anchoredPosition = new Vector2(_indicatorInitialPos.x, _indicatorInitialPos.y + bobOffset);
        }

        // Check Spacebar or Left Mouse click inputs
        if (Time.unscaledTime >= _dialogueStartTime + 0.1f)
        {
            if (CheckAdvanceInput())
            {
                AdvanceOrComplete();
            }
        }
    }

    private bool CheckAdvanceInput()
    {
        if (Time.frameCount == _lastInputFrame)
            return false;

        // New Input System
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            return true;
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        // Legacy Input
        try
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                return true;
        }
        catch { }

        return false;
    }

    private void OnScreenClick()
    {
        if (!_isDialogueActive)
            return;

        if (Time.frameCount == _lastInputFrame)
            return;

        AdvanceOrComplete();
    }

    public void AdvanceOrComplete()
    {
        _lastInputFrame = Time.frameCount;

        if (_isTyping)
        {
            // Skip typing and show full line immediately
            CompleteTyping();
        }
        else
        {
            // Advance to next line
            NextDialogue();
        }
    }

    public void StartDialogue(DialogueSequence sequence, Action onComplete = null)
    {
        if (sequence == null || sequence.lines == null || sequence.lines.Count == 0)
        {
            Debug.LogWarning("[DialogueCutsceneManager] Given dialogue sequence is null or empty!");
            return;
        }

        StartDialogue(sequence.lines, onComplete);
    }

    public void StartDialogue(IEnumerable<DialogueLine> lines, Action onComplete = null)
    {
        if (lines == null)
            return;

        _currentLines.Clear();
        _currentLines.AddRange(lines);

        if (_currentLines.Count == 0)
            return;

        _onDialogueCompleteCallback = onComplete;
        _currentLineIndex = -1;
        _isDialogueActive = true;
        _dialogueStartTime = Time.unscaledTime;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        if (dialogueCanvasGroup != null)
        {
            dialogueCanvasGroup.alpha = 1f;
            dialogueCanvasGroup.interactable = true;
            dialogueCanvasGroup.blocksRaycasts = true;
        }

        SetHUDVisibility(false);

        if (UIStateManager.Instance != null)
        {
            UIStateManager.Instance.OpenUI(UI_KEY);
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        onDialogueStart?.Invoke();
        NextDialogue();
    }

    public void NextDialogue()
    {
        StopAutoAdvance();

        _currentLineIndex++;
        if (_currentLineIndex >= _currentLines.Count)
        {
            EndDialogue();
            return;
        }

        DisplayLine(_currentLines[_currentLineIndex]);
        onDialogueLineChanged?.Invoke(_currentLineIndex);
    }

    private void DisplayLine(DialogueLine line)
    {
        if (line == null)
            return;

        // Speaker Name
        bool hasSpeaker = !string.IsNullOrEmpty(line.speakerName);
        if (speakerContainer != null)
        {
            speakerContainer.SetActive(hasSpeaker);
        }

        if (speakerNameText != null)
        {
            speakerNameText.gameObject.SetActive(hasSpeaker);
            speakerNameText.text = line.speakerName;
            speakerNameText.color = line.speakerColor;
        }

        if (speakerDividerLine != null)
        {
            speakerDividerLine.gameObject.SetActive(hasSpeaker);
            Color c = line.speakerColor;
            c.a = 0.5f;
            speakerDividerLine.color = c;
        }

        // Voice clip
        if (line.voiceClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(line.voiceClip);
        }
        else if (nextLineSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(nextLineSound);
        }

        // Hide next prompt indicator during typing
        if (nextIndicator != null)
        {
            nextIndicator.gameObject.SetActive(false);
        }

        // Typewriter text
        if (_typewriterCoroutine != null)
        {
            StopCoroutine(_typewriterCoroutine);
        }

        if (enableTypewriter && charactersPerSecond > 0)
        {
            _typewriterCoroutine = StartCoroutine(TypewriterRoutine(line.dialogueText));
        }
        else
        {
            dialogueText.text = line.dialogueText;
            dialogueText.maxVisibleCharacters = line.dialogueText.Length;
            OnTypingFinished();
        }
    }

    private IEnumerator TypewriterRoutine(string fullText)
    {
        _isTyping = true;
        dialogueText.text = fullText;
        dialogueText.maxVisibleCharacters = 0;
        dialogueText.ForceMeshUpdate();

        int totalChars = dialogueText.textInfo.characterCount;
        float delayPerChar = 1f / Mathf.Max(1f, charactersPerSecond);
        int visibleCount = 0;

        while (visibleCount < totalChars)
        {
            visibleCount++;
            dialogueText.maxVisibleCharacters = visibleCount;
            yield return new WaitForSecondsRealtime(delayPerChar);
        }

        CompleteTyping();
    }

    private void CompleteTyping()
    {
        if (_typewriterCoroutine != null)
        {
            StopCoroutine(_typewriterCoroutine);
            _typewriterCoroutine = null;
        }

        if (_currentLineIndex >= 0 && _currentLineIndex < _currentLines.Count)
        {
            dialogueText.text = _currentLines[_currentLineIndex].dialogueText;
            dialogueText.ForceMeshUpdate();
            dialogueText.maxVisibleCharacters = dialogueText.textInfo.characterCount;
        }

        _isTyping = false;
        OnTypingFinished();
    }

    private void OnTypingFinished()
    {
        _isTyping = false;

        if (nextIndicator != null)
        {
            nextIndicator.gameObject.SetActive(true);
        }

        if (autoPlayEnabled)
        {
            StartAutoAdvance();
        }
    }

    private void StartAutoAdvance()
    {
        StopAutoAdvance();
        string currentText = dialogueText != null ? dialogueText.text : "";
        float delay = baseAutoDelay + (currentText.Length * perCharacterAutoDelay);
        _autoAdvanceCoroutine = StartCoroutine(AutoAdvanceRoutine(delay));
    }

    private void StopAutoAdvance()
    {
        if (_autoAdvanceCoroutine != null)
        {
            StopCoroutine(_autoAdvanceCoroutine);
            _autoAdvanceCoroutine = null;
        }
    }

    private IEnumerator AutoAdvanceRoutine(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (_isDialogueActive && !this._isTyping)
        {
            NextDialogue();
        }
    }

    public void ToggleAutoPlay()
    {
        autoPlayEnabled = !autoPlayEnabled;
        UpdateAutoUI();

        if (autoPlayEnabled && !_isTyping && _isDialogueActive)
        {
            StartAutoAdvance();
        }
        else if (!autoPlayEnabled)
        {
            StopAutoAdvance();
        }
    }

    private void UpdateAutoUI()
    {
        Color targetColor = autoPlayEnabled ? autoActiveColor : autoInactiveColor;

        if (autoPlayText != null)
        {
            autoPlayText.color = targetColor;
        }

        if (autoPlayIconTmp != null)
        {
            autoPlayIconTmp.color = targetColor;
        }

        if (autoPlayIcon != null)
        {
            autoPlayIcon.color = targetColor;
        }
    }

    public void EndDialogue()
    {
        if (!_isDialogueActive)
            return;

        _isDialogueActive = false;
        _isTyping = false;

        if (_typewriterCoroutine != null)
        {
            StopCoroutine(_typewriterCoroutine);
            _typewriterCoroutine = null;
        }

        StopAutoAdvance();

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        SetHUDVisibility(true);

        if (UIStateManager.Instance != null)
        {
            UIStateManager.Instance.CloseUI(UI_KEY);
        }

        onDialogueEnd?.Invoke();

        var callback = _onDialogueCompleteCallback;
        _onDialogueCompleteCallback = null;
        callback?.Invoke();
    }

    private void SetHUDVisibility(bool visible)
    {
        if (topMenuBar != null) topMenuBar.SetActive(visible);
        if (partySidebar != null) partySidebar.SetActive(visible);
        if (playerHUD != null) playerHUD.SetActive(visible);
        if (minimapFrame != null) minimapFrame.SetActive(visible);
        if (quickPotionSlot != null) quickPotionSlot.SetActive(visible);
    }

    private void FindHUDReferencesIfNull()
    {
        if (topMenuBar == null)
            topMenuBar = GameObject.Find("UIPlatformSwitcher/CanvasDesktop/TopMenuBar");
        if (partySidebar == null)
            partySidebar = GameObject.Find("UIPlatformSwitcher/CanvasDesktop/PartySidebar");
        if (playerHUD == null)
            playerHUD = GameObject.Find("UIPlatformSwitcher/CanvasDesktop/PlayerHealthBarHUD");
        if (minimapFrame == null)
            minimapFrame = GameObject.Find("UIPlatformSwitcher/CanvasDesktop/MinimapFrame");
        if (quickPotionSlot == null)
            quickPotionSlot = GameObject.Find("UIPlatformSwitcher/CanvasDesktop/QuickPotionSlot");
    }

    // Context menu helper for editor testing
    [ContextMenu("Test Genshin Dialogue")]
    public void TestGenshinDialogue()
    {
        List<DialogueLine> testLines = new List<DialogueLine>
        {
            new DialogueLine("Paimon", "Cool! Suddenly, Paimon doesn't feel so bad about taking Teucer's money!", new Color(1.0f, 0.79f, 0.22f)),
            new DialogueLine("Traveler", "Hey, don't say that! We should be more considerate.", new Color(0.9f, 0.9f, 0.95f)),
            new DialogueLine("Paimon", "Hehe, Paimon was just kidding! Let's get going, there's an adventure waiting for us!", new Color(1.0f, 0.79f, 0.22f))
        };

        StartDialogue(testLines, () =>
        {
            Debug.Log("[DialogueCutsceneManager] Test dialogue finished successfully!");
        });
    }
}
