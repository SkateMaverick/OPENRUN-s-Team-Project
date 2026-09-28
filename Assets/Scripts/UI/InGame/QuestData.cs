using System;
using System.Collections.Generic;
using UnityEngine;

public enum QuestCategory
{
    All = 0,
    Archon = 1,  // Main Story Quests
    Story = 2,   // Character Story Quests
    World = 3    // World / Side Quests
}

[Serializable]
public class QuestRewardItem
{
    public string rewardName = "Reward";
    public Sprite icon;
    public int count = 100;
    public string customText = "";

    public QuestRewardItem() { }

    public QuestRewardItem(string name, Sprite icon, int count)
    {
        this.rewardName = name;
        this.icon = icon;
        this.count = count;
        this.customText = "";
    }

    public QuestRewardItem(string name, Sprite icon, int count, string custom)
    {
        this.rewardName = name;
        this.icon = icon;
        this.count = count;
        this.customText = custom;
    }
}

[Serializable]
public class QuestItemData
{
    [Header("Basic Info")]
    public string questId = "quest_01";
    public QuestCategory category = QuestCategory.Archon;
    public string actTitle = "For a Tomorrow Without Tears";
    public string chapterSubtitle = "Prologue: Act II";
    public string questTitle = "Shadow Over Mondstadt";
    public string location = "Mondstadt, Mondstadt";

    [Header("Objectives & Lore")]
    public string currentObjective = "Return to the Knights of Favonius Headquarters with Jean";
    [TextArea(2, 4)]
    public string questDescription = "Thanks to your efforts, Mondstadt's elemental flow has returned to normal. It's time to report back to Jean.";

    [Header("Status & Requirements")]
    public string distanceText = "3070m";
    public bool isLocked = false;
    public string lockRequirement = "Requires Adventure Rank 32";
    public bool isCompleted = false;
    public bool isTracking = false;

    [Header("Rewards")]
    public List<QuestRewardItem> rewards = new List<QuestRewardItem>();

    // Backwards compatibility fields
    public Sprite thumbnail;
    public int currentProgress = 0;
    public int targetProgress = 1;
    public bool isClaimed = false;
    public Sprite reward1Icon;
    public int reward1Count = 100;
    public Sprite reward2Icon;
    public int reward2Count = 10000;

    public QuestItemData() { }

    public QuestItemData(string id, string title, QuestCategory cat, string act, string sub, string loc, string obj, string desc, string dist = "")
    {
        questId = id;
        questTitle = title;
        category = cat;
        actTitle = act;
        chapterSubtitle = sub;
        location = loc;
        currentObjective = obj;
        questDescription = desc;
        distanceText = dist;
    }
}

[CreateAssetMenu(fileName = "NewQuestDatabase", menuName = "Quest/Quest Database")]
public class QuestChapterData : ScriptableObject
{
    public int chapterNumber = 1;
    public string chapterTitle = "Chapter 1: The Ancient Monolith";

    [Header("Quests")]
    public List<QuestItemData> quests = new List<QuestItemData>();

    [Header("Chapter Completion Rewards (Optional)")]
    public Sprite[] chapterRewardIcons;
    public int[] chapterRewardCounts;
    public bool isChapterClaimed = false;
}

