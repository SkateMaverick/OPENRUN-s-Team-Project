using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ItemPickup : MonoBehaviour
{
    public ItemData itemData;
    public int amount = 1;

    [Header("Visual Effects")]
    public bool bobAndRotate = true;
    public float rotateSpeed = 60f;
    public float bobFrequency = 2f;
    public float bobAmplitude = 0.15f;

    [Header("Interaction Settings")]
    [SerializeField] private bool requireInteraction = false;
    [SerializeField] private string keyName = "F";
    [SerializeField] private string customPromptText = "";
    [SerializeField] private KeyCode interactKeyCode = KeyCode.F;
    [SerializeField] private float interactionRange = 3.5f;
    [SerializeField] private float verticalTolerance = 4.0f;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private DialogueSequence dialogueSequence;

    private Vector3 _startPos;
    private bool _inRange = false;
    private bool _isPickedUp = false;

    private void Start()
    {
        _startPos = transform.position;
    }

    private void Update()
    {
        if (bobAndRotate)
        {
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
            float newY = _startPos.y + Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        if (_isPickedUp)
            return;

        // Auto-pickup proximity check when requireInteraction is false
        if (!requireInteraction)
        {
            Transform pTransform = GetActivePlayerTransform();
            if (pTransform != null)
            {
                Vector3 pPos = pTransform.position;
                Vector3 myP = transform.position;
                float hDist = Vector2.Distance(new Vector2(pPos.x, pPos.z), new Vector2(myP.x, myP.z));
                float vDist = Mathf.Abs(pPos.y - myP.y);

                if (hDist <= 2.0f && vDist <= 3.0f)
                {
                    PickUp();
                    return;
                }
            }
            return;
        }

        // Hide prompt while dialogue is active
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
                    string displayName = !string.IsNullOrEmpty(customPromptText) 
                        ? customPromptText 
                        : (itemData != null ? itemData.itemName : "아이템");
                    Sprite icon = itemData != null ? itemData.icon : null;
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
        if (_isPickedUp) return;
        if (DialogueCutsceneManager.Instance != null && DialogueCutsceneManager.Instance.IsDialogueActive) return;

        _isPickedUp = true;

        if (InteractionPromptUI.Instance != null)
        {
            InteractionPromptUI.Instance.HidePrompt(this);
        }

        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, 1.0f);
        }

        if (itemData != null)
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.AddItem(itemData, amount);
                Debug.Log($"[ItemPickup] Added {amount}x {itemData.itemName} to inventory.");
            }
            else
            {
                Debug.LogError($"[ItemPickup] InventoryManager.Instance is null! Could not add {itemData.itemName}");
                ItemAcquisitionUI.Show(itemData, amount);
            }
        }

        if (dialogueSequence != null && DialogueCutsceneManager.Instance != null)
        {
            DialogueCutsceneManager.Instance.StartDialogue(dialogueSequence);
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (requireInteraction)
            return;

        if (!IsPlayer(other))
            return;

        PickUp();
    }

    private void OnTriggerStay(Collider other)
    {
        if (requireInteraction)
            return;

        if (!IsPlayer(other))
            return;

        PickUp();
    }

    private bool IsPlayer(Collider other)
    {
        if (other == null) return false;
        if (other.CompareTag("Player")) return true;
        if (other.transform.root != null && other.transform.root.CompareTag("Player")) return true;
        if (other.GetComponentInParent<PlayerController>() != null) return true;
        if (other.GetComponentInParent<IControllable>() != null) return true;
        return false;
    }

    private Transform GetActivePlayerTransform()
    {
        if (PlayerController.Instance != null && PlayerController.Instance.CurrentCharacterTransform != null)
        {
            return PlayerController.Instance.CurrentCharacterTransform;
        }

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null && playerObj.activeInHierarchy) return playerObj.transform;

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
        if (requireInteraction)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
    }
}