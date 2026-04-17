using UnityEngine;
using Photon.Pun;

public class ProjectileBullet : MonoBehaviourPun
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifeTime = 3f;

    private void Start()
    {
        DestroySelf(lifeTime);
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        IDamageable damageable = other.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        DestroySelf(0f);
    }

    private void DestroySelf(float delay)
    {
        if (PhotonNetwork.InRoom)
        {
            if (photonView != null && photonView.IsMine)
            {
                if (delay <= 0f) PhotonNetwork.Destroy(gameObject);
                else Invoke(nameof(DestroyNow), delay);
            }
        }
        else
        {
            Destroy(gameObject, delay);
        }
    }

    private void DestroyNow()
    {
        if (gameObject != null)
            PhotonNetwork.Destroy(gameObject);
    }
}