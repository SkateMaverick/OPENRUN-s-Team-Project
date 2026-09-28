using UnityEngine;

// 밀려질 수 있는 프롭
public class PushableProp : MonoBehaviour, IPushable
{
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // 외부에서 호출
    public void Push(Vector3 direction, float power)
    {
        // direction * power만큼 밀어짐
        _rigidbody.AddForce(direction * power, ForceMode.Force);
        print(direction*power);
        print("힘이 가해짐");
    }
}
