using UnityEngine;

public class Arrow : MonoBehaviour, IProjectile
{
    // TakeDamage()를 실행할 수 있는 대상들
    [SerializeField] private LayerMask whatIsTarget;
    // 화살이 주는 대미지
    [SerializeField] private int attackDamage = 10;
    
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

        TryDamage(other.gameObject);
        if (Application.isPlaying)
            Destroy(gameObject);
        else
            DestroyImmediate(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasHit) return;

        if (((1 << other.gameObject.layer) & whatIsTarget) != 0)
        {
            _hasHit = true;
            TryDamage(other.gameObject);
            if (Application.isPlaying)
                Destroy(gameObject);
            else
                DestroyImmediate(gameObject);
        }
    }

    private void TryDamage(GameObject hitObj)
    {
        if (((1 << hitObj.layer) & whatIsTarget) != 0)
        {
            IDamageable damageable = hitObj.GetComponent<IDamageable>();
            if (damageable == null)
            {
                damageable = hitObj.GetComponentInParent<IDamageable>();
            }

            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
            }
        }
    }

    public void Launch(float speed)
    {
        // 화살 속도
        Vector3 velocity = transform.forward * speed;
        _rigidbody.AddForce(velocity, ForceMode.VelocityChange);
    }
}
