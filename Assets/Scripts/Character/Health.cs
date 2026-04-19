using UnityEngine;
using Photon.Pun;

public class Health : MonoBehaviourPun, IDamageable
{
    [SerializeField] private float maxHealth = 100f;

    public float CurrentHealth { get; private set; }

    private bool _isDead;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (_isDead)
            return;

        CurrentHealth -= damage;

        if (CurrentHealth <= 0f)
        {
            CurrentHealth = 0f;
            Die();
        }
    }

    private void Die()
    {
        if (_isDead)
            return;

        _isDead = true;

        if (photonView != null)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}