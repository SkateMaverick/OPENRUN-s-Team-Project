using UnityEngine;

public class PlayerHealth : LivingEntity
{
    // 플레이어가 대미지를 받았을 때의 처리
    public override void ApplyDamage(float damage)
    {
        base.ApplyDamage(damage);
        
        // 애니메이션, ui 등 변경사항
    }

    // 플레이어가 체력이 다했을 때의 처리
    protected override void Die()
    {
        // 애니메이션, ui 등 변경사항
    }
}
