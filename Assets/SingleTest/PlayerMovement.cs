using UnityEngine;

public class PlayerMovement : MonoBehaviour, IControllable
{
    public Camera followCam;
    public float baseSpeed = 4f;
    public float sprintMultiplier = 1.5f;
    public float turnInterpolationRatio = 10f;
    public float extraGravity = 2.5f;
    
    private Rigidbody _rigidbody;
    private float _turnSmoothVelocity;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // 더 강한 중력
        _rigidbody.AddForce(Physics.gravity * (extraGravity - 1f), ForceMode.Acceleration);
    }

    public void HandleCharacterControl(Vector2 moveInput, bool isSprint)
    {
        // 달리는 상태라면 달리기 속도를 사용
        float moveSpeed = isSprint ? baseSpeed * sprintMultiplier : baseSpeed;
        
        // 카메라의 정면과 오른쪽의 방향 벡터
        Vector3 camForward = followCam.transform.forward;
        Vector3 camRight = followCam.transform.right;

        // 방향이 위아래를 쳐다보진 않도록 조절
        camForward.y = 0;
        camRight.y = 0;
        
        // 조절로 인해 벡터의 길이가 1보다 작아질 수 있기 때문에 정규화
        camForward.Normalize();
        camRight.Normalize();
        
        // 실제 이동 방향
        Vector3 moveDirection = (camForward * moveInput.y) + (camRight * moveInput.x);
        
        Move(moveDirection, moveSpeed);
        Rotate(moveDirection);
    }

    private void Move(Vector3 direction, float speed)
    {
        // 목표 속도
        Vector3 targetVelocity = direction * speed;
        // 현재 속도
        Vector3 currentVelocity = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);
        // 현재 속도에서 목표 속도까지의 차이 (= 목표 속도 - 현재 속도)
        Vector3 velocityDifference = targetVelocity - currentVelocity;
        // 차이만큼 현재 속도에 더함 (현재 속도가 5고, 목표 속도가 3이면 3-5=-2 -> 5+(-2)=3)
        _rigidbody.AddForce(velocityDifference, ForceMode.VelocityChange);
    }
    
    private void Rotate(Vector3 direction)
    {
        // 'Look rotation viewing vector is zero' 로그 방지용
        if (direction == Vector3.zero) return;
        
        // 들어온 방향을 가리키는 쿼터니언
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        // 구한 쿼터니언으로 회전
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * turnInterpolationRatio);
    }
}
