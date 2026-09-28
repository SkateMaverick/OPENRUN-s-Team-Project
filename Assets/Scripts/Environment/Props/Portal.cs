using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// 
/// </summary>
public class Portal : MonoBehaviour
{
    // 포탈이 닿았을 때 이동할 씬
    [SerializeField] private string targetSceneName = "Dungeon 1";

    [Header("Interaction Settings")]
    [SerializeField] private bool requireInteractKey = false;
    [SerializeField] private string keyName = "F";
    [SerializeField] private KeyCode interactKeyCode = KeyCode.F;
    [SerializeField] private string promptText = "미궁으로 진입";
    [SerializeField] private float interactionRange = 5.0f;
    [SerializeField] private float verticalTolerance = 5.0f;

    [Header("Item Requirement (Optional)")]
    [Tooltip("크리스탈 등 특정 아이템을 획득해야만 포탈이 활성화되는지 여부")]
    [SerializeField] private bool requireCrystal = false;
    [SerializeField] private ItemData requiredItem;
    [SerializeField] private string lockedNoticeText = "생명의 수정이 필요합니다";

    [Header("Dialogue Before Teleport (Optional)")]
    [Tooltip("텔레포트 전 대화를 재생할지 여부")]
    [SerializeField] private bool playDialogueBeforeTeleport = false;
    [Tooltip("대화 시퀀스 에셋. 비어있으면 inlineDialogueLines 또는 기본 대사를 사용합니다.")]
    [SerializeField] private DialogueSequence dialogueSequence;
    [Tooltip("인라인 대화 목록.")]
    [SerializeField] private List<DialogueLine> inlineDialogueLines = new List<DialogueLine>();

    private bool _hasTriggered = false;
    private bool _inRange = false;

    private void Update()
    {
        if (_hasTriggered) return;
        if (!requireInteractKey) return;

        // Hide prompt if dialogue is currently active
        if (DialogueCutsceneManager.Instance != null && DialogueCutsceneManager.Instance.IsDialogueActive)
        {
            if (_inRange)
            {
                _inRange = false;
                if (InteractionPromptUI.Instance != null)
                {
                    InteractionPromptUI.Instance.HidePrompt(this);
                }
            }
            return;
        }

        Transform playerTransform = GetActivePlayerTransform();
        if (playerTransform == null)
        {
            if (_inRange)
            {
                _inRange = false;
                if (InteractionPromptUI.Instance != null)
                {
                    InteractionPromptUI.Instance.HidePrompt(this);
                }
            }
            return;
        }

        Vector3 playerPos = playerTransform.position;
        Vector3 myPos = transform.position;

        float horizontalDist = Vector2.Distance(new Vector2(playerPos.x, playerPos.z), new Vector2(myPos.x, myPos.z));
        float verticalDist = Mathf.Abs(playerPos.y - myPos.y);

        bool withinRange = horizontalDist <= interactionRange && verticalDist <= verticalTolerance;

        if (withinRange)
        {
            if (!_inRange)
            {
                _inRange = true;
                if (InteractionPromptUI.Instance != null)
                {
                    if (requireCrystal && !CheckCrystalAcquired())
                    {
                        InteractionPromptUI.Instance.ShowPrompt("", null, lockedNoticeText, this);
                    }
                    else
                    {
                        InteractionPromptUI.Instance.ShowPrompt(keyName, null, promptText, this, ExecuteTeleport);
                    }
                }
            }

            if (IsInteractKeyPressed())
            {
                ExecuteTeleport();
            }
        }
        else
        {
            if (_inRange)
            {
                _inRange = false;
                if (InteractionPromptUI.Instance != null)
                {
                    InteractionPromptUI.Instance.HidePrompt(this);
                }
            }
        }
    }

    private bool IsInteractKeyPressed()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            return true;
        }
