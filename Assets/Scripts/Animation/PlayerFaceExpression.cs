using System;
using System.Collections;
using UnityEngine;

// 표정 파츠(자식 오브젝트)를 켜고 꺼서 표정을 바꿈
// 언제 바꿀지는 호출하는 쪽(PlayerShooter)이 정함
public class PlayerFaceExpression : MonoBehaviour
{
    // 그룹마다 동시에 하나만 켜지는 상태들
    public enum Eyebrow { Neutral, Angry }
    public enum Eye { Neutral, Squint }
    public enum Mouth { Neutral, Angry, Open }

    // 눈썹·눈은 L/R 두 오브젝트가 한 상태라서 배열로 받음
    [Header("눈썹")]
    [SerializeField] private GameObject[] eyebrowNeutral;
    [SerializeField] private GameObject[] eyebrowAngry;

    [Header("눈")]
    [SerializeField] private GameObject[] eyeNeutral;
    [SerializeField] private GameObject[] eyeSquint;

    [Header("입")]
    [SerializeField] private GameObject mouthNeutral;
    [SerializeField] private GameObject mouthAngry;
    [SerializeField] private GameObject mouthOpen;

    [Header("발사 연출 타이밍")]
    [SerializeField] private float openMouthTime = 0.15f; // 2단계 유지 시간
    [SerializeField] private float resetDelay = 1f; // 이 시간 안에 다시 발사하지 않으면 평상시 표정으로 복귀

    // 재호출 시 이전 연출을 멈추기 위해 보관
    private Coroutine _shootSequence;
    
    // 표정 처리를 진행할지 판별
    public void PlayShootSequence(Action onFire, float windupTime)
    {
        // 추가로, 발사 딜레이로 인해 4단계일 때만 코루틴이 멈춤 
        if (_shootSequence != null)
        {
            StopCoroutine(_shootSequence);
        }

        _shootSequence = StartCoroutine(ShootSequence(onFire, windupTime));
    }
    
    // 표정을 변화시킴
    private IEnumerator ShootSequence(Action onFire, float windupTime)
    {
        // 1단계: 준비 표정
        SetEyebrow(Eyebrow.Angry);
        SetMouth(Mouth.Angry);
        yield return new WaitForSeconds(windupTime);

        // 2단계: 발사 순간 표정
        SetEye(Eye.Squint);
        SetMouth(Mouth.Open);
        onFire?.Invoke();
        yield return new WaitForSeconds(openMouthTime);
 
        // 3단계: 발사 후 (전투 대기)
        SetEye(Eye.Neutral);
        SetMouth(Mouth.Angry);
        yield return new WaitForSeconds(resetDelay);

        // 4단계: 기본 표정 복귀 (전투 해제)
        SetEyebrow(Eyebrow.Neutral);
        SetMouth(Mouth.Neutral);
        _shootSequence = null;
    }

    // Set... 메서드: 해당 그룹에서 지정한 상태의 파츠만 켜고 나머지 상태의 파츠는 끈다.
    private void SetEyebrow(Eyebrow eyebrow)
    {
        SetPartsActive(eyebrowNeutral, eyebrow == Eyebrow.Neutral);
        SetPartsActive(eyebrowAngry, eyebrow == Eyebrow.Angry);
    }

    private void SetEye(Eye eye)
    {
        SetPartsActive(eyeNeutral, eye == Eye.Neutral);
        SetPartsActive(eyeSquint, eye == Eye.Squint);
    }

    private void SetMouth(Mouth mouth)
    {
        SetPartActive(mouthNeutral, mouth == Mouth.Neutral);
        SetPartActive(mouthAngry, mouth == Mouth.Angry);
        SetPartActive(mouthOpen, mouth == Mouth.Open);
    }

    private void SetNeutral()
    {
        SetEyebrow(Eyebrow.Neutral);
        SetEye(Eye.Neutral);
        SetMouth(Mouth.Neutral);
    }
    
    // SetPartActive() 메서드를 배열인 표정 부위들도 호출 가능하도록 변환
    private void SetPartsActive(GameObject[] parts, bool active)
    {
        foreach (var part in parts)
        {
            SetPartActive(part, active);
        }
    }

    // 표정 게임오브젝트의 파츠를 활성화/비활성화 함 
    private void SetPartActive(GameObject part, bool active)
    {
        if (part != null && part.activeSelf != active)
        {
            part.SetActive(active);
        }
    }
}
