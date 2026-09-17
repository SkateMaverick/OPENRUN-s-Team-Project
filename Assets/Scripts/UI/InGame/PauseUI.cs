using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseUI : MonoBehaviour
{
    public static PauseUI Instance { get; private set; }

    [Header("UI Panels & Buttons")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject topMenuBar;
    [SerializeField] private GameObject partySidebar;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameOptionsUI optionsUI;

    [Header("Party Sidebar Pause Visuals")]
    [Range(0f, 1f)]
    [SerializeField] private float pausedSidebarAlpha = 0.2f;

    [Header("Scene Navigation")]
    [SerializeField] private string homeSceneName = "Menu";
    [SerializeField] private string loadingSceneName = "LoadingScene";
    [SerializeField] private bool useLoadingScene = true;

    private const string UI_KEY = "PauseUI";
    private float _lastToggleRealtime = -1f;
    private const float TOGGLE_COOLDOWN = 0.25f;

    public bool IsPaused { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
            return;
        }

        EnsureReferences();
    }

    private void Start()
    {
        EnsureReferences();

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveListener(ResumeGame);
            resumeButton.onClick.AddListener(ResumeGame);
        }

        if (optionsButton != null)
        {
            optionsButton.onClick.RemoveListener(OpenOptions);
            optionsButton.onClick.AddListener(OpenOptions);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartGame);
            restartButton.onClick.AddListener(RestartGame);
        }

        if (homeButton != null)
        {
            homeButton.onClick.RemoveListener(HomeGame);
            homeButton.onClick.AddListener(HomeGame);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(ResumeGame);
            closeButton.onClick.AddListener(ResumeGame);
        }
    }

    private void EnsureReferences()
    {
        EnsureTopMenuBar();
        EnsurePartySidebar();
    }

    private void EnsurePartySidebar()
    {
        if (partySidebar == null)
        {
            if (transform.parent != null)
            {
                var found = transform.parent.Find("PartySidebar");
                if (found != null)
                {
                    partySidebar = found.gameObject;
                }
            }

            if (partySidebar == null)
            {
                var found = GameObject.Find("PartySidebar");
                if (found != null)
                {
                    partySidebar = found;
                }
            }
        }
    }

    private void SetPartySidebarBlurred(bool blurred)
    {
        EnsurePartySidebar();
        if (partySidebar == null) return;

        var cg = partySidebar.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = partySidebar.AddComponent<CanvasGroup>();
        }

        if (blurred)
        {
            cg.alpha = pausedSidebarAlpha;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
        else
        {
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }
    }

    private void EnsureTopMenuBar()
    {
        if (topMenuBar == null)
        {
            if (transform.parent != null)
            {
                var found = transform.parent.Find("TopMenuBar");
                if (found != null)
                {
                    topMenuBar = found.gameObject;
                }
            }

            if (topMenuBar == null)
            {
                var found = GameObject.Find("TopMenuBar");
                if (found != null)
                {
                    topMenuBar = found;
                }
            }
        }
    }

    public void OpenOptions()
    {
        if (optionsUI != null)
        {
            optionsUI.Open();
        }
    }

    private void Update()
    {
        bool escapePressed = false;
        if (Keyboard.current != null)
        {
            escapePressed = Keyboard.current.escapeKey.wasPressedThisFrame;
        }
        else
        {
            escapePressed = Input.GetKeyDown(KeyCode.Escape);
        }

        if (escapePressed)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (Time.unscaledTime - _lastToggleRealtime < TOGGLE_COOLDOWN)
        {
            return;
        }
        _lastToggleRealtime = Time.unscaledTime;

        if (IsPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;

        EnsureTopMenuBar();
        if (topMenuBar != null)
        {
            topMenuBar.SetActive(false);
        }

        SetPartySidebarBlurred(true);

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        if (UIStateManager.Instance != null)
        {
            UIStateManager.Instance.OpenUI(UI_KEY);
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (optionsUI != null)
        {
            optionsUI.Close();
        }

        EnsureTopMenuBar();
        if (topMenuBar != null)
        {
            topMenuBar.SetActive(true);
        }

        SetPartySidebarBlurred(false);

        if (UIStateManager.Instance != null)
        {
            UIStateManager.Instance.CloseUI(UI_KEY);
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        EnsureTopMenuBar();
        if (topMenuBar != null)
        {
            topMenuBar.SetActive(true);
        }

        SetPartySidebarBlurred(false);

        if (UIStateManager.Instance != null)
        {
            UIStateManager.Instance.CloseUI(UI_KEY);
        }

        IsPaused = false;

        string currentScene = SceneManager.GetActiveScene().name;
        if (useLoadingScene && !string.IsNullOrEmpty(loadingSceneName))
        {
            SceneLoader.NextSceneName = currentScene;
            SceneManager.LoadScene(loadingSceneName);
        }
        else
        {
            SceneManager.LoadScene(currentScene);
        }
    }

    public void HomeGame()
    {
        Time.timeScale = 1f;

        EnsureTopMenuBar();
        if (topMenuBar != null)
        {
            topMenuBar.SetActive(true);
        }

        SetPartySidebarBlurred(false);

        if (UIStateManager.Instance != null)
        {
            UIStateManager.Instance.CloseUI(UI_KEY);
        }

        IsPaused = false;

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

    private void OnDisable()
    {
        if (IsPaused)
        {
            Time.timeScale = 1f;
            EnsureTopMenuBar();
            if (topMenuBar != null)
            {
                topMenuBar.SetActive(true);
            }
            SetPartySidebarBlurred(false);
            if (UIStateManager.Instance != null)
            {
                UIStateManager.Instance.CloseUI(UI_KEY);
            }
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        if (IsPaused)
        {
            Time.timeScale = 1f;
            EnsureTopMenuBar();
            if (topMenuBar != null)
            {
                topMenuBar.SetActive(true);
            }
            SetPartySidebarBlurred(false);
            if (UIStateManager.Instance != null)
            {
                UIStateManager.Instance.CloseUI(UI_KEY);
            }
        }
    }
}
