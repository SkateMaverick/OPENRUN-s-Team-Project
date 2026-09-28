using UnityEngine;
using Photon.Pun;

// 
public class NetworkBulletFire : MonoBehaviourPun
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    
    [Header("Settings")]
    public float visualSpeed = 50f;
    public float range = 100f;
    public float maxAimDistance = 100.0f;
    public float convergenceDistance = 5.0f; 

    private Vector3 _currentTarget;

    void Update()
    {
        if (!photonView.IsMine) return;
        
        UpdateAim();

        if (Input.GetButtonDown("Fire1"))
        {
            Fire();
        }
    }

    void UpdateAim()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        
        Vector3 cameraAimPoint;

        if (Physics.Raycast(ray, out hit, range))
        {
            cameraAimPoint = hit.point;
        }
        else
        {
            cameraAimPoint = ray.origin + ray.direction * maxAimDistance;
        }
        
        float distanceToTarget = Vector3.Distance(ray.origin, cameraAimPoint);
        Vector3 bowForwardPoint = firePoint.position + firePoint.forward * distanceToTarget;

        float t = Mathf.InverseLerp(0f, convergenceDistance, distanceToTarget);
        float blendFactor = Mathf.SmoothStep(0f, 1f, t);
        
        _currentTarget = Vector3.Lerp(bowForwardPoint, cameraAimPoint, blendFactor);
    }

    void Fire()
    {
        // _currentTarget은 이미 UpdateAim에서 계산됨
        SpawnVisualBullet(_currentTarget);
    }

    void SpawnVisualBullet(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - firePoint.position).normalized;
        // 총알이 날아갈 방향을 보게 회전
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        GameObject bulletObj = PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, lookRotation);
        NetworkBullet bulletScript = bulletObj.GetComponent<NetworkBullet>();

        if (bulletScript != null)
        {
            // 목표 지점(먼 곳)을 인자로 넘겨줌 -> Bullet에서 방향 계산
            bulletScript.Setup(firePoint.position + direction * range, visualSpeed);
        }
    }
}