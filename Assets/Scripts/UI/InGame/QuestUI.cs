using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class QuestUI : MonoBehaviour
{
    public static QuestUI Instance { get; private set; }

    [Header("Main Panels")]
    [SerializeField] private GameObject questPanel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button backgroundClickCatcher;
    [SerializeField] private Button closeButton;

    [Header("Input Settings")]
    [SerializeField] private Key toggleKey = Key.Q;
    [SerializeField] private bool enableKeyToggle = true;

    [Header("Top Menu Bar Integration")]
    [SerializeField] private Button topMenuBarQuestButton;

    [Header("Header")]
    [SerializeField] private TextMeshProUGUI headerTitleText;

    [Header("Left Panel (Quest List)")]
    [SerializeField] private Transform questListContent;
    [SerializeField] private GameObject questRowPrefab;
    [SerializeField] private GameObject actHeaderPrefab;

    [Header("Right Panel (Quest Details)")]
    [SerializeField] private TextMeshProUGUI questTitleText;
    [SerializeField] private TextMeshProUGUI locationText;
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private GameObject rewardsContainer;
    [SerializeField] private GameObject[] rewardSlots;
    [SerializeField] private Image[] rewardIcons;
    [SerializeField] private TextMeshProUGUI[] rewardCounts;
    [SerializeField] private Button navigateButton;
    [SerializeField] private TextMeshProUGUI navigateButtonText;
    [SerializeField] private Image navigateButtonIcon;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip navigateSound;

    [Header("HUD Overlap Handling")]
    [SerializeField] private GameObject quickPotionSlot;
    private bool _potionSlotWasActive = false;

    [Header("Main Story Quests")]
    [SerializeField] private List<QuestItemData> quests = new List<QuestItemData>();

    private const string UI_KEY = "QuestUI";
    private readonly List<QuestRowUI> _spawnedRows = new List<QuestRowUI>();
    private readonly List<GameObject> _spawnedHeaderObjs = new List<GameObject>();
    private QuestItemData _selectedQuest;
    private bool _isOpen = false;
    private float _lastToggleTime = -1f;
    private float _distanceUpdateTimer = 0f;

    public bool IsOpen => _isOpen;
    public QuestItemData SelectedQuest => _selectedQuest;
    public List<QuestItemData> AllQuests => quests;

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

        FindReferencesIfNull();

        if (quests == null || quests.Count == 0)
        {
            InitializeDefaultStoryQuests();
        }
        else
        {
            MigrateQuestsToKorean();
        }

        SetupButtonListeners();
    }

    private void Start()
    {
        FindReferencesIfNull();
        WireTopMenuBarButton();

        // Initially closed
        if (questPanel != null)
        {
            questPanel.SetActive(false);
        }
        _isOpen = false;
    }

    private void Update()
    {
        if (!enableKeyToggle)
            return;

        // Toggle on Q
        if (CheckToggleInput())
        {
            if (Time.unscaledTime - _lastToggleTime > 0.2f)
            {
                _lastToggleTime = Time.unscaledTime;
                Toggle();
            }
        }

        // Close on Escape when open
        if (_isOpen)
        {
            _distanceUpdateTimer += Time.unscaledDeltaTime;
            if (_distanceUpdateTimer >= 0.2f)
            {
                _distanceUpdateTimer = 0f;
                UpdateQuestsDistance();
            }

            bool escPressed = false;
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) escPressed = true;
            else { try { if (Input.GetKeyDown(KeyCode.Escape)) escPressed = true; } catch { } }

            if (escPressed)
            {
                Close();
            }
        }
    }

    private bool CheckToggleInput()
    {
        // New Input System
        if (Keyboard.current != null && Keyboard.current[toggleKey].wasPressedThisFrame)
            return true;

        // Legacy Input
        try
        {
            if (Input.GetKeyDown(KeyCode.Q))
                return true;
        }
        catch { }

        return false;
    }

    public void Toggle()
    {
        if (_isOpen)
            Close();
        else
            Open();
    }

    public void Open()
    {
        _isOpen = true;

        HidePotionSlot();

        MigrateQuestsToKorean();
        UpdateQuestsDistance();

        if (questPanel != null)
            questPanel.SetActive(true);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
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

        PlaySound(openSound);

        if (headerTitleText != null)
        {
            headerTitleText.text = "메인 퀘스트";
        }

        // Select first or tracked quest
        if (_selectedQuest == null && quests.Count > 0)
        {
            _selectedQuest = quests.Find(q => q.isTracking) ?? quests[0];
        }

        RefreshAll();
    }

    public void Close()
    {
        _isOpen = false;

        if (questPanel != null)
            questPanel.SetActive(false);

        RestorePotionSlot();

        if (UIStateManager.Instance != null)
        {
            UIStateManager.Instance.CloseUI(UI_KEY);
        }

        PlaySound(closeSound);
    }

    private void OnDisable()
    {
        RestorePotionSlot();
    }

    private void HidePotionSlot()
    {
        if (quickPotionSlot == null)
        {
            if (QuickPotionSlotUI.Instance != null)
            {
                quickPotionSlot = QuickPotionSlotUI.Instance.gameObject;
            }
            else
            {
                var slotGo = GameObject.Find("UIPlatformSwitcher/CanvasDesktop/QuickPotionSlot");
                if (slotGo != null) quickPotionSlot = slotGo;
            }
        }

        if (quickPotionSlot != null && quickPotionSlot.activeSelf)
        {
            _potionSlotWasActive = true;
            quickPotionSlot.SetActive(false);
        }
    }

    private void RestorePotionSlot()
    {
        if (_potionSlotWasActive && quickPotionSlot != null)
        {
            quickPotionSlot.SetActive(true);
            _potionSlotWasActive = false;
        }
    }

    public void SelectQuest(QuestItemData quest)
    {
        _selectedQuest = quest;
        PlaySound(clickSound);

        // Update rows selection state
        foreach (var row in _spawnedRows)
        {
            if (row != null)
            {
                row.SetSelected(row.BoundData == _selectedQuest);
            }
        }

        DisplayQuestDetails(_selectedQuest);
    }

    public void NavigateSelectedQuest()
    {
        if (_selectedQuest == null)
            return;

        // Toggle tracking
        _selectedQuest.isTracking = !_selectedQuest.isTracking;

        if (_selectedQuest.isTracking)
        {
            foreach (var q in quests)
            {
                if (q != _selectedQuest) q.isTracking = false;
            }
        }

        PlaySound(navigateSound != null ? navigateSound : clickSound);

        RebuildQuestList();
        DisplayQuestDetails(_selectedQuest);

        Debug.Log($"[QuestUI] Navigating Main Quest: {_selectedQuest.questTitle} (Tracking: {_selectedQuest.isTracking})");
    }

    public void RefreshAll()
    {
        RebuildQuestList();
        if (_selectedQuest != null)
        {
            DisplayQuestDetails(_selectedQuest);
        }
        else if (quests.Count > 0)
        {
            SelectQuest(quests[0]);
        }
    }

    private void RebuildQuestList()
    {
        if (questListContent == null || questRowPrefab == null)
            return;

        // Clear existing rows and headers
        foreach (var r in _spawnedRows)
        {
            if (r != null) SafeDestroy(r.gameObject);
        }
        _spawnedRows.Clear();

        foreach (var h in _spawnedHeaderObjs)
        {
            if (h != null) SafeDestroy(h);
        }
        _spawnedHeaderObjs.Clear();

        // Also clean up any orphan children left in questListContent
        for (int i = questListContent.childCount - 1; i >= 0; i--)
        {
            SafeDestroy(questListContent.GetChild(i).gameObject);
        }

        if (quests == null || quests.Count == 0)
            return;

        // Group by Act Title
        string lastAct = "";
        foreach (var q in quests)
        {
            // Show Act Header when act title changes
            if (!string.IsNullOrEmpty(q.actTitle) && q.actTitle != lastAct && actHeaderPrefab != null)
            {
                lastAct = q.actTitle;
                var actObj = Instantiate(actHeaderPrefab, questListContent);
                actObj.SetActive(true);

                var headerUI = actObj.GetComponent<QuestActHeaderUI>();
                if (headerUI != null)
                {
                    headerUI.Setup(q.actTitle, q.chapterSubtitle);
                }
                else
                {
                    // Fallback
                    var txts = actObj.GetComponentsInChildren<TextMeshProUGUI>();
                    if (txts.Length > 0) txts[0].text = q.actTitle;
                    if (txts.Length > 1) txts[1].text = q.chapterSubtitle;
                }
                _spawnedHeaderObjs.Add(actObj);
            }

            // Instantiate Quest Row
            var rowObj = Instantiate(questRowPrefab, questListContent);
            rowObj.SetActive(true);
            var rowUI = rowObj.GetComponent<QuestRowUI>();
            if (rowUI != null)
            {
                rowUI.Bind(q, SelectQuest);
                rowUI.SetSelected(q == _selectedQuest);
                _spawnedRows.Add(rowUI);
            }
        }
    }

    private void DisplayQuestDetails(QuestItemData quest)
    {
        if (quest == null)
            return;

        // Title
        if (questTitleText != null)
        {
            questTitleText.text = quest.questTitle;
        }

        // Location
        bool hasLocation = !string.IsNullOrEmpty(quest.location);
        if (locationText != null)
        {
            locationText.gameObject.SetActive(hasLocation);
            if (hasLocation)
            {
                locationText.text = $"<color=#D4AF37>위치:</color> {quest.location}";
            }
        }

        // Adjust positions of divider, objective card, description if no location
        Transform rightPanel = questTitleText != null ? questTitleText.transform.parent : null;
        if (rightPanel != null)
        {
            var divider = rightPanel.Find("DividerLine") as RectTransform;
            var objCard = rightPanel.Find("ObjectiveCard") as RectTransform;
            var desc = rightPanel.Find("DescriptionText") as RectTransform;

            if (divider != null) divider.anchoredPosition = new Vector2(0f, hasLocation ? -102f : -66f);
            if (objCard != null) objCard.anchoredPosition = new Vector2(0f, hasLocation ? -118f : -78f);
            if (desc != null) desc.anchoredPosition = new Vector2(0f, hasLocation ? -190f : -150f);
        }

        // Objective
        if (objectiveText != null)
        {
            objectiveText.text = $"<color=#5BC0EB>> </color>{quest.currentObjective}";
        }

        // Description
        if (descriptionText != null)
        {
            descriptionText.text = quest.questDescription;
        }

        // Rewards
        UpdateRewardsDisplay(quest);

        // Navigate button state
        if (navigateButtonText != null)
        {
            navigateButtonText.text = quest.isTracking ? "추적 중" : "추적";
        }

        if (navigateButton != null)
        {
            navigateButton.interactable = !quest.isLocked;
        }
    }

    private void UpdateRewardsDisplay(QuestItemData quest)
    {
        if (rewardSlots == null || rewardSlots.Length == 0)
            return;

        int rewardCount = quest.rewards != null ? quest.rewards.Count : 0;
        for (int i = 0; i < rewardSlots.Length; i++)
        {
            if (rewardSlots[i] == null) continue;

            bool hasReward = (i < rewardCount);
            rewardSlots[i].SetActive(hasReward);

            if (hasReward)
            {
                var r = quest.rewards[i];
                if (rewardIcons != null && i < rewardIcons.Length && rewardIcons[i] != null)
                {
                    rewardIcons[i].gameObject.SetActive(r.icon != null);
                    rewardIcons[i].sprite = r.icon;
                }

                if (rewardCounts != null && i < rewardCounts.Length && rewardCounts[i] != null)
                {
                    string textToDisplay = !string.IsNullOrEmpty(r.customText)
                        ? r.customText
                        : (!string.IsNullOrEmpty(r.rewardName) && r.rewardName != "Reward" ? r.rewardName : r.count.ToString());
                    rewardCounts[i].text = textToDisplay;
                }
            }
        }
    }

    private void SafeDestroy(GameObject obj)
    {
        if (obj == null) return;
        if (Application.isPlaying)
            Destroy(obj);
        else
            DestroyImmediate(obj);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void SetupButtonListeners()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(Close);
            closeButton.onClick.AddListener(Close);
        }

        if (backgroundClickCatcher != null)
        {
            backgroundClickCatcher.onClick.RemoveListener(Close);
            backgroundClickCatcher.onClick.AddListener(Close);
        }

        if (navigateButton != null)
        {
            navigateButton.onClick.RemoveListener(NavigateSelectedQuest);
            navigateButton.onClick.AddListener(NavigateSelectedQuest);
        }
    }

    private void WireTopMenuBarButton()
    {
        if (topMenuBarQuestButton == null)
        {
            var btnGo = GameObject.Find("UIPlatformSwitcher/CanvasDesktop/TopMenuBar/QuestButton");
            if (btnGo != null) topMenuBarQuestButton = btnGo.GetComponent<Button>();
        }

        if (topMenuBarQuestButton != null)
        {
            topMenuBarQuestButton.onClick.RemoveListener(Toggle);
            topMenuBarQuestButton.onClick.AddListener(Toggle);
        }
    }

    public void InitializeDefaultStoryQuests()
    {
        quests.Clear();

        Sprite expBookIcon = Resources.Load<Sprite>("UI/Book");
        Sprite questDoneIcon = Resources.Load<Sprite>("UI/QuestDone");

        #if UNITY_EDITOR
        var loadedBook = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/QuestUI/Icon_ExpBook.png");
        if (loadedBook != null) expBookIcon = loadedBook;

        var loadedDone = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/UI/QuestDone.png");
        if (loadedDone != null) questDoneIcon = loadedDone;
        #endif

        string distStr = CalculateDungeonDistance();

        // Main Story Quest: 무너진 유적의 탐험
        var mainQuest = new QuestItemData(
            "main_01",
            "무너진 유적의 탐험",
            QuestCategory.Archon,
            "챕터 1 : 숲의 수정",
            "",
            "",
            "길을 따라 무너진 유적에 들어가세요",
            "Noa와 Que는 여행을 시작하였습니다 둘이 협동하여 숲속에 숨겨져 있는 수정을 찾으세요",
            distStr
        );
        mainQuest.isTracking = true;
        mainQuest.rewards = new List<QuestRewardItem>
        {
            new QuestRewardItem("Exp", expBookIcon, 1000, "Exp +1000"),
            new QuestRewardItem("NextChapter", questDoneIcon, 1, "다음 챕터로 가는 길 해금")
        };
        quests.Add(mainQuest);

        _selectedQuest = mainQuest;
    }

    public void MigrateQuestsToKorean()
    {
        if (quests == null || quests.Count == 0)
        {
            InitializeDefaultStoryQuests();
            return;
        }

        foreach (var q in quests)
        {
            if (q == null) continue;

            if (q.questId == "main_01" || q.questTitle == "Journey to the Central Ruins" || q.actTitle.Contains("Chapter I") || q.questTitle.Contains("무너진"))
            {
                q.actTitle = "챕터 1 : 숲의 수정";
                q.chapterSubtitle = "";
                q.questTitle = "무너진 유적의 탐험";
                q.location = "";
                q.currentObjective = "길을 따라 무너진 유적에 들어가세요";
                q.questDescription = "Noa와 Que는 여행을 시작하였습니다 둘이 협동하여 숲속에 숨겨져 있는 수정을 찾으세요";

                Sprite expBookIcon = Resources.Load<Sprite>("UI/Book");
                Sprite questDoneIcon = Resources.Load<Sprite>("UI/QuestDone");
                #if UNITY_EDITOR
                var loadedBook = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/QuestUI/Icon_ExpBook.png");
                if (loadedBook != null) expBookIcon = loadedBook;
                var loadedDone = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/UI/QuestDone.png");
                if (loadedDone != null) questDoneIcon = loadedDone;
                #endif

                q.rewards = new List<QuestRewardItem>
                {
                    new QuestRewardItem("Exp", expBookIcon, 1000, "Exp +1000"),
                    new QuestRewardItem("NextChapter", questDoneIcon, 1, "다음 챕터로 가는 길 해금")
                };
            }
        }
    }

    public Transform GetDungeonTeleportTransform()
    {
        // 1. RuneMonolith (Dungeon 1 Teleport)
        var runeMonolith = GameObject.Find("RuneMonolith");
        if (runeMonolith != null) return runeMonolith.transform;

        // 2. Any active Portal component in the scene
        var portals = Resources.FindObjectsOfTypeAll<Portal>();
        foreach (var p in portals)
        {
            if (p != null && p.gameObject.scene.isLoaded && p.gameObject.activeInHierarchy)
                return p.transform;
        }

        // 3. RuneMonolith_Thin (First monolith)
        var monolith = GameObject.Find("RuneMonolith_Thin");
        if (monolith != null) return monolith.transform;

        // 4. TransparentPortal
        var tp = GameObject.Find("TransparentPortal");
        if (tp != null) return tp.transform;

        // 5. Fallback: look for Dungeon passage object
        var passage = GameObject.Find("Dungeon_Big_Passage");
        if (passage != null) return passage.transform;

        // 5. Fallback: look for Door_Wooden
        var door = GameObject.Find("Door_Wooden_Round_Left (1)");
        if (door != null) return door.transform;

        return null;
    }

    public Transform GetActivePlayerTransform()
    {
        if (PlayerController.Instance != null && PlayerController.Instance.CurrentCharacterTransform != null)
        {
            return PlayerController.Instance.CurrentCharacterTransform;
        }

        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null && playerObj.activeInHierarchy) return playerObj.transform;

        var noa = GameObject.Find("Noa");
        if (noa != null && noa.activeInHierarchy) return noa.transform;

        var que = GameObject.Find("Que");
        if (que != null && que.activeInHierarchy) return que.transform;

        // If inactive or fallback
        if (playerObj != null) return playerObj.transform;
        if (noa != null) return noa.transform;
        if (que != null) return que.transform;

        return null;
    }

    public string CalculateDungeonDistance()
    {
        Transform player = GetActivePlayerTransform();
        Transform teleport = GetDungeonTeleportTransform();

        if (player != null && teleport != null)
        {
            float dist = Vector3.Distance(player.position, teleport.position);
            return $"{Mathf.RoundToInt(dist)}m";
        }

        return "150m";
    }

    public void UpdateQuestsDistance()
    {
        string distStr = CalculateDungeonDistance();
        if (quests != null)
        {
            foreach (var q in quests)
            {
                if (q != null)
                {
                    q.distanceText = distStr;
                }
            }
        }

        foreach (var row in _spawnedRows)
        {
            if (row != null && row.BoundData != null)
            {
                row.UpdateDistance(row.BoundData.distanceText);
            }
        }
    }

    private void FindReferencesIfNull()
    {
        if (questPanel == null)
        {
            var p = transform.Find("QuestPanel");
            if (p != null) questPanel = p.gameObject;
        }

        if (canvasGroup == null && questPanel != null)
        {
            canvasGroup = questPanel.GetComponent<CanvasGroup>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }
}


