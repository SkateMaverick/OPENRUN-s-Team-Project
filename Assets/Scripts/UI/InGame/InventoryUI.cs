using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    private const string UI_KEY = "InventoryUI";

    public InventorySlotUI[] slotUIs;

    private void Start()
    {
        BindInventory();
        Refresh();
    }

    private void OnEnable()
    {
        BindInventory();
        Refresh();

        if (UIStateManager.Instance != null)
        {
            UIStateManager.Instance.OpenUI(UI_KEY);
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void BindInventory()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.onInventoryChanged -= Refresh;
            InventoryManager.Instance.onInventoryChanged += Refresh;
        }
    }

    private void OnDisable()
    {
        if (UIStateManager.Instance != null)
        {
            UIStateManager.Instance.CloseUI(UI_KEY);
        }
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.onInventoryChanged -= Refresh;
        }

        if (UIStateManager.Instance != null)
        {
            UIStateManager.Instance.CloseUI(UI_KEY);
        }
    }

    public void Refresh()
    {
        if (InventoryManager.Instance == null) return;

        List<InventorySlotData> slots = InventoryManager.Instance.slots;

        for (int i = 0; i < slotUIs.Length; i++)
        {
            if (i < slots.Count)
                slotUIs[i].SetSlot(slots[i]);
            else
                slotUIs[i].SetSlot(null);
        }
    }

    public void Open()
    {
        gameObject.SetActive(true);
        Refresh();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Toggle()
    {
        bool isOpen = !gameObject.activeSelf;
        gameObject.SetActive(isOpen);

        if (isOpen)
        {
            Refresh();
        }
    }
}