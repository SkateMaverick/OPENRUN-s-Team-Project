using UnityEngine;

// 밀어질 수 있는 요소들이 상속받는 인터페이스
public interface IPushable
{
    void Push(Vector3 direction, float power);
}
