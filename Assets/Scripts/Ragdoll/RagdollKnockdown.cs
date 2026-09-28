using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

// 강하게 밀려났을 때 몸통은 루트 Rigidbody로 구르고, 다리만 레그돌로 흐느적거리게 함
// 다리 뼈(Leg.L/R, .001, .002)에 Rigidbody + CapsuleCollider + CharacterJoint가 붙어 있어야 함
public class RagdollKnockdown : MonoBehaviour
{
    [Header("넉다운 시간")]
    [SerializeField] private float minKnockdownTime = 1f; // 최소한 이 시간 동안은 구름
    [SerializeField] private float maxKnockdownTime = 4f; // 이 시간이 지나면 멈추지 않았어도 일어남
    [SerializeField] private float settleSpeed = 0.5f; // 이 속도 이하면 멈춘 것으로 판단
    [SerializeField] private float settleAngularSpeed = 1f; // 이 회전 속도 이하면 멈춘 것으로 판단
    [SerializeField] private float maxAngularVelocity = 20f; // 구를 때 허용할 최대 회전 속도 (Rigidbody 기본값 7은 너무 느림)
    [SerializeField] private float legMaxDepenetrationVelocity = 2f; // 다리 콜라이더가 바닥에 파묻힌 채 켜졌을 때 튕겨나가는 최대 속도
    [SerializeField] private float centerOfMassHeight = 1f; // 루트 기준 무게중심 높이. 구를 때 이 점을 중심으로 돔 (높을수록 머리 쪽을 축으로 돌아 발이 크게 휘둘러짐)

    [Header("일어나기")]
    [SerializeField] private float standUpDuration = 0.4f; // 몸을 똑바로 세우는 데 걸리는 시간
    [SerializeField] private float legBlendDuration = 0.25f; // 흐트러진 다리 자세에서 애니메이션 자세로 돌아가는 시간

    [Header("참조")]
    [SerializeField] private Collider legSubstituteCollider; // 평소 다리 대신 쓰는 콜라이더. 넉다운 중엔 진짜 다리 콜라이더가 대신함

    private Rigidbody _rigidbody;
    private PlayerMovement _movement;
    private Animator _animator;
    private RigBuilder _rigBuilder;
    private WalkingIK _walkingIK;
    private PlayerAnimator _playerAnimator;

    private Rigidbody[] _legRigidbodies; // 다리 뼈의 Rigidbody들
    private Collider[] _legColliders; // 다리 뼈의 콜라이더들
    private Collider[] _bodyColliders; // 루트에 붙은 몸통 콜라이더들
    private CharacterJoint[] _hipJoints; // 루트에 연결된 허벅지 관절들
    private RigidbodyConstraints _originalConstraints;
    private float _standingBottomOffset; // 서 있을 때 루트 위치에서 콜라이더 맨 아래까지의 높이

    // 다리 자세 블렌딩용: 일어날 때 레그돌 자세를 저장해두고 애니메이션 자세와 섞음
    private Quaternion[] _savedLegRotations;
    private float _legBlendWeight; // 1이면 저장한 레그돌 자세, 0이면 애니메이션 자세

    public bool IsKnockedDown { get; private set; }

