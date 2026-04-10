using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI countText;

    public void SetSlot(InventorySlotData data)
    {
        if (data == null || data.item == null)
        {
            icon.enabled = false;
            countText.text = "";
            return;
        }

        icon.enabled = true;
        icon.sprite = data.item.icon;
        countText.text = data.amount > 1 ? data.amount.ToString() : "";
    }
}