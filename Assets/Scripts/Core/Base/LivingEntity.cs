using System;
using UnityEngine;

public abstract class LivingEntity : MonoBehaviour, IDamageable
{
    //public event Action OnHit;
    //public event Action OnDeath;

    [SerializeField] private float startingHealth = 100f; // 시작 체력
    private float _health; // 현재 체력
    private bool _initialized;

    public float StartingHealth => startingHealth;
    public float Health => _initialized ? Mathf.Max(0f, _health) : startingHealth;
    public event Action<float, float> OnHealthChanged;

    protected virtual void Awake()
    {
        InitHealth();
    }

    private void InitHealth()
    {
        if (_initialized) return;
        _health = startingHealth;
        _initialized = true;
        OnHealthChanged?.Invoke(_health, startingHealth);
    }
    
    // 외부에서 대미지를 받는 메서드
    public virtual void TakeDamage(int damage = 0)
    {
        if (!_initialized)
        {
            InitHealth();
        }

        // 받은 데미지만큼 현재 체력 감소
        _health -= damage;
        OnHealthChanged?.Invoke(_health, startingHealth);
        
        // '현재 체력 <= 0' 이라면
        if (_health <= 0)
        {
            // 사망(파괴)
            Die();
        }
    }

    public virtual void Heal(float amount)
    {
        if (!_initialized)
        {
            InitHealth();
        }

        _health = Mathf.Min(_health + amount, startingHealth);
        OnHealthChanged?.Invoke(_health, startingHealth);
    }

    // 사망(또는 파괴) 처리
    protected virtual void Die()
    {
        
    }
}
