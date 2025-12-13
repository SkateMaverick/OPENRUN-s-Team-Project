using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

// 조이스틱
public class MobileJoystick : OnScreenControl, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform handle; // 조이스틱 이미지
    public RectTransform background; // 조이스택 배경 이미지
    public float handleRange = 150f; // 조이스틱이 움직일 수 있는 최대 거리

    private Vector2 _defaultPosition; // 조이스틱과 조이스틱 배경의 처음 또는 되돌아갈 기준 위치
    private Vector2 _inputVector; // mControlPath에 보낼 값

    [InputControl(layout = "Vector2")]
    [SerializeField]
    private string mControlPath; // 연결될 Input System 장치

    // OnScreenControl의 abstract 프로퍼티
    protected override string controlPathInternal
    {
        get => mControlPath;
        set => mControlPath = value;
    }

    // OnScreenControl이 MonoBehaviour를 상속받고 있음
    private void Start()
    {
        // 게임 시작 시의 조이스틱 배경 이미지 위치를 저장
        // anchoredPosition: Rect Transform의 Pos X, Pos Y 값
        _defaultPosition = background.anchoredPosition;
    }

    // IPointerDownHandler 메서드
    // 포인터가 오브젝트 위에서 눌렸을 때 호출 (https://docs.unity3d.com/kr/2022.1/Manual/SupportedEvents.html)
    public void OnPointerDown(PointerEventData eventData)
    {
        // RectTransformUtility: 랙트 트랜스폼을 사용하기 위해 도움을 주는 메서드들이 담긴 유틸리티 클래스
        // ScreenPointToLocalPointInRectangle: bool 타입을 리턴 / True를 리턴할 때는 평면(여기서는 하이어라키의 Joystick Area)이 힛 되었을 때고, 이는 포인트가 UI 직사각형 내부이든 관계없음
        // + (테스트할 RectTrnasform, 테스트할 스크린 포인트, 테스트가 수행될 카메라, out Vector2 localPoint)
        // localPoint: 스크린 위를 터치했을 때, 그 위치를 RectTransform 범위 안에서의 좌표 값으로 변환하여 가짐
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        background.anchoredPosition = localPoint;
        handle.anchoredPosition = localPoint;
        //print($"Down의 localPoint: {localPoint}"); // background의 기본 위치를 (0, 0)으로 기준을 잡음
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        //print($"Drag의 localPoint: {localPoint}"); // 터치한 위치를 기준으로 잡음
        Vector2 clampedPosition = localPoint;
        // 드래그한 길이가 handleRange보다 길다면
        if (localPoint.magnitude > handleRange)
        {
            // clampedPosition은 최대치 범위(handleRange)의 값으로 덮어씌워짐
            clampedPosition = clampedPosition.normalized * handleRange;
        }

        // 조이스틱 이미지를 드래그된 위치로 이동시킴
        handle.anchoredPosition = background.anchoredPosition + clampedPosition;
        
        // clampedPosition을 handleRange로 스케일링
        _inputVector = clampedPosition / handleRange;
        // _inputVector의 값을 controlPathInternal에 설정한 control path(Left Stick/Gamepad)에 보냄
        SendValueToControl(_inputVector);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 이동을 멈춤
        _inputVector = Vector2.zero;
        SendValueToControl(_inputVector);

        // 조이스틱 이미지와 조이스틱을 제자리로 이동
        background.anchoredPosition = _defaultPosition;
        handle.anchoredPosition = _defaultPosition;
    }
}
