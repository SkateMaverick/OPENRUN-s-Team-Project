using UnityEngine;

public class EnemyHealth : LivingEntity
{
    public override void ApplyDamage(float damage)
    {
        base.ApplyDamage(damage);
    }

    protected override void Die()
    {
        base.Die();
        
        print("적 사망");
    }
}