    private void Reset()
    {
        legSubstituteCollider = GetComponent<SphereCollider>();
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _movement = GetComponent<PlayerMovement>();
        _animator = GetComponent<Animator>();
        _rigBuilder = GetComponent<RigBuilder>();
        _walkingIK = GetComponent<WalkingIK>();
        _playerAnimator = GetComponent<PlayerAnimator>();
        _bodyColliders = GetComponents<Collider>();

        _rigidbody.maxAngularVelocity = maxAngularVelocity;
        // 자동 계산하면 큰 몸통 BoxCollider 때문에 무게중심이 머리 쪽으로 올라가므로 직접 지정
        // 한 번 지정하면 콜라이더를 껐다 켜도 다시 계산되지 않음
        _rigidbody.centerOfMass = new Vector3(0f, centerOfMassHeight, 0f);

        // 루트를 제외한 자식 Rigidbody = 다리 뼈
        Rigidbody[] allBodies = GetComponentsInChildren<Rigidbody>(true);
        _legRigidbodies = new Rigidbody[allBodies.Length - 1];
        _legColliders = new Collider[_legRigidbodies.Length];
        int index = 0;
        foreach (Rigidbody body in allBodies)
        {
            if (body == _rigidbody) continue;
            _legRigidbodies[index] = body;
            _legColliders[index] = body.GetComponent<Collider>();
            body.maxDepenetrationVelocity = legMaxDepenetrationVelocity;
            index++;
        }
        _savedLegRotations = new Quaternion[_legRigidbodies.Length];

        // 허벅지 관절만 따로 모아둠 (루트와 연결된 관절)
        var hipJoints = new List<CharacterJoint>();
        foreach (CharacterJoint joint in GetComponentsInChildren<CharacterJoint>(true))
            if (joint.connectedBody == _rigidbody) hipJoints.Add(joint);
        _hipJoints = hipJoints.ToArray();

        SetLegRagdoll(false);
    }

    private void Start()
    {
        // 서 있는 자세 기준으로 측정. 일어날 때 이 높이만큼 띄워서 바닥에 파묻히지 않게 함
        _standingBottomOffset = _rigidbody.position.y - GetBodyBottomY();
    }

    // 넉다운 중에 비활성화되면 코루틴이 멈추므로, 잠긴 상태로 남지 않게 원래대로 되돌림
    private void OnDisable()
    {
        if (!IsKnockedDown) return;

        _rigidbody.isKinematic = false;
        _rigidbody.constraints = _originalConstraints;
        SetLegRagdoll(false);
        _legBlendWeight = 0f;
        if (legSubstituteCollider != null) legSubstituteCollider.enabled = true;

        _animator.enabled = true;
        _rigBuilder.enabled = true;
        _playerAnimator.enabled = true;
        _walkingIK.enabled = true;
        _movement.IsControlLocked = false;
        IsKnockedDown = false;
    }

    // 장애물 쪽에서 호출. force: 밀어내는 속도, torque: 회전 속도 (둘 다 질량 무시)
    public void Knockdown(Vector3 force, Vector3 torque)
    {
        // 이미 구르는 중이면 무시. 다리 콜라이더가 켜지면서 같은 장애물에 여러 번 닿아 힘이 쌓이는 것을 막음
        if (IsKnockedDown) return;

        StartCoroutine(KnockdownRoutine(force, torque));
    }

