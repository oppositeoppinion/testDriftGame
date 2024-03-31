using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCosmetics : MonoBehaviour
{
    public Color32 CarColor; 
    public EquippedState Wheels; 
    public EquippedState Spoiler;
    public void SyncronizeCosmetics()
    {

        PhotonView photonView = PhotonView.Get(this);

        string colorToSend;
        if (CarColor == Color.white) colorToSend = "white";
        else if (CarColor == Color.red) colorToSend = "red";
        else if (CarColor == Color.black) colorToSend = "black";
        else colorToSend = "blue";

        photonView.RPC("PUNSynchronize", RpcTarget.Others,colorToSend, Wheels, Spoiler);
    }

    [PunRPC]
    private void PUNSynchronize(string color, EquippedState wheels, EquippedState spoiler, PhotonMessageInfo info)
    {
        if (info.photonView.name == "AnotherPlayerCar(processed)") return;
        Debug.Log(info.photonView.name);

        var anotherPlayerCar = GameObject.Find(info.photonView.name);
        Color carColor;
        switch (color)
        {
            case "white": carColor = Color.white; break;
            case "red": carColor = Color.red; break;
            case "black": carColor = Color.black; break;

            default: carColor = Color.black; break;
        }

        var mRenderer = anotherPlayerCar.transform.Find("Body").gameObject.GetComponent<MeshRenderer>();
        mRenderer.material.color = carColor;

        anotherPlayerCar.name = $"AnotherPlayerCar(processed)";
    }
    
}
