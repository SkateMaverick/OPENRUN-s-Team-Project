using System.Collections.Generic;
using UnityEngine;

public class PressPuzzle : MonoBehaviour
{
    [SerializeField] private Dungeon1Door dungeon1Door;
    [SerializeField] private List<PressurePlate> pressurePlates;
    
    [HideInInspector] public int pressedPlateCount;

    private void Awake()
    {
        #region 초기화

        pressedPlateCount = 0;

        #endregion
    }

    // PressurePlate에서 호출
    // pressurePlateCount에다가 isPressed가 True면 +1, False면 -1
    public void ChangeCount(bool isPressed)
    {
        if (isPressed) pressedPlateCount++; // 발판이 눌려졌기에 호출한 거라면
        else pressedPlateCount--; // 발판이 초기화되었기에 호출한 거라면
        
        if (pressedPlateCount >= pressurePlates.Count) GiveReward(); 
    }

    private void GiveReward()
    {
        dungeon1Door.TriggerOpenGate();
    }
}
