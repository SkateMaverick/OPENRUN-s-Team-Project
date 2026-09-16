using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour
{
    public Transform target;
    public float height = 30f;
    public Vector3 offset = Vector3.zero;
    public bool autoFindTarget = true;
    public bool smoothFollow = false;
    public float smoothSpeed = 10f;

    private void Start()
    {
        if (target == null && autoFindTarget)
        {
            FindTarget();
        }

        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnCharacterChanged += SetTarget;
            if (target == null)
            {
                SetTarget(PlayerController.Instance.CurrentCharacterTransform);
            }
        }
    }

    private void OnDestroy()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnCharacterChanged -= SetTarget;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void LateUpdate()
    {
        if (target == null && autoFindTarget)
        {
            FindTarget();
            if (target == null) return;
        }

        if (target == null) return;

        Vector3 targetPos = target.position + offset;
        targetPos.y = target.position.y + height;

        if (smoothFollow)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = targetPos;
        }

        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    private void FindTarget()
    {
        if (PlayerController.Instance != null && PlayerController.Instance.CurrentCharacterTransform != null)
        {
            target = PlayerController.Instance.CurrentCharacterTransform;
            return;
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }
}
