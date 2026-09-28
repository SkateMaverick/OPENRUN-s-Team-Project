using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Witcher 3 style cinematic chapter title card that fades in, displays over the live gameplay screen, and fades out.
/// </summary>
public class ChapterTitleCardUI : MonoBehaviour
{
    public static ChapterTitleCardUI Instance { get; private set; }

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

    private Coroutine _playCoroutine;
    private bool _hasTriggeredOnFirstIntro = false;

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
        if (_hasTriggeredOnFirstIntro) return;
        _hasTriggeredOnFirstIntro = true;

        PlayChapterIntro();
    }

    public void PlayChapterIntro()
    {
        PlayTitle(defaultChapter, defaultSubtitle, displayDuration);
    }

    public void PlayTitle(string chapter, string subtitle, float duration = 3.0f)
    {
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
}
