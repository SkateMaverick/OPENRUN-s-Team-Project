using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class IntroWalkCutsceneController : MonoBehaviour
{
    public static IntroWalkCutsceneController Instance { get; private set; }

    [Header("Characters")]
    [SerializeField] private Transform noa;
    [SerializeField] private Transform que;

    [Header("Cameras")]
    [SerializeField] private CinemachineCamera noaCam;

    [Header("Dialogue Content")]
    [Tooltip("DialogueSequence to play when both characters arrive at the destination on the FIRST run.")]
    [SerializeField] private DialogueSequence dialogueSequence;

    [Tooltip("DialogueSequence to play when returning to SingleMain after clearing Dungeon 1-4.")]
    [SerializeField] private DialogueSequence dungeonReturnDialogueSequence;

    public static bool HasPlayedFirstIntro { get; set; } = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticState()
    {
        HasPlayedFirstIntro = false;
    }

    [Header("Path Settings")]
    [Tooltip("Center X position of the road.")]
    [SerializeField] private float pathCenterX = 0.40f;

    [Tooltip("Starting Z position for Noa (in back).")]
    [SerializeField] private float startZNoa = -11.51f;

    [Tooltip("Starting Z position for Que (in front).")]
    [SerializeField] private float startZQue = -9.20f;

    [Tooltip("Arrival Z position for Que (front golem).")]
    [SerializeField] private float targetZQue = -2.20f;

    [Tooltip("Arrival Z position for Noa (back golem).")]
    [SerializeField] private float targetZNoa = -4.50f;

    [Tooltip("Movement speed while walking down the path.")]
    [SerializeField] private float walkSpeed = 2.4f;

    [Tooltip("Layer mask used for ground slope raycasting.")]
    [SerializeField] private LayerMask groundLayer = ~0;

    [Header("UI & Control Settings")]
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private GameObject canvasDesktop;
    [SerializeField] private GameObject canvasMobile;
    [SerializeField] private GameObject topMenuBar;
    [SerializeField] private GameObject partySidebar;
    [SerializeField] private GameObject playerHUD;
    [SerializeField] private GameObject minimapFrame;
    [SerializeField] private GameObject quickPotionSlot;

    [Header("Debug")]
    [SerializeField] private bool enableTestHotkey = true;
    [SerializeField] private Key testHotkey = Key.F6;

    [Header("Events")]
    public UnityEvent onCutsceneStart;
    public UnityEvent onArrivalReached;
    public UnityEvent onCutsceneComplete;

    private const string CUTSCENE_UI_KEY = "IntroWalkCutscene";
    private Rigidbody _noaRb;
    private Rigidbody _queRb;
    private Collider[] _noaColliders;
    private Collider[] _queColliders;
    private PlayerController _playerController;

    private bool _isCutsceneActive = false;
    private bool _hasArrived = false;

    public bool IsCutsceneActive => _isCutsceneActive;

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
    }

    private void Start()
    {
        FindReferencesIfNull();

        if (playOnStart)
        {
            StartCoroutine(StartCutsceneDelayedRoutine());
        }
    }

    private IEnumerator StartCutsceneDelayedRoutine()
    {
        // Wait 2 frames so other initializations (UIPlatformSwitcher, PlayerController) finish
        yield return null;
        yield return null;

        // Check if player is returning from dungeon clear
        bool isDungeonReturn = CheckIsDungeonReturn();

        if (isDungeonReturn)
        {
            // Returning from Dungeon 1-4: Play the return dialogue
            PlayDungeonReturnCutscene();
        }
        else if (!HasPlayedFirstIntro)
        {
            // First time in SingleMain: Play the intro walk cutscene
            StartCutscene();
        }
        else
        {
            // Already played first intro in this playthrough and not returning from dungeon: do nothing
            // Normal gameplay starts immediately
        }
    }

    private bool CheckIsDungeonReturn()
    {
        // 1. CrystalPickup acquired flag
        if (CrystalPickup.IsCrystalAcquired) return true;

        // 2. Inventory check
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

        return false;
    }

    private void PlayDungeonReturnCutscene()
    {
        FindReferencesIfNull();

        _isCutsceneActive = true;
        _hasArrived = true;

        // Disable player manipulation
        if (_playerController != null)
        {
            _playerController.enabled = false;
        }

        if (UIStateManager.Instance != null)
        {
            UIStateManager.Instance.OpenUI(CUTSCENE_UI_KEY);
        }

        // Disable all gameplay UI
        SetGameplayUIVisibility(false);

        // Position characters at the arrival area so they stand naturally
        if (que != null)
        {
            float groundY = GetGroundY(pathCenterX, targetZQue, que.position.y);
            que.position = new Vector3(pathCenterX, groundY, targetZQue);
            que.rotation = Quaternion.LookRotation(Vector3.forward);
            if (_queRb != null)
            {
                _queRb.linearVelocity = Vector3.zero;
                _queRb.angularVelocity = Vector3.zero;
            }
        }

        if (noa != null)
        {
            float groundY = GetGroundY(pathCenterX, targetZNoa, noa.position.y);
            noa.position = new Vector3(pathCenterX, groundY, targetZNoa);
            noa.rotation = Quaternion.LookRotation(Vector3.forward);
            if (_noaRb != null)
            {
                _noaRb.linearVelocity = Vector3.zero;
                _noaRb.angularVelocity = Vector3.zero;
            }
        }

        if (noaCam != null && noa != null)
        {
            noaCam.gameObject.SetActive(true);
            noaCam.Target.TrackingTarget = noa;
        }

        onCutsceneStart?.Invoke();
        onArrivalReached?.Invoke();

        DialogueSequence sequenceToPlay = dungeonReturnDialogueSequence;
        if (sequenceToPlay == null)
        {
            sequenceToPlay = Resources.Load<DialogueSequence>("DungeonReturnDialogue");
        }

        if (DialogueCutsceneManager.Instance != null && sequenceToPlay != null)
        {
            DialogueCutsceneManager.Instance.StartDialogue(sequenceToPlay, OnDialogueComplete);
        }
        else
        {
            List<DialogueLine> fallbackLines = new List<DialogueLine>
            {
                new DialogueLine("Noa", "드디어 수정을 얻었어", new Color(0.35f, 0.75f, 1.0f)),
                new DialogueLine("Que", "좋아 이제 다음 수정을 향해 나아가는거야", new Color(1.0f, 0.85f, 0.35f))
            };

            if (DialogueCutsceneManager.Instance != null)
            {
                DialogueCutsceneManager.Instance.StartDialogue(fallbackLines, OnDialogueComplete);
            }
            else
            {
                OnDialogueComplete();
            }
        }
    }

    private void Update()
    {
        if (enableTestHotkey && !_isCutsceneActive)
        {
            bool pressed = false;
            if (Keyboard.current != null && Keyboard.current[testHotkey].wasPressedThisFrame)
            {
                pressed = true;
            }
            else
            {
                try
                {
                    if (Input.GetKeyDown(KeyCode.F6)) pressed = true;
                }
                catch { }
            }

            if (pressed)
            {
                StartCutscene();
            }
        }
    }

    [Header("Movement & Animation")]
    private IControllable _noaControllable;
    private IControllable _queControllable;

    private void FixedUpdate()
    {
        if (!_isCutsceneActive || _hasArrived)
            return;

        // 1. Move Que (Front)
        if (que != null)
        {
            if (que.position.z < targetZQue)
            {
                if (_queControllable != null)
                {
                    _queControllable.HandleCharacterControl(new Vector2(0f, 0.45f), false);
                }
                else if (_queRb != null)
                {
                    Vector3 moveDir = GetSlopeForward(que.position, que);
                    Vector3 targetVel = moveDir * walkSpeed;
                    float xDiff = (pathCenterX - que.position.x) * 3f;
                    _queRb.linearVelocity = new Vector3(xDiff, _queRb.linearVelocity.y, targetVel.z);
                    que.rotation = Quaternion.Slerp(que.rotation, Quaternion.LookRotation(Vector3.forward), Time.fixedDeltaTime * 12f);
                }
            }
            else
            {
                if (_queControllable != null) _queControllable.HandleCharacterControl(Vector2.zero, false);
                if (_queRb != null) _queRb.linearVelocity = new Vector3(0f, _queRb.linearVelocity.y, 0f);
            }
        }

        // 2. Move Noa (Back)
        if (noa != null)
        {
            if (noa.position.z < targetZNoa)
            {
                if (_noaControllable != null)
                {
                    _noaControllable.HandleCharacterControl(new Vector2(0f, 0.45f), false);
                }
                else if (_noaRb != null)
                {
                    Vector3 moveDir = GetSlopeForward(noa.position, noa);
                    Vector3 targetVel = moveDir * walkSpeed;
                    float xDiff = (pathCenterX - noa.position.x) * 3f;
                    _noaRb.linearVelocity = new Vector3(xDiff, _noaRb.linearVelocity.y, targetVel.z);
                    noa.rotation = Quaternion.Slerp(noa.rotation, Quaternion.LookRotation(Vector3.forward), Time.fixedDeltaTime * 12f);
                }
            }
            else
            {
                if (_noaControllable != null) _noaControllable.HandleCharacterControl(Vector2.zero, false);
                if (_noaRb != null) _noaRb.linearVelocity = new Vector3(0f, _noaRb.linearVelocity.y, 0f);
            }
        }

        // 3. Check Arrival
        bool queArrived = (que == null) || (que.position.z >= targetZQue);
        bool noaArrived = (noa == null) || (noa.position.z >= targetZNoa);

        if (queArrived && noaArrived)
        {
            _hasArrived = true;
            if (_queControllable != null) _queControllable.HandleCharacterControl(Vector2.zero, false);
            if (_noaControllable != null) _noaControllable.HandleCharacterControl(Vector2.zero, false);
            StartCoroutine(ArrivalRoutine());
        }
    }

    public void StartCutscene()
    {
        FindReferencesIfNull();

        _isCutsceneActive = true;
        _hasArrived = false;

        // Disable player manipulation
        if (_playerController != null)
        {
            _playerController.enabled = false;
        }

        if (UIStateManager.Instance != null)
        {
            UIStateManager.Instance.OpenUI(CUTSCENE_UI_KEY);
        }

        // Disable all gameplay UI
        SetGameplayUIVisibility(false);

        // Ignore collisions between Noa and Que during the cutscene walk
        SetCharactersCollisionIgnored(true);

        // Position characters at starting positions
        if (que != null)
        {
            float groundY = GetGroundY(pathCenterX, startZQue, que.position.y);
            que.position = new Vector3(pathCenterX, groundY, startZQue);
            que.rotation = Quaternion.LookRotation(Vector3.forward);
            if (_queRb != null)
            {
                _queRb.linearVelocity = Vector3.zero;
                _queRb.angularVelocity = Vector3.zero;
            }
        }

        if (noa != null)
        {
            float groundY = GetGroundY(pathCenterX, startZNoa, noa.position.y);
            noa.position = new Vector3(pathCenterX, groundY, startZNoa);
            noa.rotation = Quaternion.LookRotation(Vector3.forward);
            if (_noaRb != null)
            {
                _noaRb.linearVelocity = Vector3.zero;
                _noaRb.angularVelocity = Vector3.zero;
            }
        }

        // Ensure NoaCam is tracking Noa
        if (noaCam != null && noa != null)
        {
            noaCam.gameObject.SetActive(true);
            noaCam.Target.TrackingTarget = noa;
        }

        onCutsceneStart?.Invoke();
    }

    private IEnumerator ArrivalRoutine()
    {
        // Stop both characters
        if (_queRb != null) _queRb.linearVelocity = Vector3.zero;
        if (_noaRb != null) _noaRb.linearVelocity = Vector3.zero;

        onArrivalReached?.Invoke();

        // Brief delay for animations to settle into Idle
        yield return new WaitForSeconds(0.4f);

        // Start Dialogue Cutscene
        if (DialogueCutsceneManager.Instance != null && dialogueSequence != null)
        {
            DialogueCutsceneManager.Instance.StartDialogue(dialogueSequence, OnDialogueComplete);
        }
        else if (DialogueCutsceneManager.Instance != null)
        {
            DialogueCutsceneManager.Instance.TestGenshinDialogue();
            // Fallback: poll until dialogue completes
            StartCoroutine(WaitForDialogueEndRoutine());
        }
        else
        {
            OnDialogueComplete();
        }
    }

    private IEnumerator WaitForDialogueEndRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        while (DialogueCutsceneManager.Instance != null && DialogueCutsceneManager.Instance.IsDialogueActive)
        {
            yield return null;
        }
        OnDialogueComplete();
    }

    private void OnDialogueComplete()
    {
        _isCutsceneActive = false;

        // Restore collision between characters
        SetCharactersCollisionIgnored(false);

        // Re-enable player manipulation
        if (_playerController != null)
        {
            _playerController.enabled = true;
        }

        if (UIStateManager.Instance != null)
        {
            UIStateManager.Instance.CloseUI(CUTSCENE_UI_KEY);
        }

        // Restore gameplay UI
        SetGameplayUIVisibility(true);

        // Mark first intro as played for this playthrough
        HasPlayedFirstIntro = true;
        PlayerPrefs.DeleteKey("IntroCutscene_Played");

        onCutsceneComplete?.Invoke();
    }

    private void SetGameplayUIVisibility(bool visible)
    {
        if (topMenuBar != null) topMenuBar.SetActive(visible);
        if (partySidebar != null) partySidebar.SetActive(visible);
        if (playerHUD != null) playerHUD.SetActive(visible);
        if (minimapFrame != null) minimapFrame.SetActive(visible);
        if (quickPotionSlot != null) quickPotionSlot.SetActive(visible);

        if (canvasMobile != null && Application.isMobilePlatform)
        {
            canvasMobile.SetActive(visible);
        }
    }

    private void SetCharactersCollisionIgnored(bool ignore)
    {
        if (_noaColliders == null || _queColliders == null)
            return;

        foreach (var colA in _noaColliders)
        {
            if (colA == null) continue;
            foreach (var colB in _queColliders)
            {
                if (colB == null) continue;
                Physics.IgnoreCollision(colA, colB, ignore);
            }
        }
    }

    private Vector3 GetSlopeForward(Vector3 currentPos, Transform character)
    {
        Vector3 rayOrigin = currentPos + Vector3.up * 1.5f;
        RaycastHit[] hits = Physics.RaycastAll(rayOrigin, Vector3.down, 4.0f, groundLayer);
        foreach (var hit in hits)
        {
            if (hit.collider.isTrigger) continue;
            if (character != null && (hit.collider.transform == character || hit.collider.transform.IsChildOf(character)))
                continue;

            return Vector3.ProjectOnPlane(Vector3.forward, hit.normal).normalized;
        }
        return Vector3.forward;
    }

    private float GetGroundY(float x, float z, float defaultY)
    {
        Vector3 rayOrigin = new Vector3(x, 10f, z);
        RaycastHit[] hits = Physics.RaycastAll(rayOrigin, Vector3.down, 20f, groundLayer);
        foreach (var hit in hits)
        {
            if (hit.collider.isTrigger) continue;
            if (noa != null && (hit.collider.transform == noa || hit.collider.transform.IsChildOf(noa))) continue;
            if (que != null && (hit.collider.transform == que || hit.collider.transform.IsChildOf(que))) continue;

            return hit.point.y + 0.1f;
        }
        return defaultY;
    }

    private void FindReferencesIfNull()
    {
        if (noa == null)
        {
            var noaObj = GameObject.Find("Noa");
            if (noaObj != null) noa = noaObj.transform;
        }

        if (que == null)
        {
            var queObj = GameObject.Find("Que");
            if (queObj != null) que = queObj.transform;
        }

        if (noa != null)
        {
            _noaRb = noa.GetComponent<Rigidbody>();
            _noaColliders = noa.GetComponents<Collider>();
            _noaControllable = noa.GetComponent<IControllable>();
        }

        if (que != null)
        {
            _queRb = que.GetComponent<Rigidbody>();
            _queColliders = que.GetComponents<Collider>();
            _queControllable = que.GetComponent<IControllable>();
        }

        if (_playerController == null)
        {
            _playerController = Object.FindFirstObjectByType<PlayerController>();
        }

        if (noaCam == null)
        {
            var noaCamObj = GameObject.Find("NoaCam");
            if (noaCamObj != null) noaCam = noaCamObj.GetComponent<CinemachineCamera>();
        }

        if (canvasDesktop == null)
            canvasDesktop = GameObject.Find("UIPlatformSwitcher/CanvasDesktop");
        if (canvasMobile == null)
            canvasMobile = GameObject.Find("UIPlatformSwitcher/CanvasMobile");

        if (topMenuBar == null)
            topMenuBar = GameObject.Find("UIPlatformSwitcher/CanvasDesktop/TopMenuBar");
        if (partySidebar == null)
            partySidebar = GameObject.Find("UIPlatformSwitcher/CanvasDesktop/PartySidebar");
        if (playerHUD == null)
            playerHUD = GameObject.Find("UIPlatformSwitcher/CanvasDesktop/PlayerHealthBarHUD");
        if (minimapFrame == null)
            minimapFrame = GameObject.Find("UIPlatformSwitcher/CanvasDesktop/MinimapFrame");
        if (quickPotionSlot == null)
            quickPotionSlot = GameObject.Find("UIPlatformSwitcher/CanvasDesktop/QuickPotionSlot");
    }

    [ContextMenu("Reset Intro Cutscene State")]
    public void ResetIntroCutscenePref()
    {
        HasPlayedFirstIntro = false;
        PlayerPrefs.DeleteKey("IntroCutscene_Played");
        PlayerPrefs.Save();
        Debug.Log("[IntroWalkCutsceneController] Reset HasPlayedFirstIntro to false");
    }
}
