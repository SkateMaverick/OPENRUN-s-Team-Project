using UnityEngine;

public class MovementTransfer : MonoBehaviour
{
    [Header("탑승 판정 전용 콜라이더")]
    [SerializeField] private Collider rideTriggerCollider;

    [Header("탑승 가능한 대상 레이어")]
    [SerializeField] private LayerMask passengerLayer;

    [Header("플레이어 상태 참조")]
    [SerializeField] private PlayerController playerController;

    private Rigidbody _myRb;
    private Rigidbody _passengerRb;
    
    // 본체와 승객의 '모든' 콜라이더를 담을 배열 (압사 완벽 방지용)
    private Collider[] _myColliders;
    private Collider[] _passengerColliders;
    
    private Vector3 _lastPosition;
    private bool _changedKinematicState = false;

    private void Awake()
    {
        _myRb = GetComponent<Rigidbody>();
        
        // 시작할 때 내 몸(자식 포함)에 달린 모든 콜라이더를 미리 싹 다 수집
        _myColliders = GetComponentsInChildren<Collider>();

        if (rideTriggerCollider != null)
        {
            rideTriggerCollider.isTrigger = true;
        }
    }

    private void Start()
    {
        _lastPosition = _myRb.position;
    }

    private void FixedUpdate()
    {
        Vector3 deltaMovement = _myRb.position - _lastPosition;

        if (IsBoxGolemActive())
        {
            if (_passengerRb != null && deltaMovement != Vector3.zero)
            {
                _passengerRb.MovePosition(_passengerRb.position + deltaMovement);
            }
        }
        else
        {
            ReleasePassenger();
        }

        _lastPosition = _myRb.position;
    }

    private void OnTriggerStay(Collider other)
    {
        if ((passengerLayer.value & (1 << other.gameObject.layer)) == 0) return;
        if (!IsBoxGolemActive()) return;

        if (rideTriggerCollider != null && rideTriggerCollider.bounds.Intersects(other.bounds))
        {
            if (_passengerRb == null && other.TryGetComponent(out Rigidbody rb))
            {
                if (rb == _myRb) return;

                _passengerRb = rb;
                
                // 승객의 몸(자식 포함)에 달린 모든 콜라이더를 수집
                _passengerColliders = rb.GetComponentsInChildren<Collider>();
                
                // [핵심] 두 캐릭터의 '모든' 콜라이더끼리 충돌을 강제로 무시시킴
                IgnoreAllCollisions(true);

                _passengerRb.isKinematic = true;
                _changedKinematicState = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Rigidbody rb) && _passengerRb == rb)
        {
            ReleasePassenger();
        }
    }

    private void OnDisable()
    {
        ReleasePassenger();
    }

    private void ReleasePassenger()
    {
        if (_passengerRb != null)
        {
            // 하차 시 모든 콜라이더의 충돌 판정을 다시 켜줌
            IgnoreAllCollisions(false);

            if (_changedKinematicState)
            {
                _passengerRb.isKinematic = false;
                _changedKinematicState = false;
            }
            
            _passengerRb = null;
            _passengerColliders = null;
        }
    }

    // 이중 for문을 돌며 두 캐릭터 간의 모든 콜라이더 충돌을 제어하는 함수
    private void IgnoreAllCollisions(bool ignore)
    {
        if (_myColliders == null || _passengerColliders == null) return;

        foreach (var myCol in _myColliders)
        {
            foreach (var passCol in _passengerColliders)
            {
                if (myCol != passCol && myCol.enabled && passCol.enabled)
                {
                    Physics.IgnoreCollision(myCol, passCol, ignore);
                }
            }
        }
    }

    private bool IsBoxGolemActive()
    {
        if (playerController == null) return false;
        return playerController.CurrentCharacterType == Enums.CharacterType.BoxGolem;
    }
}