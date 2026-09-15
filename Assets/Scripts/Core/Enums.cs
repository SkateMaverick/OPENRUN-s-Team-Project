// enum들을 모아두는 네임스페이스
namespace Enums
{
    /// <summary> 캐릭터가 어느 캐릭터인지 구분. (0이 SphereGolem, 1이 BoxGolem) </summary>
    public enum CharacterType
    {
        SphereGolem,
        BoxGolem
    }

    /// <summary> 인게임 그래픽 품질 단계 </summary>
    public enum GraphicQualityLevel
    {
        Low = 0,
        Middle = 1,
        High = 2
    }

    /// <summary> 커서 타입 </summary>
    public enum GameCursorType
    {
        Default,
        Hover,
        Custom
    }
}
