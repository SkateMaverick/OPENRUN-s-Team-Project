using UnityEngine;

public class EnemyHealth : LivingEntity
{
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }

    protected override void Die()
    {
        base.Die();
        
        print("적 사망");
    }
}
