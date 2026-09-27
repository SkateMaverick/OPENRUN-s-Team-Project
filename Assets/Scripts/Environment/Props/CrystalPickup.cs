using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class CrystalPickup : MonoBehaviour
{
    [Header("Item Configuration")]
    [SerializeField] private bool giveItem = true;
    [SerializeField] private ItemData itemData;
    [SerializeField] private int amount = 1;

    [Header("Interaction Settings")]
    [SerializeField] private string keyName = "F";
    [SerializeField] private string customPromptText = "";
    [SerializeField] private KeyCode interactKeyCode = KeyCode.F;
    [SerializeField] private float interactionRange = 4.5f;
    [SerializeField] private float verticalTolerance = 5.0f;
    [SerializeField] private bool destroyOnInteract = true;
    [SerializeField] private bool interactOnce = false;

    [Header("Audio & Effects")]
    [SerializeField] private AudioClip pickupSound;

    [Header("Dialogue Sequence (Optional)")]
    [SerializeField] private DialogueSequence dialogueSequence;

    public static bool IsCrystalAcquired { get; set; } = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticState()
    {
        IsCrystalAcquired = false;
    }

    private bool _inRange = false;
    private bool _isPickedUp = false;

    private void Awake()
    {
        IsCrystalAcquired = false;
    }

    private void Start()
    {
        if (giveItem && itemData == null && string.IsNullOrEmpty(customPromptText))
        {
            itemData = Resources.Load<ItemData>("LifeCrystal");
        }
    }

    private void Update()
    {
        if (interactOnce && _isPickedUp) return;

        // Hide prompt while dialogue is playing
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
                    string displayName = !string.IsNullOrEmpty(customPromptText) ? customPromptText : (itemData != null ? itemData.itemName : "생명의 수정");
                    Sprite icon = !string.IsNullOrEmpty(customPromptText) ? null : (itemData != null ? itemData.icon : null);
                    InteractionPromptUI.Instance.ShowPrompt(keyName, icon, displayName, this, PickUp);
                }
            }

            if (IsInteractKeyPressed())
            {
                PickUp();
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

    public void PickUp()
    {
        if (interactOnce && _isPickedUp) return;
        if (DialogueCutsceneManager.Instance != null && DialogueCutsceneManager.Instance.IsDialogueActive) return;

        _isPickedUp = true;
        if (giveItem) IsCrystalAcquired = true;

        if (InteractionPromptUI.Instance != null)
        {
            InteractionPromptUI.Instance.HidePrompt(this);
        }

        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, 1.0f);
        }

        if (giveItem && itemData != null && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(itemData, amount);
        }

        // Trigger Dialogue Cutscene if assigned or available
        TriggerAcquiredDialogue();

        if (destroyOnInteract)
        {
            Destroy(gameObject);
        }
        else
        {
            if (!interactOnce)
            {
                StartCoroutine(ResetInteractionAfterDialogue());
            }
        }
    }

    private System.Collections.IEnumerator ResetInteractionAfterDialogue()
    {
        yield return null;
        while (DialogueCutsceneManager.Instance != null && DialogueCutsceneManager.Instance.IsDialogueActive)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.4f);
        _isPickedUp = false;
        _inRange = false;
    }

    private void TriggerAcquiredDialogue()
    {
        if (DialogueCutsceneManager.Instance == null)
            return;

        DialogueSequence sequenceToPlay = dialogueSequence;
        if (sequenceToPlay == null)
        {
            sequenceToPlay = Resources.Load<DialogueSequence>("CrystalAcquiredDialogue");
        }

        if (sequenceToPlay != null)
        {
            DialogueCutsceneManager.Instance.StartDialogue(sequenceToPlay);
        }
        else
        {
            var fallbackLines = new System.Collections.Generic.List<DialogueLine>
            {
                new DialogueLine("Noa", "좋았어! 수정을 얻었어!", new Color(0.35f, 0.75f, 1.0f)),
                new DialogueLine("Que", "좋아! 여기서 빨리 나가자", new Color(1.0f, 0.85f, 0.35f))
            };
            DialogueCutsceneManager.Instance.StartDialogue(fallbackLines);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