    private IEnumerator KnockdownRoutine(Vector3 force, Vector3 torque)
    {
        IsKnockedDown = true;

        // 조작과 애니메이션 정지 (뼈를 움직이는 주체를 물리로 넘김)
        _movement.IsControlLocked = true;
        _playerAnimator.enabled = false;
        _walkingIK.enabled = false;
        _rigBuilder.enabled = false;
        _animator.enabled = false;

        // 몸통이 자유롭게 구를 수 있도록 회전 고정 해제
        _originalConstraints = _rigidbody.constraints;
        _rigidbody.constraints = RigidbodyConstraints.None;

        if (legSubstituteCollider != null) legSubstituteCollider.enabled = false;
        SetLegRagdoll(true);

        _rigidbody.AddForce(force, ForceMode.VelocityChange);
        _rigidbody.AddTorque(torque, ForceMode.VelocityChange);

        // 다리가 제자리에 남지 않도록 넉백이 적용된 몸통 속도를 이어받게 함
        // AddForce는 다음 물리 스텝에야 반영되므로 넉백 후 속도를 직접 계산
        Vector3 bodyVelocity = _rigidbody.linearVelocity + force;
        Vector3 bodyAngularVelocity = _rigidbody.angularVelocity + torque;
        foreach (Rigidbody leg in _legRigidbodies)
            leg.linearVelocity = bodyVelocity + Vector3.Cross(bodyAngularVelocity, leg.position - _rigidbody.worldCenterOfMass);

        // 최소 시간은 무조건 구르고, 그 뒤엔 멈추거나 최대 시간이 될 때까지 대기
        float elapsed = 0f;
        while (elapsed < maxKnockdownTime &&
               (elapsed < minKnockdownTime ||
                _rigidbody.linearVelocity.magnitude > settleSpeed ||
                _rigidbody.angularVelocity.magnitude > settleAngularSpeed))
        {
            yield return new WaitForFixedUpdate();
            elapsed += Time.fixedDeltaTime;
        }

        // 다리의 현재(흐트러진) 자세를 저장하고 다시 애니메이션을 따라가게 함
        for (int i = 0; i < _legRigidbodies.Length; i++)
            _savedLegRotations[i] = _legRigidbodies[i].transform.localRotation;
        SetLegRagdoll(false);

        // 몸을 똑바로 세움. 바라보던 수평 방향은 유지
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        // 앞이나 뒤로 완전히 엎어져 forward가 거의 수직이면, 머리가 향하던 방향을 앞으로 사용
        if (forward.sqrMagnitude < 0.01f) forward = Vector3.ProjectOnPlane(transform.up, Vector3.up);
        Quaternion from = _rigidbody.rotation;
        Quaternion to = Quaternion.LookRotation(forward.normalized, Vector3.up);

        // 누운 채로 중심만 돌리면 콜라이더가 바닥을 파고들어 튕기므로, 서 있을 때 높이까지 같이 올림
        Vector3 fromPosition = _rigidbody.position;
        Vector3 toPosition = fromPosition;
        toPosition.y = Mathf.Max(fromPosition.y, GetBodyBottomY() + _standingBottomOffset);

        // 세우는 동안엔 kinematic으로 두어 중력이나 충돌 반발 없이 자세만 바꿈
        _rigidbody.isKinematic = true;
        for (float t = 0f; t < 1f; t += Time.fixedDeltaTime / standUpDuration)
        {
            _rigidbody.MovePosition(Vector3.Lerp(fromPosition, toPosition, t));
            _rigidbody.MoveRotation(Quaternion.Slerp(from, to, t));
            yield return new WaitForFixedUpdate();
        }
        _rigidbody.position = toPosition;
        _rigidbody.rotation = to;
        _rigidbody.constraints = _originalConstraints;
        _rigidbody.isKinematic = false;
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        if (legSubstituteCollider != null) legSubstituteCollider.enabled = true;

        // 애니메이션 재개. 다리는 LateUpdate에서 저장한 자세 -> 애니메이션 자세로 서서히 섞임
        // 블렌딩은 Animator가 켜진 이 시점부터 시작해야 함 (먼저 켜두면 세우는 동안 다 줄어들어버림)
        _animator.enabled = true;
        _rigBuilder.enabled = true;
        _playerAnimator.enabled = true;
        _legBlendWeight = 1f;
        // Animator가 다시 켜지면 기본 상태(Idle)부터 재생되므로 상태 머신도 Idle로 맞춤
        _playerAnimator.AnimationStateMachine.TransitionTo(_playerAnimator.AnimationStateMachine.IdleState);

        while (_legBlendWeight > 0f) yield return null;

        _walkingIK.enabled = true;
        _movement.IsControlLocked = false;
        IsKnockedDown = false;
    }

    private void LateUpdate()
    {
        if (_legBlendWeight <= 0f) return;

        // Animator가 이번 프레임 자세를 쓴 뒤에 저장한 레그돌 자세를 덮어 섞음
        for (int i = 0; i < _legRigidbodies.Length; i++)
        {
            Transform bone = _legRigidbodies[i].transform;
            bone.localRotation = Quaternion.Slerp(bone.localRotation, _savedLegRotations[i], _legBlendWeight);
        }

        _legBlendWeight = Mathf.MoveTowards(_legBlendWeight, 0f, Time.deltaTime / legBlendDuration);
    }

