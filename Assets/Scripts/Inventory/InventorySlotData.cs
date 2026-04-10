[System.Serializable]
public class InventorySlotData
{
    public ItemData item;
    public int amount;

    public bool IsEmpty()
    {
        return item == null;
    }

    public void Clear()
    {
        item = null;
        amount = 0;
    }
}