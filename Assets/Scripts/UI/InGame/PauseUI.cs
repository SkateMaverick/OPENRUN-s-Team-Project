using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    public static PauseUI Instance { get; private set; }

    [Header("UI Panels & Buttons")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameOptionsUI optionsUI;

    [Header("Scene Navigation")]
    [SerializeField] private string homeSceneName = "Menu";
    [SerializeField] private string loadingSceneName = "LoadingScene";
    [SerializeField] private bool useLoadingScene = true;

    private const string UI_KEY = "PauseUI";

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
    }

    private void Start()
    {
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

    public void OpenOptions()
    {
        if (optionsUI != null)
        {
            optionsUI.Open();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
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
            if (UIStateManager.Instance != null)
            {
                UIStateManager.Instance.CloseUI(UI_KEY);
            }
        }
    }
}
