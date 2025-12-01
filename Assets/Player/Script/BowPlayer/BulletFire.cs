using UnityEngine;

public class BulletFire : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float visualSpeed = 50f;
    public float range = 100f;
    public float maxAimDistance = 100.0f;
    public float convergenceDistance = 5.0f; 

    private Vector3 _currentTarget;

    void Update()
    {
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

        DrawDebugRay(firePoint.position, _currentTarget);
    }

    void Fire()
    {
        SpawnVisualBullet(_currentTarget);
    }

    void SpawnVisualBullet(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - firePoint.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, lookRotation);
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            bulletScript.Setup(firePoint.position + direction * range, visualSpeed);
        }
    }

    void DrawDebugRay(Vector3 startPos, Vector3 endPos)
    {
        Debug.DrawLine(startPos, endPos, Color.yellow);
    }
}