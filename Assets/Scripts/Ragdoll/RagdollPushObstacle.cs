using System.Collections.Generic;
using UnityEngine;

// 테스트용 장애물: 제자리를 기준으로 자기 Z축 방향으로 앞뒤 왕복하다가 닿은 캐릭터를 날려버림
[RequireComponent(typeof(Rigidbody))]
public class TestPushObstacle : MonoBehaviour
{
    [Header("왕복 이동")]
    [SerializeField] private float moveRange = 3f; // 제자리에서 앞뒤로 움직이는 거리 (±moveRange)
    [SerializeField] private float moveSpeed = 4f; // 초당 이동 거리

    [Header("밀어내기")]
    [SerializeField] private float pushPower = 6f; // 수평으로 밀어내는 속도
    [SerializeField] private float liftPower = 2f; // 위로 띄우는 속도
    [SerializeField] private float spinPower = 5f; // 구르게 만드는 회전 속도
    [SerializeField] private float hitCooldown = 1f; // 같은 캐릭터를 한 번 친 뒤 다시 칠 수 있기까지의 시간

    private Rigidbody _rigidbody;
    private Vector3 _startPosition;
    private Vector3 _moveAxis;
    private readonly Dictionary<RagdollKnockdown, float> _lastHitTimes = new(); // 캐릭터별 마지막으로 친 시간

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        // 스크립트로만 움직이고, 캐릭터에게 밀리지 않도록 kinematic
        _rigidbody.isKinematic = true;
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        _startPosition = transform.position;
        _moveAxis = transform.forward; // 자기 Z축
    }

    private void FixedUpdate()
    {
        // -moveRange ~ +moveRange 사이를 왕복
        float offset = Mathf.PingPong(Time.time * moveSpeed, moveRange * 2f) - moveRange;
        _rigidbody.MovePosition(_startPosition + _moveAxis * offset);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 다리 뼈 콜라이더가 먼저 닿을 수도 있으므로 부모에서 찾음
        RagdollKnockdown knockdown = collision.collider.GetComponentInParent<RagdollKnockdown>();
        if (knockdown == null || knockdown.IsKnockedDown) return;

        if (_lastHitTimes.TryGetValue(knockdown, out float lastHitTime) && Time.time - lastHitTime < hitCooldown) return;
        _lastHitTimes[knockdown] = Time.time;

        // 장애물 중심에서 캐릭터 쪽으로 향하는 수평 방향으로 밀어냄
        Vector3 pushDirection = knockdown.transform.position - transform.position;
        pushDirection.y = 0f;
        if (pushDirection.sqrMagnitude < 0.001f) pushDirection = _moveAxis;
        pushDirection.Normalize();

        Vector3 force = pushDirection * pushPower + Vector3.up * liftPower;
        // 밀리는 방향으로 앞구르기 하듯 회전 + 약간의 무작위 회전
        Vector3 torque = Vector3.Cross(Vector3.up, pushDirection) * spinPower + Random.insideUnitSphere * (spinPower * 0.3f);

        knockdown.Knockdown(force, torque);
    }
}
