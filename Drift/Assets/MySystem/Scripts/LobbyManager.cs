using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [field:SerializeField] public TextMeshProUGUI LogText { get; private set; }
    [SerializeField] private Levels _currentLevel;
    private TextMeshProUGUI _levelButtonText;
    private void Start()
    {
        PhotonNetwork.NickName = $"player {Random.Range(0, 100)}";
        AddLog($"Player name set to {PhotonNetwork.NickName}");
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.ConnectUsingSettings();

         _currentLevel = Levels.Level01;
        _levelButtonText = GameObject.Find("ButtonLevel").transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
    }
    public override void OnConnectedToMaster()
    {
        AddLog($"Connected to master server");
    }
    public override void OnJoinedRoom()
    {
        AddLog("joined room");
        PhotonNetwork.LoadLevel("Garage");
    }
    public void AddLog(string text)
    {
        LogText.text += "\n";
        LogText.text += $"          {text}";
    }
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(null);
        var levelToLoad = _currentLevel.ToString();
        PhotonNetwork.LoadLevel(levelToLoad);
    }
    public void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public void LevelChange()
    {
        Debug.Log(_currentLevel);
        switch (_currentLevel)
        {
            case Levels.Level01:
                _currentLevel = Levels.Level02;
                _levelButtonText.text = "Level: 02";
                break;
            case Levels.Level02:
                _currentLevel = Levels.Level01;
                _levelButtonText.text = "Level: 01";
                break;
            default:
                break;
        }
        
    }
    public void Garage()
    {
        SceneManager.LoadScene("Garage");
    }
    public void Exit()
    {
        Application.Quit();
    }
}
enum Levels
{
    Level01,
    Level02
}