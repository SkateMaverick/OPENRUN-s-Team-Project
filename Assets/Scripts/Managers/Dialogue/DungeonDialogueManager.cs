using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DungeonDialogueManager : MonoBehaviour
{
    [Header("Dungeon Intro Dialogue")]
    [SerializeField] private DialogueSequence dungeonIntroDialogue;
    [SerializeField] private List<DialogueLine> inlineDialogueLines = new List<DialogueLine>();
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private float startDelay = 0.3f;

    [Header("Slimes Clear Dialogue")]
    [Tooltip("슬라임을 전부 처치했을 때 재생할 대화 시퀀스")]
    [SerializeField] private DialogueSequence slimesClearDialogue;
    [Tooltip("추적할 슬라임 목록 (비워둘 시 씬의 모든 SlimeSinglePlay 자동 감지)")]
    [SerializeField] private List<SlimeSinglePlay> targetSlimes = new List<SlimeSinglePlay>();
    [SerializeField] private float clearDialogueDelay = 0.8f;

    [Header("Puzzle Clear Dialogue")]
    [Tooltip("발판 2개를 활성화하여 벽이 열렸을 때 재생할 대화 시퀀스")]
    [SerializeField] private DialogueSequence puzzleClearDialogue;
    [SerializeField] private PressPuzzle pressPuzzle;
    [SerializeField] private float puzzleDialogueDelay = 0.5f;

    [Header("Events")]
    public UnityEvent onIntroDialogueFinished;
    public UnityEvent onClearDialogueFinished;
    public UnityEvent onPuzzleDialogueFinished;
    [HideInInspector] public UnityEvent onDialogueFinished; // 기존 호환용

    private bool _hasPlayedClearDialogue = false;
    private bool _hasPlayedPuzzleDialogue = false;

    private void Start()
    {
        // 1. 슬라임 타겟 목록 자동 등록 및 죽음 이벤트 바인딩
        InitSlimeTracking();

        // 2. 발판 퍼즐 풀림 이벤트 바인딩
        InitPuzzleTracking();

        // 3. 시작 시 대화 재생
        if (playOnStart)
        {
            StartCoroutine(PlayStartDialogueRoutine());
        }
    }

    private void InitPuzzleTracking()
    {
        if (pressPuzzle == null)
        {
            pressPuzzle = Object.FindAnyObjectByType<PressPuzzle>();
        }

        if (pressPuzzle != null)
        {
            pressPuzzle.OnPuzzleSolved += OnPressPuzzleSolved;
        }
    }

    private void InitSlimeTracking()
    {
        if (targetSlimes == null || targetSlimes.Count == 0)
        {
            var foundSlimes = Object.FindObjectsByType<SlimeSinglePlay>(FindObjectsSortMode.None);
            targetSlimes = new List<SlimeSinglePlay>(foundSlimes);
        }

        foreach (var slime in targetSlimes)
        {
            if (slime != null)
            {
                slime.OnDeath += OnSlimeDied;
            }
        }
    }

    private void OnDestroy()
    {
        if (targetSlimes != null)
        {
            foreach (var slime in targetSlimes)
            {
                if (slime != null)
                {
                    slime.OnDeath -= OnSlimeDied;
                }
            }
        }

        if (pressPuzzle != null)
        {
            pressPuzzle.OnPuzzleSolved -= OnPressPuzzleSolved;
        }
    }

    private void OnPressPuzzleSolved()
    {
        if (_hasPlayedPuzzleDialogue)
            return;

        StartCoroutine(WaitForSlimesAndPlayPuzzleDialogueRoutine());
    }

    private IEnumerator WaitForSlimesAndPlayPuzzleDialogueRoutine()
    {
        // 1. 적 처치 이후 조건 만족 대기
        while (!AreAllSlimesDead())
        {
            yield return new WaitForSeconds(0.2f);
        }

        // 2. 슬라임 클리어 대화가 시작될 때까지(지연 시간 포함) 대기
        while (slimesClearDialogue != null && !_hasPlayedClearDialogue)
        {
            yield return null;
        }

        // 3. 현재 진행 중인 대화가 완전히 끝날 때까지 대기
        while (DialogueCutsceneManager.Instance != null && DialogueCutsceneManager.Instance.IsDialogueActive)
        {
            yield return null;
        }

        if (_hasPlayedPuzzleDialogue)
            yield break;

        _hasPlayedPuzzleDialogue = true;

        if (puzzleDialogueDelay > 0f)
        {
            yield return new WaitForSeconds(puzzleDialogueDelay);
        }

        if (puzzleClearDialogue != null && DialogueCutsceneManager.Instance != null)
        {
            DialogueCutsceneManager.Instance.StartDialogue(puzzleClearDialogue, () =>
            {
                onPuzzleDialogueFinished?.Invoke();
            });
        }
    }

    private bool AreAllSlimesDead()
    {
        if (targetSlimes == null || targetSlimes.Count == 0)
            return true;

        foreach (var slime in targetSlimes)
        {
            if (slime != null && !slime.IsDead)
                return false;
        }
        return true;
    }

    private IEnumerator PlayStartDialogueRoutine()
    {
        if (startDelay > 0f)
        {
            yield return new WaitForSeconds(startDelay);
        }

        // Wait until DialogueCutsceneManager is available
        float timeout = 3.0f;
        while (DialogueCutsceneManager.Instance == null && timeout > 0f)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        PlayDungeonDialogue();
    }

    public void PlayDungeonDialogue()
    {
        if (DialogueCutsceneManager.Instance == null)
        {
            Debug.LogWarning("[DungeonDialogueManager] DialogueCutsceneManager.Instance is null!");
            return;
        }

        if (dungeonIntroDialogue != null)
        {
            DialogueCutsceneManager.Instance.StartDialogue(dungeonIntroDialogue, () =>
            {
                onIntroDialogueFinished?.Invoke();
                onDialogueFinished?.Invoke();
            });
        }
        else if (inlineDialogueLines != null && inlineDialogueLines.Count > 0)
        {
            DialogueCutsceneManager.Instance.StartDialogue(inlineDialogueLines, () =>
            {
                onIntroDialogueFinished?.Invoke();
                onDialogueFinished?.Invoke();
            });
        }
    }

    private void OnSlimeDied()
    {
        if (_hasPlayedClearDialogue)
            return;

        // 모든 슬라임이 죽었는지 확인
        bool allDead = true;
        foreach (var slime in targetSlimes)
        {
            if (slime != null && !slime.IsDead)
            {
                allDead = false;
                break;
            }
        }

        if (allDead)
        {
            _hasPlayedClearDialogue = true;
            StartCoroutine(PlayClearDialogueRoutine());
        }
    }

    private IEnumerator PlayClearDialogueRoutine()
    {
        if (clearDialogueDelay > 0f)
        {
            yield return new WaitForSeconds(clearDialogueDelay);
        }

        if (slimesClearDialogue != null && DialogueCutsceneManager.Instance != null)
        {
            DialogueCutsceneManager.Instance.StartDialogue(slimesClearDialogue, () =>
            {
                onClearDialogueFinished?.Invoke();
            });
        }
    }
}


