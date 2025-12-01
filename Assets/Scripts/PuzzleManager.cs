using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    [System.Serializable]
    public class PuzzleGroup
    {
        public string groupID;
        public UnityEvent onPuzzleSolved;
        public UnityEvent onPuzzleReset;
        
        [HideInInspector] public bool isSolved = false;
        [HideInInspector] public List<PressurePlateScript> connectedPlates = new List<PressurePlateScript>();
    }

    [Header("퍼즐 그룹 설정")]
    public List<PuzzleGroup> puzzles = new List<PuzzleGroup>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    public void RegisterPlate(PressurePlateScript plate)
    {
        var group = puzzles.FirstOrDefault(p => p.groupID == plate.groupID);
        
        if (group != null)
        {
            group.connectedPlates.Add(plate);
        }
        else
        {
            Debug.LogWarning($"ID가 '{plate.groupID}'인 퍼즐 그룹 설정이 매니저에 없습니다.");
        }
    }
    
    public void CheckPuzzleState(string groupID)
    {
        var group = puzzles.FirstOrDefault(p => p.groupID == groupID);
        if (group == null) return;
        
        bool allPressed = group.connectedPlates.All(plate => plate.isPressed);

        if (allPressed && !group.isSolved)
        {
            group.isSolved = true;
            Debug.Log($"퍼즐 성공: {groupID}");
            group.onPuzzleSolved.Invoke();
        }
        else if (!allPressed && group.isSolved)
        {
            group.isSolved = false;
            Debug.Log($"퍼즐 해제: {groupID}");
            group.onPuzzleReset.Invoke();
        }
    }
}