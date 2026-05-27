using UnityEngine;
using Photon.Pun;

// Bullet에 맞았을 때, 자신 게임오브젝트를 파괴
public class DestroyableProp : MonoBehaviourPun
{
    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 콜라이더가 Bullet이면 자신을 파괴
        if (other.TryGetComponent(out IBullet bullet))
        {
            photonView.RPC("Destroyed", RpcTarget.All);
        }
    }

    [PunRPC]
    private void Destroyed()
    {
        Destroy(gameObject);
    }
}
