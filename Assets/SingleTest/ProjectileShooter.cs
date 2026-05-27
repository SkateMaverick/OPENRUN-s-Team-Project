using UnityEngine;

public class ProjectileShooter : Shooter
{
    protected override void Awake()
    {
        base.Awake();
    }
    
    protected override void Shot()
    {
        
    }
    

    public void Shooting(Vector3 direction)
    {
        //_projectileRigidbody.MovePosition(transform.position + direction * shotSpeed * Time.deltaTime);
    }
}
