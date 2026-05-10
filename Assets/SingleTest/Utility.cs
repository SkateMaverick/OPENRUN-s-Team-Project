using UnityEngine;

/// <summary>
/// Vector2를 Vector3로 변경하는 등 유틸리티 모음 클래스
/// </summary>
public static class Utility
{
    /// <summary>
    /// Vector2 값을 Vector3 값으로 변경. (Vector2(1, 2) -> Vector3(1, 0, 2)) 
    /// </summary>
    public static Vector3 Vector2ToVector3(Vector2 vector)
    {
        return new Vector3(vector.x, 0f, vector.y);
    }
}
