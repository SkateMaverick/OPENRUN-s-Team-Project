using System;
using UnityEngine;

// 
public class NetworkBullet : MonoBehaviour, IBullet
{
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Setup(Vector3 targetPosition, float speed)
    {
        // 목표 지점까지의 방향
        Vector3 direction = (targetPosition - transform.position).normalized;

        // 리지드바디에 속도를 할당
        _rb.linearVelocity = direction * speed;
        
        Destroy(gameObject, 3.0f);
    }

    // 물리 충돌 시 파괴 (벽이나 적에 닿았을 때)
    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
