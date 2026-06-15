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

    public void TakeDamage(int damage)
    {
        if (IsDead)
            return;

        CurrentHealth -= damage;

        Debug.Log($"{gameObject.name} ������: {damage}, ���� ü��: {CurrentHealth}");

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

        Debug.Log($"{gameObject.name} ��� �� ������ ����");

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