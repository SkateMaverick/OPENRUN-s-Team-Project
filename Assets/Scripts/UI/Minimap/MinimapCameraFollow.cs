using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour
{
    public Transform target;
    public float height = 30f;
    public Vector3 offset = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 pos = target.position + offset;
        pos.y = height;

        transform.position = pos;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}