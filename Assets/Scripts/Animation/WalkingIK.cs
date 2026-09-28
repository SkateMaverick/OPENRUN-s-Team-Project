using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Random = UnityEngine.Random;

public class WalkingIK : MonoBehaviour
{
    [SerializeField] private WalkPoseSet poseSet;

    [SerializeField] private TwoBoneIKConstraint leftLegIK;
    [SerializeField] private TwoBoneIKConstraint rightLegIK;
    // poseSet의 duration들이 자연스러워 보이는 기준 속도(m/s). 이 속도로 움직일 때 사이클이 원래 타이밍대로 재생됨
    [SerializeField, Min(0.01f)] private float referenceSpeed = 4f;
    [SerializeField, Min(0.01f)] private float weightBlendDuration = 0.15f; // IK가 켜지거나 꺼지는 데 걸리는 시간
    
    private Rigidbody _rigidbody;
    private float[] _keyframeEndTimes; // 각 키프레임이 끝나는 시각을 누적하는 룩업 테이블

    private float _cycleDuration; // 걷는 한 사이클의 총 지속시간
    private float _elapsedTime;
    // 걷는 중인지 아닌지 (1이면 걷기, 0이면 아님)
    // bool로 하지 않는 이유는 TwoBoneIKConstraint의 weight 필드에 넣어질 값이기 때문
    private float _targetWeight;
    private float _currentWeight; // 현재 TwoBoneIKConstraint의 weight
    // 이번 사이클에서 먼저 나갈 다리가 어느 다리일지 판별. 임의로 왼쪽 다리를 기준으로 삼음
    // true: poseSet 데이터 그대로 사용 (첫 포즈에서 왼발이 먼저 앞으로 나감)
    // false: x축만 반전시켜 오른발 리드 포즈로 변환해 사용 (첫 포즈에서 오른발이 먼저 앞으로 나감)
    private bool _leftLegLeads;

    // IK 움직임 시작
    public void ActivateIK() => _targetWeight = 1f;
    // IK 움직임 중지
    public void DeactivateIK() => _targetWeight = 0f;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _keyframeEndTimes = new float[poseSet.keyframes.Length];
        for (int i = 0; i < poseSet.keyframes.Length; i++)
        {
            _cycleDuration += poseSet.keyframes[i].duration;
            _keyframeEndTimes[i] = _cycleDuration;
        }
        
        _leftLegLeads = Random.Range(0, 2) == 0; // 어느 발부터 내밀지 
    }

    private void LateUpdate()
    {
        // 정확히 도달해야 하기 때문에 Lerp 대신 MvoeTowards
        _currentWeight = Mathf.MoveTowards(_currentWeight, _targetWeight, Time.deltaTime / weightBlendDuration);

        leftLegIK.weight = _currentWeight;
        rightLegIK.weight = _currentWeight;
        
        // _currentWeight가 0이면 ik 움직임이 없어야 하기 때문에
        if (_currentWeight <= 0f) return;
        
        WalkPoseSet.WalkKeyframe[] keyframes = poseSet.keyframes;

        // 수평 속도만 사용. y를 빼는 이유는 낙하나 상승 중에 다리가 움직이면 어색하기 때문
        Vector3 velocity = _rigidbody.linearVelocity;
        float horizontalSpeed = new Vector2(velocity.x, velocity.z).magnitude;

        // 시간이 아니라 이동한 거리에 비례해 사이클을 진행시킴
        // 속도 0 -> 위상 정지(다리 멈춤), 속도가 referenceSpeed의 2배 -> 걸음도 2배 빠르게
        _elapsedTime += Time.deltaTime * (horizontalSpeed / referenceSpeed);
        
        // 한 프레임에 사이클을 두 번 이상 넘길 수 있어 while로 변경.
        // if로 두면 _elapsedTime이 _cycleDuration보다 큰 채로 남아 아래 index가 배열 범위를 넘어감
        while (_elapsedTime >= _cycleDuration)
        {
            _elapsedTime -= _cycleDuration; // 한 사이클이 끝나면, 넘친 시간은 유지해서 시간을 초기화

            _leftLegLeads = Random.Range(0, 2) == 0; // 한 사이클이 끝나면, 어느 발이 리드할지  
        }

        int index = 0; // 현재 구간은 어느 키프레임이 시작점인지
        while (index < keyframes.Length && _elapsedTime >= _keyframeEndTimes[index])
            index++;

        // index의 다음 키프레임이 없다면 0번째를 가짐
        int next = (index + 1) % keyframes.Length;

        float segmentStartTime = index == 0 ? 0f : _keyframeEndTimes[index - 1];
        
        float t = (_elapsedTime - segmentStartTime) / keyframes[index].duration;
        
        TwoBoneIKConstraint leadIK = _leftLegLeads ? leftLegIK : rightLegIK;
        TwoBoneIKConstraint followIK = _leftLegLeads ? rightLegIK : leftLegIK;

        WalkPoseSet.WalkKeyframe from = keyframes[index];
        WalkPoseSet.WalkKeyframe to = keyframes[next];
        
        ApplyLeg(leadIK, Vector3.Lerp(from.leadTargetPosition, to.leadTargetPosition, t),
            Vector3.Lerp(from.leadHintPosition, to.leadHintPosition, t), !_leftLegLeads);
        ApplyLeg(followIK, Vector3.Lerp(from.followTargetPosition, to.followTargetPosition, t),
            Vector3.Lerp(from.followHintPosition, to.followHintPosition, t), !_leftLegLeads);
    }

    // 파라미터로 받은 ik의 target, hint의 위치를 변경
    private void ApplyLeg(TwoBoneIKConstraint ik, Vector3 targetPosition, Vector3 hintPosition, bool mirrored)
    {
        // 걷는 ik 애니메이션은 x축만 반전하면 필드의 _leftLegLeads가 참이든 아니든 반전된 애니메이션 연출이 가능하기 때문
        if (mirrored)
        {
            targetPosition.x = -targetPosition.x;
            hintPosition.x = -hintPosition.x;
        }
        
        ik.data.target.localPosition = targetPosition;
        ik.data.hint.localPosition = hintPosition;
    }
}
