using UnityEngine;
using System.Collections;

public class PressurePlateScript : MonoBehaviour
{
    [Header("Settings")]
    public string groupID;
    public float resetDelay = 3.0f;
    public LayerMask targetLayers; 

    [Header("Detection Area")]
    public Vector3 detectionSize = new Vector3(1f, 1f, 1f);
    public Vector3 detectionOffset = new Vector3(0f, 0.5f, 0f);

    [Header("Debug")]
    public bool isPressed = false; 

    private Coroutine _resetCoroutine;

    private void Start()
    {
        if (targetLayers == 0)
        {
            targetLayers = -1; 
        }

        if (PuzzleManager.Instance != null)
            PuzzleManager.Instance.RegisterPlate(this);
    }

    private void Update()
    {
        DetectObjects();
    }

    private void DetectObjects()
    {
        Collider[] hits = Physics.OverlapBox(
            transform.position + detectionOffset, 
            detectionSize * 0.5f, 
            transform.rotation, 
            targetLayers, 
            QueryTriggerInteraction.Ignore
        );

        bool currentFramePressed = hits.Length > 0;

        if (currentFramePressed)
        {
            // 밟고 있는 중
            if (_resetCoroutine != null)
            {
                StopCoroutine(_resetCoroutine);
                _resetCoroutine = null;
            }

            if (!isPressed)
            {
                SetState(true);
            }
        }
        else
        {
            if (isPressed && _resetCoroutine == null)
            {
                _resetCoroutine = StartCoroutine(ResetAfterDelay(resetDelay));
            }
        }
    }

    private IEnumerator ResetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetState(false);
        _resetCoroutine = null;
    }

    private void SetState(bool pressed)
    {
        isPressed = pressed;
        
        // 매니저에게 알림
        if (PuzzleManager.Instance != null)
            PuzzleManager.Instance.CheckPuzzleState(groupID);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = isPressed ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);
        
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position + detectionOffset, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;
        
        Gizmos.DrawCube(Vector3.zero, detectionSize);
        Gizmos.DrawWireCube(Vector3.zero, detectionSize);
    }
}