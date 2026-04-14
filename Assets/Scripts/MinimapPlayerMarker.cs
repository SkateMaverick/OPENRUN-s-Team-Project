using UnityEngine;

public class MinimapPlayerMarker : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        if (target == null) return;

        transform.rotation = Quaternion.Euler(0f, 0f, -target.eulerAngles.y);
    }
}