using UnityEngine;

/// <summary>
/// 플레이어가 컨트롤할 수 있는 게임오브젝트임을 나타내는 인터페이스
/// </summary>
public interface IControllable
{
    void HandleCharacterControl(Vector2 moveInput, bool isSprint);
}
