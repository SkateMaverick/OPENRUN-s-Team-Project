using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public int maxSlotCount = 20;
    public List<InventorySlotData> slots = new List<InventorySlotData>();

    public event Action onInventoryChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitSlots();
    }

    private void InitSlots()
    {
        if (slots.Count > 0) return;

        for (int i = 0; i < maxSlotCount; i++)
        {
            slots.Add(new InventorySlotData());
        }
    }

    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0)
            return false;

        if (item.stackable)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (!slots[i].IsEmpty() &&
                    slots[i].item == item &&
                    slots[i].amount < item.maxStack)
                {
                    int availableSpace = item.maxStack - slots[i].amount;
                    int addAmount = Mathf.Min(availableSpace, amount);

                    slots[i].amount += addAmount;
                    amount -= addAmount;

                    if (amount <= 0)
                    {
                        onInventoryChanged?.Invoke();
                        return true;
                    }
                }
            }
        }

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsEmpty())
            {
                int addAmount = item.stackable ? Mathf.Min(item.maxStack, amount) : 1;

                slots[i].item = item;
                slots[i].amount = addAmount;
                amount -= addAmount;

                if (amount <= 0)
                {
                    onInventoryChanged?.Invoke();
                    return true;
                }
            }
        }

        onInventoryChanged?.Invoke();
        Debug.Log("인벤토리가 가득 찼습니다.");
        return false;
    }
}