#endif
        try
        {
            if (Input.GetKeyDown(interactKeyCode))
            {
                return true;
            }
        }
        catch { }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        TryPortal(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryPortal(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsPlayer(other))
        {
            if (InteractionPromptUI.Instance != null)
            {
                InteractionPromptUI.Instance.HidePrompt(this);
            }
            _inRange = false;
        }
    }

    private void TryPortal(Collider other)
    {
        if (_hasTriggered) return;
        if (!IsPlayer(other)) return;

        if (requireInteractKey)
        {
            // Handled in Update() with F prompt
            return;
        }

        ExecuteTeleport();
    }

    public void ExecuteTeleport()
    {
        if (_hasTriggered) return;

        if (requireCrystal && !CheckCrystalAcquired())
        {
            // 크리스탈 미획득 시 안내 표시
            if (InteractionPromptUI.Instance != null)
            {
                InteractionPromptUI.Instance.ShowPrompt("", null, lockedNoticeText, this);
            }
            return;
        }

        if (playDialogueBeforeTeleport && DialogueCutsceneManager.Instance != null)
        {
            StartDialogueAndTeleport();
            return;
        }

        DoTeleport();
    }

    private void StartDialogueAndTeleport()
    {
        if (_hasTriggered) return;
        _hasTriggered = true;

        if (InteractionPromptUI.Instance != null)
        {
            InteractionPromptUI.Instance.HidePrompt(this);
        }
        _inRange = false;

        if (dialogueSequence != null && dialogueSequence.lines != null && dialogueSequence.lines.Count > 0)
        {
            DialogueCutsceneManager.Instance.StartDialogue(dialogueSequence, DoTeleport);
        }
        else if (inlineDialogueLines != null && inlineDialogueLines.Count > 0)
        {
            DialogueCutsceneManager.Instance.StartDialogue(inlineDialogueLines, DoTeleport);
        }
        else
        {
            var defaultLines = new List<DialogueLine>
            {
                new DialogueLine("Que", "여기가 작은돌이 말한 그곳인가봐", new Color(1.0f, 0.85f, 0.35f, 1.0f)),
                new DialogueLine("Noa", "좋아 그럼 한번 같이 들어가보자고", new Color(0.35f, 0.75f, 1.0f, 1.0f))
            };
            DialogueCutsceneManager.Instance.StartDialogue(defaultLines, DoTeleport);
        }
    }

    private void DoTeleport()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            _hasTriggered = true;

            if (InteractionPromptUI.Instance != null)
            {
                InteractionPromptUI.Instance.HidePrompt(this);
            }

            SceneLoader.NextSceneName = targetSceneName;
            SceneManager.LoadScene("LoadingScene");
        }
        else
        {
            Debug.LogError("씬 목적지가 적혀있지 않습니다. Portal 스크립트를 가진 게임오브젝트의 인스펙터 창에서 targetSceneName에 씬 이름을 넣어주세요.");
        }
    }

    private Transform GetActivePlayerTransform()
    {
        if (PlayerController.Instance != null && PlayerController.Instance.CurrentCharacterTransform != null)
        {
            return PlayerController.Instance.CurrentCharacterTransform;
        }

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            return playerObj.transform;
        }

        GameObject noa = GameObject.Find("Noa");
        if (noa != null && noa.activeInHierarchy) return noa.transform;

        GameObject que = GameObject.Find("Que");
        if (que != null && que.activeInHierarchy) return que.transform;

        return null;
    }

    private void OnDisable()
    {
        if (_inRange && InteractionPromptUI.Instance != null)
        {
            InteractionPromptUI.Instance.HidePrompt(this);
        }
        _inRange = false;
    }

    private void OnDestroy()
    {
        if (_inRange && InteractionPromptUI.Instance != null)
        {
            InteractionPromptUI.Instance.HidePrompt(this);
        }
    }

    private bool IsPlayer(Collider other)
    {
        if (other == null) return false;
        return other.TryGetComponent<IControllable>(out _) ||
               other.GetComponentInParent<IControllable>() != null ||
               other.CompareTag("Player");
    }

    public bool CheckCrystalAcquired()
    {
        // 1. CrystalPickup의 정적 플래그 확인
        if (CrystalPickup.IsCrystalAcquired) return true;

        // 2. 인벤토리에 지정된 아이템이 있는지 확인
        if (requiredItem != null && InventoryManager.Instance != null && InventoryManager.Instance.HasItem(requiredItem))
        {
            return true;
        }

        // 3. 인벤토리에 '생명의 수정' 또는 유사 이름 아이템 확인
        if (InventoryManager.Instance != null)
        {
            foreach (var slot in InventoryManager.Instance.slots)
            {
                if (!slot.IsEmpty() && slot.item != null &&
                    (slot.item.itemName.Contains("수정") || slot.item.itemName.Contains("Crystal")))
                {
                    return true;
                }
            }
        }

        // 4. 씬 내 Crystal 오브젝트가 파괴되었는지 확인
        GameObject crystalGo = GameObject.Find("Crystal");
        if (crystalGo == null)
        {
            return true;
        }

        return false;
    }
}

