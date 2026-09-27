using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Potion Item Data", fileName = "NewPotionItemData")]
public class PotionItemData : ItemData
{
    [Header("Potion Properties")]
    [Tooltip("Amount of HP restored when consumed")]
    public float healAmount = 50f;

    [Tooltip("Cooldown in seconds before another potion can be consumed")]
    public float cooldownDuration = 2f;

    [Tooltip("Audio clip played when consumed")]
    public AudioClip useSound;

    [Tooltip("Can this potion be used when the player is already at full health?")]
    public bool allowAtFullHealth = false;

    public virtual bool CanUse(LivingEntity target)
    {
        if (target == null) return false;
        if (!allowAtFullHealth && target.Health >= target.StartingHealth)
        {
            return false;
        }
        return true;
    }

    public virtual bool Use(LivingEntity target)
    {
        if (!CanUse(target)) return false;
        target.Heal(healAmount);
        return true;
    }
}
