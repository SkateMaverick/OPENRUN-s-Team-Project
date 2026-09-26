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
            IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
            if (damageable == null)
            {
                damageable = other.gameObject.GetComponentInParent<IDamageable>();
            }

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
}
