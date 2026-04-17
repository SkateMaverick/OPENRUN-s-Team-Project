using UnityEngine;
using Photon.Pun;

public class TurretWeapon : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private float fireRate = 1f;

    private float _lastFireTime;

    public bool CanFire()
    {
        return Time.time >= _lastFireTime + (1f / fireRate);
    }

    public void Fire(Vector3 direction)
    {
        if (!CanFire())
            return;

        _lastFireTime = Time.time;

        if (projectilePrefab == null || muzzlePoint == null)
            return;

        Quaternion rotation = Quaternion.LookRotation(direction.normalized);

        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.Instantiate(projectilePrefab.name, muzzlePoint.position, rotation);
        }
        else
        {
            Instantiate(projectilePrefab, muzzlePoint.position, rotation);
        }
    }
}