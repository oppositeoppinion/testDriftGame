using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageManager : MonoBehaviour
{
    public void StartDriftLevel()
    {
        PhotonNetwork.LoadLevel("Lobby");
    }

}
