using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//
public class NetworkPressPuzzle : MonoBehaviourPun
{
    public Dungeon1Door dungeon1Door;
    
    public List<NetworkPressurePlate> pressurePlates;
    public GameObject treasure;
    public Transform treasurePoint;
    
    [HideInInspector] public int pressedPlateCount;

    private void Awake()
    {
        pressedPlateCount = 0;
    }
    
    // NetworkPressurePlate에서 호출
    // pressurePlateCount에 isPressed가 True면 +1; False면 -1 
    public void ChangeCount(bool isPressed)
    {
        if (!PhotonNetwork.IsMasterClient) return; // 마스터 클라이언트가 아니면 실행하지 않음

        if (isPressed) pressedPlateCount++; // 발판이 눌려졌기에 호출한 거라면
        else pressedPlateCount--; // 발판이 초기화되었기에 호출한 거라면

        if (pressedPlateCount >= pressurePlates.Count) GiveReward();
    }

    private void GiveReward()
    {
        dungeon1Door.TriggerOpenGate();
    }


    // private void Update()
    // {
    //     if (!PhotonNetwork.IsMasterClient) return;
    //     
    //     int pressurePlateCount = 0;
    //     
    //     foreach (var pressurePlate in pressurePlates)
    //     {
    //         if (pressurePlate.isPressed) pressurePlateCount++;
    //     }
    //     
    //     if (pressurePlateCount >= pressurePlates.Count) photonView.RPC("GetTreasure", RpcTarget.All);//GetTreasure();
    // }

    public void OnPlateStateChanged(bool isPressed)
    {
        if (!PhotonNetwork.IsMasterClient) return; // 마스터 클라이언트가 아니면 빠져나감
        
    }

    [PunRPC]
    private void GetTreasure()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(treasure.name, treasurePoint.position, treasurePoint.rotation);
            this.enabled = false;
        }
    }
}
