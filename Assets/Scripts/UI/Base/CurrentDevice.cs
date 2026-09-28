using UnityEngine;

// 현재 디바이스가 PC인지 모바일인지 구분하기 위한 정적 클래스
public class CurrentDevice
{
    // 리모트로 모바일 확인할 때 Application.isMobilePlatform을 못써가지고 일단 지금은 임의로 모바일 상태인지 정해줘야 합니다..
    private static bool _isMobile = false;
    
    public static bool IsMobile
    {
        get
        {
            if (Application.isMobilePlatform) _isMobile = true;
            return _isMobile;
        }
        set
        {
            _isMobile = value;
        }
    }
}