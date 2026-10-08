using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static string NextSceneName;

    [Header("Scene Names")]
    [SerializeField] private string loadingSceneName = "LoadingScene";
    [SerializeField] private string storySceneName = "StoryScene";
    [SerializeField] private string singleMainSceneName = "SingleTest";
    [SerializeField] private string menuSceneName = "Menu";

    private void Awake()
    {
        ApplyCursorForMenuOrLoading();
    }

    private void Start()
    {
        ApplyCursorForMenuOrLoading();
    }

    private void ApplyCursorForMenuOrLoading()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == menuSceneName || currentScene == loadingSceneName)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 1f;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void RegisterSceneLoadedCursorHandler()
    {
        SceneManager.sceneLoaded -= OnSceneLoadedCursorHandler;
        SceneManager.sceneLoaded += OnSceneLoadedCursorHandler;
    }

    private static void OnSceneLoadedCursorHandler(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu" || scene.name == "LoadingScene")
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 1f;
        }
    }

    public void LoadStoryScene()
    {
        LoadSceneWithLoading(storySceneName);
    }

    public void LoadSingleMain()
    {
        LoadSceneWithLoading(singleMainSceneName);
    }

    public void LoadMenu()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;
        LoadSceneWithLoading(menuSceneName);
    }

    public void LoadSceneWithLoading(string targetSceneName)
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("이동할 씬 이름이 비어 있습니다.");
            return;
        }

        NextSceneName = targetSceneName;
        Debug.Log("다음 씬 설정: " + NextSceneName);

        SceneManager.LoadScene(loadingSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}