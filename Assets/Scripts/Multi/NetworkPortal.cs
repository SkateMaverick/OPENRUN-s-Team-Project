using UnityEngine;
using Photon.Pun;

public class NetworkPortal : MonoBehaviourPun
{
    [SerializeField] private string sceneName;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(sceneName);
            }
        }
    }
}