    // true: 다리를 물리로 움직임, false: 다리가 애니메이션을 따라감
    private void SetLegRagdoll(bool isRagdoll)
    {
        // 애니메이션으로 뼈를 움직이는 동안의 루트 Transform 기준 자세를 물리상 루트 자세 기준으로 옮기기 위한 행렬
        // 루트는 보간(Interpolate) 때문에 Transform과 물리 위치가 조금 다르므로 물리 위치에 맞춰 다리를 놓음
        Matrix4x4 transformToPhysics = Matrix4x4.TRS(_rigidbody.position, _rigidbody.rotation, Vector3.one) * transform.worldToLocalMatrix;
        Quaternion rotationToPhysics = _rigidbody.rotation * Quaternion.Inverse(transform.rotation);

        for (int i = 0; i < _legRigidbodies.Length; i++)
        {
            Rigidbody leg = _legRigidbodies[i];

            if (isRagdoll)
            {
                // kinematic인 동안 물리상 다리 위치가 실제 뼈 위치와 어긋나 있을 수 있으므로, 물리를 켜기 전에 뼈 위치로 맞춤
                // 어긋난 채로 켜면 관절이 멀리 떨어진 다리를 한 번에 끌어와 폭발하듯 튕겨나감
                leg.position = transformToPhysics.MultiplyPoint3x4(leg.transform.position);
                leg.rotation = rotationToPhysics * leg.transform.rotation;
            }

            // kinematic인 동안 보간을 켜두면, 달릴 때 물리상 다리 위치가 뼈를 따라가지 못하고 수백 m까지 벌어짐
            // 그래서 애니메이션을 따라갈 때는 끄고, 물리로 움직일 때만 켜서 부드럽게 보이게 함
            leg.interpolation = isRagdoll ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
            leg.isKinematic = !isRagdoll;
            _legColliders[i].enabled = isRagdoll;
        }

        // 평소엔 허벅지 관절을 루트에서 떼어둠
        // kinematic인 허벅지와 루트가 관절로 묶여 있으면 관절이 루트를 억지로 끌어당겨 튀기 때문
        // 다시 연결하면 현재 자세 기준으로 연결 위치가 새로 잡힘
        foreach (CharacterJoint joint in _hipJoints)
            joint.connectedBody = isRagdoll ? _rigidbody : null;

        // IgnoreCollision은 콜라이더를 껐다 켜면 풀리기 때문에 켤 때마다 다시 설정
        if (isRagdoll) IgnoreSelfCollisions();
    }

    // 다리 콜라이더가 몸통 콜라이더나 다른 다리 콜라이더와 부딪혀 떨리지 않도록 서로 무시
    private void IgnoreSelfCollisions()
    {
        foreach (Collider legCollider in _legColliders)
        {
            if (legCollider == null) continue;

            foreach (Collider bodyCollider in _bodyColliders)
                Physics.IgnoreCollision(legCollider, bodyCollider);

            foreach (Collider otherLeg in _legColliders)
                if (otherLeg != null && otherLeg != legCollider) Physics.IgnoreCollision(legCollider, otherLeg);
        }
    }

    // 켜져 있는 몸통 콜라이더 중 가장 낮은 지점의 높이
    private float GetBodyBottomY()
    {
        float bottom = float.MaxValue;
        foreach (Collider bodyCollider in _bodyColliders)
            if (bodyCollider.enabled && !bodyCollider.isTrigger) bottom = Mathf.Min(bottom, bodyCollider.bounds.min.y);

        return bottom == float.MaxValue ? _rigidbody.position.y : bottom;
    }
}
