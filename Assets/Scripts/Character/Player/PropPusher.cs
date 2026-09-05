using UnityEngine;
using Photon.Pun;

// 큐의 밀기 능력
public class PropPusher : MonoBehaviourPun
{
    public float pushPower = 100f;

    // Character Controller에 무언가가 충돌되었을 때 매 프레임 호출
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // 충돌된 콜라이더가 밀어질 수 있다면
        if (hit.gameObject.TryGetComponent(out IPushable prop))
        {
            if (!photonView.IsMine) return;
            print("부딪힘");
            // 이 게임 오브젝트가 움직이던 방향
            Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z).normalized;
            
            // 충돌된 리지드바디를 밀어냄
            prop.Push(pushDirection, pushPower);
        }
    }
}
