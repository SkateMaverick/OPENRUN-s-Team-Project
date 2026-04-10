using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData itemData;
    public int amount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (InventoryManager.Instance == null)
            return;

        bool success = InventoryManager.Instance.AddItem(itemData, amount);

        if (success)
        {
            Destroy(gameObject);
        }
    }
}