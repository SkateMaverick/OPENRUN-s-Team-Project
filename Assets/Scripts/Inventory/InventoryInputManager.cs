using UnityEngine;

public class InventoryInputManager : MonoBehaviour
{
    public GameObject inventoryPanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        }
    }
}