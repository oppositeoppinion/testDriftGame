using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlineManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private GameObject Car;
    private OnlineTimer _onlintTimer;
    
    private void Start()
    {
        StartCoroutine(ConnectDelay());
        _onlintTimer = GetComponent<OnlineTimer>();
    }

    public override void OnLeftRoom()  //current player lefts room
    {
        SceneManager.LoadScene("Lobby");
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.LogFormat($"Player entered room, nickname = {newPlayer.NickName}");
        StartCoroutine(CosmeticSyncForOldPlayers());
    }
    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.LogFormat($"Player left room, nickname = {newPlayer.NickName}");
    }
    public void Leave()
    {
        PhotonNetwork.LeaveRoom();
    }
    private IEnumerator CosmeticSyncForOldPlayers()
    {
        yield return new WaitForSeconds(0.7f);
        var cosmeticsScript = Car.GetComponent<CarCosmetics>();
        cosmeticsScript.SyncronizeCosmetics();

    }

    private void OnPlayerConnected()
    {
        Debug.Log("playerconnected event");
        
    }
    
    private IEnumerator ConnectDelay()
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 newPosition = new Vector3(Random.Range(-10f, 10f), 1f, Random.Range(-2f, 2f));
        Car = PhotonNetwork.Instantiate("PlayerCar", newPosition, Quaternion.identity);
        Car.name = "PlayerCar";
        GameObject.Find("SavingSystem").GetComponent<SavingSystem>().OnlineInitialize();
        InitializeCarCosmetics(Car);
        Car.GetComponent<CarCosmetics>().SyncronizeCosmetics();
        SetCamera();
        //_onlintTimer.StartPreTimer();
    }

    private void SetCamera()
    {
        var camera = Camera.main;
        camera.transform.parent = Car.transform.Find("CameraHolder");
        camera.transform.localPosition = new Vector3(0f, 2.5f, -7f);
        camera.transform.localRotation = Quaternion.Euler(9f, 0f, 0f);
    }

    private void InitializeCarCosmetics(GameObject car)
    {
        var carCosmetics = car.GetComponent<CarCosmetics>();
        var savSystem = GameObject.Find("SavingSystem");
        var saveData = savSystem.GetComponent<SavingSystem>().LoadSavingData();
        carCosmetics.Wheels = saveData.Wheels;
        carCosmetics.Spoiler = saveData.Spoiler;
        carCosmetics.CarColor = saveData.CarColor;
        savSystem.name = "SavingSystemProcessed";
    }
}
