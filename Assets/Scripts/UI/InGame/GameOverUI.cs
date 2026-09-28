using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance { get; private set; }

    [Header("UI Panels & References")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private UnityEngine.UI.Image bannerImage;
    [SerializeField] private UnityEngine.UI.Button continueButton;
    [SerializeField] private UnityEngine.UI.Button exitButton;
    [SerializeField] private TextMeshProUGUI continueText;
    [SerializeField] private TextMeshProUGUI exitText;

    [Header("HUD Elements to Hide/Fade")]
    [SerializeField] private GameObject topMenuBar;
    [SerializeField] private GameObject partySidebar;
    [SerializeField] private GameObject playerHUD;

    [Header("Scene Navigation")]
    [SerializeField] private string homeSceneName = "Menu";
    [SerializeField] private string loadingSceneName = "LoadingScene";
    [SerializeField] private bool useLoadingScene = true;

    private const string UI_KEY = "GameOverUI";
    public bool IsGameOver { get; private set; }

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

        EnsureHUDReferences();
    }

    private void Start()
    {
        EnsureHUDReferences();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(RestartGame);
            continueButton.onClick.AddListener(RestartGame);
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(ExitGame);
            exitButton.onClick.AddListener(ExitGame);
        }
    }

    private void OnEnable()
    {
        PlayerHealth.OnPlayerDied -= ShowGameOver;
        PlayerHealth.OnPlayerDied += ShowGameOver;
    }

    private void OnDisable()
    {
        PlayerHealth.OnPlayerDied -= ShowGameOver;

        if (IsGameOver)
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

        PlayerHealth.OnPlayerDied -= ShowGameOver;

        if (IsGameOver)
        {
            Time.timeScale = 1f;
            if (UIStateManager.Instance != null)
            {
                UIStateManager.Instance.CloseUI(UI_KEY);
            }
        }
    }

    private void Update()
    {
        if (!IsGameOver) return;

        bool confirmPressed = false;
        bool cancelPressed = false;

        if (Keyboard.current != null)
        {
            confirmPressed = Keyboard.current.enterKey.wasPressedThisFrame ||
                             Keyboard.current.numpadEnterKey.wasPressedThisFrame ||
                             Keyboard.current.spaceKey.wasPressedThisFrame;
            cancelPressed = Keyboard.current.escapeKey.wasPressedThisFrame;
        }
        else
        {
            confirmPressed = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space);
            cancelPressed = Input.GetKeyDown(KeyCode.Escape);
        }

        if (confirmPressed)
        {
            RestartGame();
        }
        else if (cancelPressed)
        {
            ExitGame();
        }
    }

    private void EnsureHUDReferences()
    {
        if (transform.parent != null)
        {
            if (topMenuBar == null)
            {
                var found = transform.parent.Find("TopMenuBar");
                if (found != null) topMenuBar = found.gameObject;
            }

            if (partySidebar == null)
            {
                var found = transform.parent.Find("PartySidebar");
                if (found != null) partySidebar = found.gameObject;
            }

            if (playerHUD == null)
            {
                var found = transform.parent.Find("PlayerHealthBarHUD");
                if (found != null) playerHUD = found.gameObject;
            }
        }
    }

    public void ShowGameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;

        EnsureHUDReferences();

        if (topMenuBar != null) topMenuBar.SetActive(false);
        if (partySidebar != null) partySidebar.SetActive(false);
        if (playerHUD != null) playerHUD.SetActive(false);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;

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

    public void RestartGame()
    {
        Time.timeScale = 1f;
        IsGameOver = false;

        if (UIStateManager.Instance != null)
        {
            UIStateManager.Instance.CloseUI(UI_KEY);
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

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

    public void ExitGame()
    {
        Time.timeScale = 1f;
        IsGameOver = false;

        if (UIStateManager.Instance != null)
        {
            UIStateManager.Instance.CloseUI(UI_KEY);
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
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
