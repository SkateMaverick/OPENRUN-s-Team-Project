using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairAppearingPuzzle : MonoBehaviour
{
    [Header("Stair Target")]
    [Tooltip("The staircase GameObject that will appear or rise")]
    [SerializeField] private GameObject staircaseObject;

    [Tooltip("Should the stairs rise up from under the floor, or activate immediately?")]
    [SerializeField] private bool riseFromFloor = true;
    [SerializeField] private float riseDuration = 2.0f;
    [SerializeField] private float submergedOffsetY = 5.5f;

    [Header("Pressure Plates")]
    [SerializeField] private List<PressurePlate> pressurePlates = new List<PressurePlate>();

    [Header("Puzzle State")]
    [SerializeField] private bool permanentUnlock = true; // Once both are stepped on, stays unlocked

    [Header("Audio Feedback")]
    [SerializeField] private AudioClip solvedSound;
    [SerializeField] private AudioClip risingSound;

    private int _pressedCount = 0;
    private bool _isSolved = false;
    private Vector3 _originalStairPosition;
    private Vector3 _submergedStairPosition;
    private Coroutine _moveCoroutine;

    private void Awake()
    {
        if (staircaseObject != null)
        {
            _originalStairPosition = staircaseObject.transform.position;
            _submergedStairPosition = _originalStairPosition - new Vector3(0f, submergedOffsetY, 0f);

            if (riseFromFloor)
            {
                staircaseObject.transform.position = _submergedStairPosition;
                staircaseObject.SetActive(false);
            }
            else
            {
                staircaseObject.SetActive(false);
            }
        }
    }

    public void OnPlateStateChanged(bool isPressed)
    {
        if (_isSolved && permanentUnlock) return;

        if (isPressed)
        {
            _pressedCount++;
        }
        else
        {
            _pressedCount = Mathf.Max(0, _pressedCount - 1);
        }

        CheckPuzzleState();
    }

    private void CheckPuzzleState()
    {
        int required = pressurePlates.Count > 0 ? pressurePlates.Count : 2;

        if (_pressedCount >= required && !_isSolved)
        {
            _isSolved = true;
            OnPuzzleSolved();
        }
        else if (_pressedCount < required && _isSolved && !permanentUnlock)
        {
            _isSolved = false;
            OnPuzzleReset();
        }
    }

    private void OnPuzzleSolved()
    {
        if (solvedSound != null)
        {
            AudioSource.PlayClipAtPoint(solvedSound, staircaseObject != null ? staircaseObject.transform.position : transform.position);
        }

        if (staircaseObject != null)
        {
            staircaseObject.SetActive(true);

            if (riseFromFloor)
            {
                if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
                _moveCoroutine = StartCoroutine(MoveStairRoutine(_submergedStairPosition, _originalStairPosition));
            }
        }
    }

    private void OnPuzzleReset()
    {
        if (staircaseObject != null)
        {
            if (riseFromFloor)
            {
                if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
                _moveCoroutine = StartCoroutine(MoveStairRoutine(staircaseObject.transform.position, _submergedStairPosition, () =>
                {
                    staircaseObject.SetActive(false);
                }));
            }
            else
            {
                staircaseObject.SetActive(false);
            }
        }
    }

    private IEnumerator MoveStairRoutine(Vector3 fromPos, Vector3 toPos, System.Action onComplete = null)
    {
        float elapsed = 0f;

        while (elapsed < riseDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / riseDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            staircaseObject.transform.position = Vector3.Lerp(fromPos, toPos, smoothT);
            yield return null;
        }

        staircaseObject.transform.position = toPos;
        _moveCoroutine = null;
        onComplete?.Invoke();
    }
}
