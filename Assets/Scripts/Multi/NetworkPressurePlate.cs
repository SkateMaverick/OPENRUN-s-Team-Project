using UnityEngine;
using Photon.Pun;

/// <summary>
/// 발판 스크립트. Color로 직접 발판의 색을 바꾸기 때문에 기본으로 제공하는 큐브로 만든 발판임을 가정하고 사용.
/// </summary>
public class NetworkPressurePlate : MonoBehaviourPun
{
    public NetworkPressPuzzle networkPressPuzzle;
    
    public bool isPressed; // 눌린 상태인지 아닌지 구분
    public bool autoReset; // True면 안누르고 있으면 초기화되는 발판, False면 한 번만 눌러도 되는 발판

    [System.Serializable]
    public struct PlateColor
    {
        public Color defaultColor; // 눌리지 않은 상태의 색상
        public Color pressedColor; // 눌린 상태의 색상
    }
    public PlateColor plateColor;

    private Renderer _renderer;
    private Color _currentColor;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _currentColor = GetComponent<Renderer>().material.color;
    }

    private void OnTriggerEnter(Collider other) // 발판 위에 누가 올라온다면
    {
        // 눌리게 한 대상에게 PhotonView를 가져와보고, 없거나 리모트 오브젝트라면 처리하지 않음
        //if (!other.TryGetComponent<PhotonView>(out PhotonView targetPhotonView) || !targetPhotonView.IsMine) return;
        
        if (!isPressed) photonView.RPC("OnPress", RpcTarget.All); // 발판이 눌린 상태가 아니라면 OnPress() 호출
    }

    private void OnTriggerExit(Collider other) // 발판 위에 올라왔던게 나가면
    {
        //if (!other.TryGetComponent<PhotonView>(out PhotonView targetPhotonView) || !targetPhotonView.IsMine) return;
        
        if (isPressed && autoReset) photonView.RPC("OnRelease", RpcTarget.All); // 발판이 눌린 상태며, autoReset이 True면 OnRelease() 호출 
    }

    [PunRPC]
    private void OnPress() // 눌렸을 때 실행할 내용
    {
        if (isPressed) return; // 눌려있다면 실행하지 않음
        
        isPressed = true; // 이 발판을 눌린 상태로 변경
        _renderer.material.color = plateColor.pressedColor; // 이 발판의 색상을 눌렸을 때의 색상으로 변경

        networkPressPuzzle.ChangeCount(isPressed); // 앞처리들 이후 발판이 눌렸는지 아닌지에 대한 상태를 넘김 
    }

    [PunRPC]
    private void OnRelease() // 눌린게 초기화되었을 때 실행할 내용
    {
        if (!isPressed) return; // 눌려있지 않다면 실행하지 않음
        
        isPressed = false; // 이 발판을 눌린 상태로 변경
        _renderer.material.color = plateColor.defaultColor; // 이 발판의 색상을 눌렸을 때의 색상으로 변경
        
        networkPressPuzzle.ChangeCount(isPressed);
    }
}
