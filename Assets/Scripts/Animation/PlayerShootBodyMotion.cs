using UnityEngine;

// 발사할 때 Body가 뒤로 물러났다가 앞으로 튀어 나가는 움직임
// 언제 재생할지와 뜸 들이는 시간은 호출하는 쪽(PlayerShooter)이 정한다.
public class PlayerShootBodyMotion : MonoBehaviour
{
    [SerializeField] private Transform body; // Armature/Root/Body
    [SerializeField] private float pullBackDistance = 0.1f; // 뜸 들이는 동안 뒤로 물러나는 거리
    [SerializeField] private float pushForwardDistance = 0.08f; // 발사 순간 앞으로 나가는 거리
    [SerializeField] private float pushTime = 0.05f; // 뒤 → 앞으로 튀어 나가는 시간
    [SerializeField] private float returnTime = 0.15f; // 앞 → 제자리로 돌아오는 시간

    private float _windupTime;
    private float _startTime = -1f; // 음수면 재생 중이 아님

    // windupTime은 표정 연출과 발사 시점에 맞추려고 PlayerShooter가 정한 값을 받는다
    public void Play(float windupTime)
    {
        _windupTime = windupTime;
        _startTime = Time.time;
    }
    
    private void LateUpdate()
    {
        if (_startTime < 0f) return;

        // Play() 이후 흐른 시간
        float elapsed = Time.time - _startTime;
        if (elapsed >= _windupTime + pushTime + returnTime)
        {
            _startTime = -1f;
            return;
        }

        // Body 뼈는 애니메이션에 따라 기울어지므로, 수평으로 밀도록 루트의 forward를 씀
        body.position += transform.forward * EvaluateOffset(elapsed);
    }
    
    // 경과된 시간에 따라 뒤 또는 앞으로 이동할 때의 오프셋인 스칼라 값을 반환
    private float EvaluateOffset(float elapsed)
    {
        // 발사 전 뜸들이기 (뒤로 물러남)
        if (elapsed < _windupTime)
        {
            return Mathf.Lerp(0f, -pullBackDistance, elapsed / _windupTime);
        }

        // 발사 후 (앞으로 튀어나감)
        elapsed -= _windupTime;
        if (elapsed < pushTime)
        {
            return Mathf.Lerp(-pullBackDistance, pushForwardDistance, elapsed / pushTime);
        }

        // 제자리로 돌아오기
        elapsed -= pushTime;
        return Mathf.Lerp(pushForwardDistance, 0f, elapsed / returnTime);
    }
}
