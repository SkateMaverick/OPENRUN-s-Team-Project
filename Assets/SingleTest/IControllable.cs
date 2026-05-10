using UnityEngine;

public interface IControllable
{
    void HandleCharacterControl(Vector2 moveInput, bool isSprint);
}
