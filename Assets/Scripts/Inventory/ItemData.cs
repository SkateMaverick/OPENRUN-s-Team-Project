using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    public int id;
    public string itemName;
    public Sprite icon;

    [TextArea]
    public string description;

    public bool stackable = true;
    public int maxStack = 99;
}