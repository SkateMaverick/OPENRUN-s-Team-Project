using UnityEngine;

/// <summary>
/// 플레이어블 캐릭터 위에 다른 플레이어블 캐릭터가 올라왔을 때, 아래에 있는 캐릭터가 움직이면 위에 있는 캐릭터도 움직이도록 하는 스크립트
/// </summary>
public class MovementTransfer : MonoBehaviour
{
    private Rigidbody _myRb; // 내(운반자) 리지드바디
    private Rigidbody _passengerRb; // 위에 탄 승객의 리지드바디
    private Vector3 _lastPosition;

    private void Awake()
    {
        // 1. 내 리지드바디를 미리 찾아둠
        _myRb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // 2. 초기 위치도 진짜 물리 좌표로 저장
        _lastPosition = _myRb.position; 
    }

    private void FixedUpdate()
    {
        // 3. transform.position이 아닌 Rigidbody.position으로 완벽한 물리 변위 계산!
        Vector3 deltaMovement = _myRb.position - _lastPosition;

        if (_passengerRb != null && deltaMovement != Vector3.zero)
        {
            _passengerRb.MovePosition(_passengerRb.position + deltaMovement);
        }

        // 4. 다음 프레임 계산을 위해 현재 물리 위치 갱신
        _lastPosition = _myRb.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        print("부딪힘");
        if (other.TryGetComponent(out Rigidbody rb))
        {
            _passengerRb = rb;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        print("나감");
        if (other.TryGetComponent(out Rigidbody rb) && _passengerRb == rb)
        {
            _passengerRb = null;
        }
    }
}
