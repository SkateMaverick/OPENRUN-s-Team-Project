using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public InventorySlotUI[] slotUIs;

    private void Start()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.onInventoryChanged += Refresh;
        }

        Refresh();
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.onInventoryChanged -= Refresh;
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
        gameObject.SetActive(!gameObject.activeSelf);

        if (gameObject.activeSelf)
            Refresh();
    }
}