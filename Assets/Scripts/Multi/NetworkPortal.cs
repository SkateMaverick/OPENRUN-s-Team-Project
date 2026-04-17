using UnityEngine;
using Photon.Pun;

public class NetworkPortal : MonoBehaviourPun
{
    private void OnTriggerEnter(Collider other)
    {
        print("포탈이동");
        if (other.gameObject.CompareTag("Player"))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("Dungeon 1");
            }
        }
    }
}
