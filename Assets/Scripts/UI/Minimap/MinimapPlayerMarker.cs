using UnityEngine;

public class MinimapPlayerMarker : MonoBehaviour
{
    public Transform target;
    public bool autoFindTarget = true;

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

    void Update()
    {
        if (target == null && autoFindTarget)
        {
            FindTarget();
            if (target == null) return;
        }

        if (target == null) return;

        transform.rotation = Quaternion.Euler(0f, 0f, -target.eulerAngles.y);
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
