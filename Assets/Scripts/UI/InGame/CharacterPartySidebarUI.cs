using UnityEngine;

public class CharacterPartySidebarUI : MonoBehaviour
{
    [Header("Slots")]
    public CharacterPartySlotUI noaSlot;
    public CharacterPartySlotUI queSlot;

    private PlayerController _playerController;

    private void Start()
    {
        InitializeParty();
        BindPlayerController();
    }

    private void OnEnable()
    {
        BindPlayerController();
    }

    private void OnDisable()
    {
        UnbindPlayerController();
    }

    private void OnDestroy()
    {
        UnbindPlayerController();
    }

    private void BindPlayerController()
    {
        UnbindPlayerController();
        _playerController = PlayerController.Instance != null ? PlayerController.Instance : Object.FindFirstObjectByType<PlayerController>();
        if (_playerController != null)
        {
            _playerController.OnCharacterChanged += OnCharacterChanged;
            RefreshSelection(_playerController.CurrentCharacterType);
        }
    }

    private void UnbindPlayerController()
    {
        if (_playerController != null)
        {
            _playerController.OnCharacterChanged -= OnCharacterChanged;
            _playerController = null;
        }
    }

    public void InitializeParty()
    {
        if (noaSlot != null)
        {
            noaSlot.characterType = Enums.CharacterType.BoxGolem;
            noaSlot.characterName = "Noa";
            noaSlot.slotNumber = 1;
            noaSlot.ApplySlotData();
            noaSlot.SetHealth(100f, 100f);
        }

        if (queSlot != null)
        {
            queSlot.characterType = Enums.CharacterType.SphereGolem;
            queSlot.characterName = "Que";
            queSlot.slotNumber = 2;
            queSlot.ApplySlotData();
            queSlot.SetHealth(100f, 100f);
        }
    }

    private void OnCharacterChanged(Transform currentTransform)
    {
        var pc = PlayerController.Instance != null ? PlayerController.Instance : Object.FindFirstObjectByType<PlayerController>();
        if (pc != null)
        {
            if (currentTransform != null)
            {
                bool isNoa = currentTransform == (pc.boxGolem != null ? pc.boxGolem.transform : null) || currentTransform.name.Contains("Noa");
                RefreshSelection(isNoa ? Enums.CharacterType.BoxGolem : Enums.CharacterType.SphereGolem);
            }
            else
            {
                RefreshSelection(pc.CurrentCharacterType);
            }
        }
    }

    public void RefreshSelection(Enums.CharacterType activeType)
    {
        if (noaSlot != null)
        {
            noaSlot.SetActiveState(activeType == Enums.CharacterType.BoxGolem);
        }

        if (queSlot != null)
        {
            queSlot.SetActiveState(activeType == Enums.CharacterType.SphereGolem);
        }
    }
}
