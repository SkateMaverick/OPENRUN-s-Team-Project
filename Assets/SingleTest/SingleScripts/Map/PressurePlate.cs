using System;
using UnityEngine;

/// <summary>
/// 발판 스크립트. Color로 직접 발판의 색을 바꾸기 때문에 기본으로 제공하는 큐브로 만든 발판임을 가정하고 사용.
/// </summary>
public class PressurePlate : MonoBehaviour
{
    [SerializeField] private PressPuzzle pressPuzzle;
    [SerializeField] private bool isPressed; // 눌린 상태인지 아닌지 구분
    [SerializeField] private bool autoReset; // True: 누르지 있지 않으면 초기화되면 발판 False: 한 번만 눌러도 되는 발판
    
    [System.Serializable]
    private struct PlateColor
    {
        public Color defaultColor; // 눌리지 않은 상태의 색상
        public Color pressedColor; // 눌린 상태의 색상
    }
    [SerializeField] private PlateColor plateColor;
    
    private Renderer _renderer;

    private void Awake()
    {
        #region 초기화

        _renderer = GetComponent<Renderer>();

        #endregion
    }
    
    private void OnTriggerEnter(Collider other) // 발판 위에 누가 올라온다면
    {
        if (!isPressed) OnPress(); // 발판이 눌린 상태가 아니라면
    }
    
    private void OnTriggerExit(Collider other) // 발판 위에 올라왔던게 나가면
    {
        if (isPressed && autoReset) OnRelease(); // 발판이 눌린 상태며, autoReset이 True면
    }
    
    private void OnPress() // 눌렀을 때 실행할 내용
    {
        if (isPressed) return; // 눌려있다면 실행하지 않음
        
        isPressed = true; // 이 발판을 눌린 상태로 변경
        _renderer.material.color = plateColor.pressedColor;
        
        pressPuzzle.ChangeCount(isPressed);
    }
    
    private void OnRelease() // 눌린게 초기화되었을 때 실행할 내용
    {
        if (!isPressed) return; // 눌려있지 않다면 실행하지 않음

        isPressed = false; // 이 발판을 눌리지 않은 상태로 변경
        _renderer.material.color = plateColor.defaultColor; // 이 발판의 색상을 눌렀을 때의 색상으로 변경
        
        pressPuzzle.ChangeCount(isPressed);
    }
}
