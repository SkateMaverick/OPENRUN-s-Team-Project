using System;
using UnityEngine;

public abstract class LivingEntity : MonoBehaviour
{
    //public event Action OnHit;
    //public event Action OnDeath;

    [SerializeField] private float startingHealth = 100f; // 시작 체력
    private float _health; // 현재 체력

    protected void Awake()
    {
        _health = startingHealth;
    }
    
    // 외부에서 대미지를 받는 메서드
    public virtual void ApplyDamage(float damage)
    {
        // 받은 데미지만큼 현재 체력 감소
        _health -= damage;
        
        // '현재 체력 <= 0' 이라면
        if (_health <= 0)
        {
            // 사망(파괴)
            Die();
        }
    }

    // 사망(또는 파괴) 처리
    protected virtual void Die()
    {
        
    }
}
