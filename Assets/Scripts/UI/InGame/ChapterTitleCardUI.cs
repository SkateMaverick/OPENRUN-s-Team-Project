using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Witcher 3 style cinematic chapter title card that fades in, displays over the live gameplay screen, and fades out.
/// </summary>
public class ChapterTitleCardUI : MonoBehaviour
{
    private static ChapterTitleCardUI _instance;
    public static ChapterTitleCardUI Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Object.FindFirstObjectByType<ChapterTitleCardUI>(FindObjectsInactive.Include);
            }
            return _instance;
        }
        private set => _instance = value;
    }

    [Header("UI References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform contentTransform;
    [SerializeField] private TextMeshProUGUI chapterText;
    [SerializeField] private Image dividerLine;
    [SerializeField] private TextMeshProUGUI subtitleText;

    [Header("Default Content")]
    [SerializeField] private string defaultChapter = "Chapter 1";
    [SerializeField] private string defaultSubtitle = "숲속의 비밀";

    [Header("Timing Settings")]
    [Tooltip("Delay in seconds after trigger before title begins fading in.")]
    [SerializeField] private float startDelay = 0.35f;

    [Tooltip("Duration in seconds for fade-in transition.")]
    [SerializeField] private float fadeInDuration = 1.2f;

    [Tooltip("Duration in seconds the title remains fully visible.")]
    [SerializeField] private float displayDuration = 3.0f;

    [Tooltip("Duration in seconds for fade-out transition.")]
    [SerializeField] private float fadeOutDuration = 1.8f;

    [Header("Cinematic Subtle Scale Animation")]
    [Tooltip("Whether to slowly expand/zoom the title card while visible for cinematic polish.")]
    [SerializeField] private bool enableSubtleZoom = true;
    [SerializeField] private float initialScale = 0.96f;
    [SerializeField] private float targetScale = 1.03f;

    [Header("Audio (Optional)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip titleSound;

    [Header("Debug")]
    [SerializeField] private bool enableDebugKey = true;
#if ENABLE_INPUT_SYSTEM
    [SerializeField] private Key debugKey = Key.F7;
#endif

    [Header("Chapter Ending Black Screen")]
    [SerializeField] private Image blackOverlay;
    [SerializeField] private TextMeshProUGUI endingText;

    [Header("Main Menu Transition")]
    [SerializeField] private string homeSceneName = "Menu";
    [SerializeField] private string loadingSceneName = "LoadingScene";
    [SerializeField] private bool useLoadingScene = true;

    private Coroutine _playCoroutine;
    private bool _hasTriggeredOnFirstIntro = false;
    private bool _isEndingActive = false;

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

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        if (contentTransform == null && transform.childCount > 0)
        {
            contentTransform = transform.GetChild(0) as RectTransform;
        }
    }

    private void Start()
    {
        // Auto-subscribe to IntroWalkCutsceneController if available
        if (IntroWalkCutsceneController.Instance != null)
        {
            IntroWalkCutsceneController.Instance.onCutsceneComplete.RemoveListener(OnIntroCutsceneComplete);
            IntroWalkCutsceneController.Instance.onCutsceneComplete.AddListener(OnIntroCutsceneComplete);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        if (IntroWalkCutsceneController.Instance != null)
        {
            IntroWalkCutsceneController.Instance.onCutsceneComplete.RemoveListener(OnIntroCutsceneComplete);
        }
    }

    private void Update()
    {
        if (_isEndingActive)
            return;

        if (enableDebugKey)
        {
            bool pressed = false;
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current[debugKey].wasPressedThisFrame)
            {
                pressed = true;
            }
#endif
            try
            {
                if (Input.GetKeyDown(KeyCode.F7))
                {
                    pressed = true;
                }
            }
            catch { }

            if (pressed)
            {
                PlayChapterIntro();
            }
        }
    }

    public void OnIntroCutsceneComplete()
    {
        if (_hasTriggeredOnFirstIntro || _isEndingActive) return;
        if (IntroWalkCutsceneController.Instance != null && IntroWalkCutsceneController.Instance.CheckIsDungeonReturn()) return;
        _hasTriggeredOnFirstIntro = true;

        PlayChapterIntro();
    }

    public void PlayChapterIntro()
    {
        if (_isEndingActive) return;
        PlayTitle(defaultChapter, defaultSubtitle, displayDuration);
    }

    public void PlayTitle(string chapter, string subtitle, float duration = 3.0f)
    {
        if (_isEndingActive) return;

        if (_playCoroutine != null)
        {
            StopCoroutine(_playCoroutine);
        }

        _playCoroutine = StartCoroutine(PlayTitleRoutine(chapter, subtitle, duration));
    }

    private IEnumerator PlayTitleRoutine(string chapter, string subtitle, float duration)
    {
        if (chapterText != null) chapterText.text = chapter;
        if (subtitleText != null) subtitleText.text = subtitle;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        if (contentTransform != null)
        {
            contentTransform.localScale = Vector3.one * (enableSubtleZoom ? initialScale : 1f);
        }

        if (startDelay > 0.001f)
        {
            yield return new WaitForSeconds(startDelay);
        }

        if (audioSource != null && titleSound != null)
        {
            audioSource.PlayOneShot(titleSound);
        }

        float totalDuration = fadeInDuration + duration + fadeOutDuration;
        float elapsed = 0f;

        // Animate Fade In
        float fadeTimer = 0f;
        while (fadeTimer < fadeInDuration)
        {
            fadeTimer += Time.unscaledDeltaTime;
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(fadeTimer / fadeInDuration);

            // Smooth ease out for fade in
            float alpha = Mathf.SmoothStep(0f, 1f, t);
            if (canvasGroup != null) canvasGroup.alpha = alpha;

            if (enableSubtleZoom && contentTransform != null)
            {
                float overallT = Mathf.Clamp01(elapsed / totalDuration);
                contentTransform.localScale = Vector3.one * Mathf.Lerp(initialScale, targetScale, overallT);
            }

            yield return null;
        }

        if (canvasGroup != null) canvasGroup.alpha = 1f;

        // Stay on screen
        float stayTimer = 0f;
        while (stayTimer < duration)
        {
            stayTimer += Time.unscaledDeltaTime;
            elapsed += Time.unscaledDeltaTime;

            if (enableSubtleZoom && contentTransform != null)
            {
                float overallT = Mathf.Clamp01(elapsed / totalDuration);
                contentTransform.localScale = Vector3.one * Mathf.Lerp(initialScale, targetScale, overallT);
            }

            yield return null;
        }

        // Animate Fade Out (similar to Witcher 3 dissolution in Item 2)
        fadeTimer = 0f;
        while (fadeTimer < fadeOutDuration)
        {
            fadeTimer += Time.unscaledDeltaTime;
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(fadeTimer / fadeOutDuration);

            // Smooth ease in-out for fade out
            float alpha = Mathf.SmoothStep(1f, 0f, t);
            if (canvasGroup != null) canvasGroup.alpha = alpha;

            if (enableSubtleZoom && contentTransform != null)
            {
                float overallT = Mathf.Clamp01(elapsed / totalDuration);
                contentTransform.localScale = Vector3.one * Mathf.Lerp(initialScale, targetScale, overallT);
            }

            yield return null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        _playCoroutine = null;
    }

    /// <summary>
    /// Fades the screen completely to black, then displays the ending text (e.g. 'Chapter 2 에서 계속...').
    /// Screen stays black and waits for a Left Click to return to the Main Menu.
    /// </summary>
    public void PlayBlackFadeEnding(string text = "Chapter 2 에서 계속...", System.Action onComplete = null)
    {
        Debug.Log("[ChapterTitleCardUI] PlayBlackFadeEnding called with text: " + text);
        _isEndingActive = true;

        if (_playCoroutine != null)
        {
            StopCoroutine(_playCoroutine);
        }

        _playCoroutine = StartCoroutine(BlackFadeEndingRoutine(text, onComplete));
    }

    private IEnumerator BlackFadeEndingRoutine(string text, System.Action onComplete)
    {
        Debug.Log("[ChapterTitleCardUI] BlackFadeEndingRoutine started!");
        _isEndingActive = true;

        // Bring to front in canvas hierarchy so nothing covers it
        transform.SetAsLastSibling();

        // 1. Hide the normal chapter title card contents
        if (contentTransform != null)
        {
            contentTransform.gameObject.SetActive(false);
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }

        // 2. Ensure black overlay and ending text are present and initialized
        if (blackOverlay == null)
        {
            var overlayGo = new GameObject("BlackOverlay");
            overlayGo.transform.SetParent(transform, false);
            overlayGo.transform.SetAsFirstSibling();
            var rt = overlayGo.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            blackOverlay = overlayGo.AddComponent<Image>();
            blackOverlay.color = new Color(0f, 0f, 0f, 0f);
            blackOverlay.raycastTarget = true;
        }

        if (endingText == null)
        {
            var textGo = new GameObject("EndingText");
            textGo.transform.SetParent(transform, false);
            var rt = textGo.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            endingText = textGo.AddComponent<TextMeshProUGUI>();
            if (subtitleText != null)
            {
                endingText.font = subtitleText.font;
            }
            endingText.fontSize = 38;
            endingText.alignment = TextAlignmentOptions.Center;
            endingText.color = new Color(1f, 1f, 1f, 0f);
            endingText.raycastTarget = false;
        }

        blackOverlay.gameObject.SetActive(true);
        endingText.gameObject.SetActive(true);
        endingText.text = text;

        // Set initial state
        blackOverlay.color = new Color(0f, 0f, 0f, 0f);
        endingText.color = new Color(0.95f, 0.95f, 0.96f, 0f);

        // 3. Fade screen to black
        float fadeToBlackDuration = 1.5f;
        float elapsed = 0f;
        while (elapsed < fadeToBlackDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeToBlackDuration);
            float alpha = Mathf.SmoothStep(0f, 1f, t);
            blackOverlay.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
        blackOverlay.color = Color.black;

        // Brief delay in black
        yield return new WaitForSecondsRealtime(0.4f);

        // 4. Fade in the ending text
        float textFadeInDuration = 1.2f;
        elapsed = 0f;
        while (elapsed < textFadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / textFadeInDuration);
            float alpha = Mathf.SmoothStep(0f, 1f, t);
            endingText.color = new Color(0.95f, 0.95f, 0.96f, alpha);
            yield return null;
        }
        endingText.color = new Color(0.95f, 0.95f, 0.96f, 1f);

        // Ensure cursor is visible and free
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        onComplete?.Invoke();

        // 5. Wait for Left Click (or Space/Enter) to return to main menu
        // Small debounce delay so input from previous dialogue doesn't skip immediately
        yield return new WaitForSecondsRealtime(0.25f);

        while (!CheckLeftClickInput())
        {
            yield return null;
        }

        // 6. Transition to Main Menu
        ReturnToMainMenu();
    }

    private bool CheckLeftClickInput()
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;
        if (Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame))
            return true;
#endif
        try
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
                return true;
        }
        catch { }

        return false;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Reset gameplay flags for future playthroughs
        IntroWalkCutsceneController.HasPlayedFirstIntro = false;
        CrystalPickup.IsCrystalAcquired = false;

        if (InventoryManager.Instance != null)
        {
            Destroy(InventoryManager.Instance.gameObject);
        }

        if (useLoadingScene && !string.IsNullOrEmpty(loadingSceneName))
        {
            SceneLoader.NextSceneName = homeSceneName;
            SceneManager.LoadScene(loadingSceneName);
        }
        else
        {
            SceneManager.LoadScene(homeSceneName);
        }
    }
}
