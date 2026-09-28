using System.Collections;
using UnityEngine;

// 추후 확인 필요
public class Dungeon1Door : MonoBehaviour
{
    [SerializeField] private Transform leftWall;
    [SerializeField] private Transform rightWall;

    [SerializeField] private float moveDistance; // 각 벽이 움직일 거리
    [SerializeField] private float duration = 2f; // 열리는 데 걸리는 시간

    public void TriggerOpenGate()
    {
        StartCoroutine(OpenWallsRoutine());
    }

    private IEnumerator OpenWallsRoutine()
    {
        // 시작 위치
        Vector3 leftStartPos = leftWall.position;
        Vector3 rightStartPos = rightWall.position;

        // 목표 위치
        Vector3 leftTargetPos = leftStartPos + (-leftWall.right * moveDistance);
        Vector3 rightTargetPos = rightStartPos + (rightWall.right * moveDistance);

        float elapsed = 0f; // 문이 열리고 있는 시간

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curve = Mathf.SmoothStep(0, 1, t);

            leftWall.position = Vector3.Lerp(leftStartPos, leftTargetPos, curve);
            rightWall.position = Vector3.Lerp(rightStartPos, rightTargetPos, curve);

            yield return null; 
        }

        leftWall.position = leftTargetPos;
        rightWall.position = rightTargetPos;
    }
}
