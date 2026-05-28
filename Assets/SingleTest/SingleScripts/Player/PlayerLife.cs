using UnityEngine;

public class PlayerLife : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Respawn")]
    [SerializeField] private float respawnDelay = 0.5f;

    public float CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }

    private void Awake()
    {
        ResetLife();
    }

    public void TakeDamage(float damage)
    {
        if (IsDead)
            return;

        CurrentHealth -= damage;

        Debug.Log($"{gameObject.name} 데미지: {damage}, 현재 체력: {CurrentHealth}");

        if (CurrentHealth <= 0f)
        {
            Kill();
        }
    }

    public void Kill()
    {
        if (IsDead)
            return;

        IsDead = true;
        CurrentHealth = 0f;

        Debug.Log($"{gameObject.name} 사망 → 리스폰 예정");

        Invoke(nameof(Respawn), respawnDelay);
    }

    private void Respawn()
    {
        if (SingleSpawnManager.Instance != null)
        {
            SingleSpawnManager.Instance.RespawnPlayer(transform);
        }
    }

    public void ResetLife()
    {
        CurrentHealth = maxHealth;
        IsDead = false;
    }
}