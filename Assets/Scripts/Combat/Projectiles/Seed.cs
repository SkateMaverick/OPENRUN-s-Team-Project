using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;

/// <summary>
/// 노아가 쏘는 투사체.
/// </summary>
public class Seed : MonoBehaviour, IProjectile
{
    [SerializeField] private LayerMask whatIsTarget; // TakeDamage()를 실행할 수 있는 대상들
    [SerializeField] private int attackDamage = 10; // 씨앗 피격시 대미지
    
    private Rigidbody _rigidbody;
    private bool _hasHit = false;

    private void Awake()
    {
        #region 초기화
        _rigidbody = GetComponent<Rigidbody>();
        
        _rigidbody.useGravity = false;
        #endregion
    }

    private void OnCollisionEnter(Collision other)
    {
        if (_hasHit) return;
        _hasHit = true;

        if (((1 << other.gameObject.layer) & whatIsTarget) != 0)
        {
            IDamageable damageable = GetActiveDamageable(other.gameObject);

            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
            }
        }
        if (Application.isPlaying)
            Destroy(gameObject);
        else
            DestroyImmediate(gameObject);
    }

    public void Launch(float speed)
    {
        // 씨앗 속도
        Vector3 velocity = transform.forward * speed;
        _rigidbody.AddForce(velocity, ForceMode.VelocityChange);
    }

    private IDamageable GetActiveDamageable(GameObject target)
    {
        if (target == null) return null;

        var damageables = target.GetComponents<IDamageable>();
        foreach (var d in damageables)
        {
            if (d is Behaviour b && !b.isActiveAndEnabled)
                continue;
            return d;
        }

        var parentDamageables = target.GetComponentsInParent<IDamageable>();
        foreach (var d in parentDamageables)
        {
            if (d is Behaviour b && !b.isActiveAndEnabled)
                continue;
            return d;
        }

        if (damageables.Length > 0) return damageables[0];
        if (parentDamageables.Length > 0) return parentDamageables[0];

        return null;
    }
}
