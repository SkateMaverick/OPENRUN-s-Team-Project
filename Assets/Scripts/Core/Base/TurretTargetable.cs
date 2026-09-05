using UnityEngine;
using Photon.Pun;

public class TurretTargetable : MonoBehaviourPun
{
    [SerializeField] private Transform aimPoint;

    public Transform AimPoint => aimPoint != null ? aimPoint : transform;
}