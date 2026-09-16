using UnityEngine;

public class CharacterPartySidebarUI : MonoBehaviour
{
    [Header("Slots")]
    public CharacterPartySlotUI noaSlot;
    public CharacterPartySlotUI queSlot;

    private void Start()
    {
        InitializeParty();

        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnCharacterChanged += OnCharacterChanged;
            RefreshSelection(PlayerController.Instance.CurrentCharacterType);
        }
    }

    private void OnEnable()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnCharacterChanged -= OnCharacterChanged;
            PlayerController.Instance.OnCharacterChanged += OnCharacterChanged;
            RefreshSelection(PlayerController.Instance.CurrentCharacterType);
        }
    }

    private void OnDisable()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnCharacterChanged -= OnCharacterChanged;
        }
    }

    private void OnDestroy()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnCharacterChanged -= OnCharacterChanged;
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
