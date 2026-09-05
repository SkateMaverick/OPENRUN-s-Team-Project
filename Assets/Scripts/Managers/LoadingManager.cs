using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private TextMeshProUGUI progressText;

    [Header("Random Background")]
    [SerializeField] private Sprite[] loadingBackgrounds;

    [Header("Settings")]
    [SerializeField] private float minimumLoadingTime = 1.0f;

    [Header("Fallback")]
    [SerializeField] private string fallbackSceneName = "StoryScene";

    private void Start()
    {
        SetRandomBackground();

        string nextScene = SceneLoader.NextSceneName;

        if (string.IsNullOrEmpty(nextScene))
        {
            Debug.LogWarning("SceneLoader.NextSceneName이 비어 있습니다. fallbackSceneName으로 이동합니다: " + fallbackSceneName);
            nextScene = fallbackSceneName;
        }

        Debug.Log("LoadingScene에서 로드할 씬: " + nextScene);

        StartCoroutine(LoadSceneAsync(nextScene));
    }

    private void SetRandomBackground()
    {
        if (backgroundImage == null)
        {
            Debug.LogWarning("Background Image가 연결되지 않았습니다.");
            return;
        }

        if (loadingBackgrounds == null || loadingBackgrounds.Length == 0)
        {
            Debug.LogWarning("Loading Backgrounds가 비어 있습니다.");
            return;
        }

        int randomIndex = Random.Range(0, loadingBackgrounds.Length);
        backgroundImage.sprite = loadingBackgrounds[randomIndex];
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        if (loadingText != null)
            loadingText.text = "숲의 기억을 불러오는 중...";

        if (loadingSlider != null)
            loadingSlider.value = 0f;

        if (progressText != null)
            progressText.text = "0%";

        float elapsedTime = 0f;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        if (operation == null)
        {
            Debug.LogError("씬을 로드할 수 없습니다. Build Profiles에 씬이 등록되어 있는지 확인하세요: " + sceneName);
            yield break;
        }

        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            elapsedTime += Time.deltaTime;

            float sceneProgress = Mathf.Clamp01(operation.progress / 0.9f);
            float timeProgress = Mathf.Clamp01(elapsedTime / minimumLoadingTime);
            float displayProgress = Mathf.Min(sceneProgress, timeProgress);

            if (loadingSlider != null)
                loadingSlider.value = displayProgress;

            if (progressText != null)
                progressText.text = $"{displayProgress * 100f:0}%";

            if (sceneProgress >= 1f && elapsedTime >= minimumLoadingTime)
            {
                if (loadingSlider != null)
                    loadingSlider.value = 1f;

                if (progressText != null)
                    progressText.text = "100%";

                yield return new WaitForSeconds(0.2f);

                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}