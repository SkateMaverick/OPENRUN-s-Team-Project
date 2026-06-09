using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StorySceneManager : MonoBehaviour
{
    [System.Serializable]
    public class StoryPage
    {
        public Sprite image;

        [TextArea(2, 5)]
        public string dialogue;
    }

    [Header("Intro Page")]
    [SerializeField] private Sprite introImage;

    [TextArea(2, 5)]
    [SerializeField] private string introDialogue = "Earth Seed & Golem\n어쓰 씨드 앤 골렘";

    [Header("UI")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Story Pages")]
    [SerializeField] private StoryPage[] storyPages;

    [Header("Next Scene")]
    [SerializeField] private string nextSceneName = "Main_SingleTest";

    [Header("Input")]
    [SerializeField] private float inputDelay = 0.2f;

    // -1이면 아직 스토리 Element에 들어가기 전, 즉 인트로 화면
    private int currentPageIndex = -1;
    private float startTime;
    private bool isFinished;

    private void Start()
    {
        startTime = Time.time;
        ShowIntroPage();
    }

    private void Update()
    {
        if (isFinished)
            return;

        if (Time.time < startTime + inputDelay)
            return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            NextPage();
        }
    }

    private void ShowIntroPage()
    {
        currentPageIndex = -1;

        if (backgroundImage != null)
            backgroundImage.sprite = introImage;

        if (dialogueText != null)
            dialogueText.text = introDialogue;
    }

    public void NextPage()
    {
        currentPageIndex++;

        if (currentPageIndex >= storyPages.Length)
        {
            EndStory();
            return;
        }

        ShowPage(currentPageIndex);
    }

    private void ShowPage(int index)
    {
        if (storyPages == null || storyPages.Length == 0)
        {
            EndStory();
            return;
        }

        StoryPage page = storyPages[index];

        if (backgroundImage != null)
            backgroundImage.sprite = page.image;

        if (dialogueText != null)
            dialogueText.text = page.dialogue;
    }

    public void EndStory()
    {
        if (isFinished)
            return;

        isFinished = true;

        SceneLoader.NextSceneName = nextSceneName;
        SceneManager.LoadScene("LoadingScene");
    }
}