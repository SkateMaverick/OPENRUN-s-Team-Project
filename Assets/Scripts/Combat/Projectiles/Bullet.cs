using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 _targetPosition;
    private float _speed;
    private bool _isFired = false;


    public void Setup(Vector3 target, float bulletSpeed)
    {
        this._targetPosition = target;
        this._speed = bulletSpeed;
        _isFired = true;
        Destroy(gameObject, 3.0f);
    }

    void Update()
    {
        if (!_isFired) return;
        
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _speed * Time.deltaTime);
        
        if (Vector3.Distance(transform.position, _targetPosition) < 0.1f)
        {
            Destroy(gameObject, 3.0f);
        }
    }
}