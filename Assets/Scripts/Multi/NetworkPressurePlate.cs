using UnityEngine;
using Photon.Pun;

public class NetworkPressurePlate : MonoBehaviourPun, IPressable
{
    public bool IsPressed { get; set; }

    private void Awake()
    {
        IsPressed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsPressed == false) OnPress();
    }

    public void OnPress()
    {
        IsPressed = true;
        photonView.RPC("ChangeColor", RpcTarget.All);
    }
    
    [PunRPC]
    private void ChangeColor()
    {
        GetComponent<Renderer>().material.color = Color.red;
    }
}
