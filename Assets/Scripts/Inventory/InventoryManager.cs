using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance
    {
        get
        {
            if (_isShuttingDown) return null;

            if (_instance == null)
            {
                _instance = FindFirstObjectByType<InventoryManager>();

                if (_instance == null && Application.isPlaying)
                {
                    GameObject go = new GameObject("InventoryManager");
                    _instance = go.AddComponent<InventoryManager>();
                }
            }
            return _instance;
        }
        private set => _instance = value;
    }

    private static InventoryManager _instance;
    private static bool _isShuttingDown = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticState()
    {
        _instance = null;
        _isShuttingDown = false;
    }

    public int maxSlotCount = 20;
    public List<InventorySlotData> slots = new List<InventorySlotData>();

    public event Action onInventoryChanged;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        InitSlots();
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    private void OnApplicationQuit()
    {
        _isShuttingDown = true;
    }

    private void InitSlots()
    {
        if (slots.Count > 0) return;

        for (int i = 0; i < maxSlotCount; i++)
        {
            slots.Add(new InventorySlotData());
        }
    }

    public int GetItemCount(ItemData item)
    {
        if (item == null) return 0;
        int total = 0;
        for (int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].IsEmpty() && slots[i].item == item)
            {
                total += slots[i].amount;
            }
        }
        return total;
    }

    public bool HasItem(ItemData item, int amount = 1)
    {
        return GetItemCount(item) >= amount;
    }

    public bool RemoveItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;
        if (!HasItem(item, amount)) return false;

        int remaining = amount;
        for (int i = slots.Count - 1; i >= 0; i--)
        {
            if (!slots[i].IsEmpty() && slots[i].item == item)
            {
                if (slots[i].amount <= remaining)
                {
                    remaining -= slots[i].amount;
                    slots[i].Clear();
                }
                else
                {
                    slots[i].amount -= remaining;
                    remaining = 0;
                }

                if (remaining <= 0)
                {
                    break;
                }
            }
        }

        onInventoryChanged?.Invoke();
        return true;
    }

    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0)
            return false;

        InitSlots();

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