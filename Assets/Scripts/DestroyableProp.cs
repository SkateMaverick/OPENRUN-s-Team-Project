using UnityEngine;
using Photon.Pun;

// Bullet에 맞았을 때, 파티클을 생성하고 자신 게임오브젝트를 파괴
public class DestroyableProp : MonoBehaviourPun
{
    [Header("Destroy Effect")]
    [SerializeField] private GameObject destroyParticlePrefab;
    [SerializeField] private Transform effectSpawnPoint;

    private bool isDestroyed;

    private void OnTriggerEnter(Collider other)
    {
        if (isDestroyed)
            return;

        // 충돌한 콜라이더가 Bullet이면 자신을 파괴
        if (other.TryGetComponent(out IBullet bullet))
        {
            isDestroyed = true;
            photonView.RPC(nameof(Destroyed), RpcTarget.All);
        }
    }

    [PunRPC]
    private void Destroyed()
    {
        SpawnDestroyParticle();
        Destroy(gameObject);
    }

    private void SpawnDestroyParticle()
    {
        if (destroyParticlePrefab == null)
            return;

        Vector3 spawnPosition = effectSpawnPoint != null
            ? effectSpawnPoint.position
            : transform.position;

        Instantiate(
            destroyParticlePrefab,
            spawnPosition,
            Quaternion.identity
        );
    }
}