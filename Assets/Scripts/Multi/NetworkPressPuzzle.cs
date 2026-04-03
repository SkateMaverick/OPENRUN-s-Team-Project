using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkPressPuzzle : MonoBehaviourPun
{
    public List<NetworkPressurePlate> pressurePlates;
    public GameObject treasure;
    public Transform treasurePoint;

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        int pressurePlateCount = 0;
        
        foreach (var pressurePlate in pressurePlates)
        {
            if (pressurePlate.IsPressed) pressurePlateCount++;
        }
        
        if (pressurePlateCount >= pressurePlates.Count) photonView.RPC("GetTreasure", RpcTarget.All);//GetTreasure();
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
