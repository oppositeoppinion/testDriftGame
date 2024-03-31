using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnlineTimer : MonoBehaviour
{
    [SerializeField] private int _preTimereTime;
    [SerializeField] private int _mainTimereTime;
    private float _preTimerHolder;
    private float _mainTimerHolder;
    private TextMeshProUGUI _timerText;
    private int _preTime;
    private PhotonView _photonView;
    private GameObject _finalNote;
    private TextMeshProUGUI _finalText;
    private int _cashToAdd;



    private void Start()
    {
        StartCoroutine(LoadingAfterStart());
        //Debug.Log(PhotonNetwork.IsMasterClient);
        _finalNote = GameObject.Find("FinishNote");
        _finalText = _finalNote.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        _cashToAdd = 0;
        _finalNote.SetActive(false);
    }
    IEnumerator LoadingAfterStart()
    {
        yield return new WaitForSeconds(1f);
        _photonView = PhotonView.Get(this);
        _photonView.RPC("StartPreTimer", RpcTarget.All);
    }

    [PunRPC]
        public void StartPreTimer()
    {
        StopAllCoroutines();
        _timerText = GameObject.Find("TimerText").GetComponent<TextMeshProUGUI>();
        _timerText.text = $"Drift starts in {_preTimereTime} seconds";
        _preTimerHolder = _preTimereTime;
        StartCoroutine(TimerBeforeDrift());
    }
    public void SaveCashAfterDrift()
    {
        CashSavingProcedures();
        _cashToAdd = 0;
        _finalNote.SetActive(false);
    }
    public void SaveDoubleCashAfterDrift()
    {
        _cashToAdd = _cashToAdd * 2;
        CashSavingProcedures();
        _cashToAdd = 0;
        _finalNote.SetActive(false);
    }

    private IEnumerator TimerBeforeDrift()
    {
        if (_preTimerHolder == 0)
        {
            yield return null;
            //StopCoroutine(TimerBeforeDrift());
            StopAllCoroutines();
            _timerText.text = $"";
            //_photonView = PhotonView.Get(this);
            if (PhotonNetwork.IsMasterClient) _photonView.RPC("StartMainTimer", RpcTarget.All);
        }
        else
        {
            yield return new WaitForSeconds(1);
            _preTimerHolder--;
            _timerText.text = $"Drift starts in {_preTimerHolder} seconds";
            StopCoroutine(TimerBeforeDrift());
            StartCoroutine(TimerBeforeDrift());
        }
    }
    [PunRPC]
    private void StartMainTimer()
    {
        StopAllCoroutines();
        //Camera.main.backgroundColor = UnityEngine.Color.white; //dbg
        _mainTimerHolder = _mainTimereTime;
        
        StartCoroutine(TimerToGetPoints());
    }

    private IEnumerator TimerToGetPoints()
    {
        if (_mainTimerHolder == 0)
        {
            yield return null;
            if (PhotonNetwork.IsMasterClient) _photonView.RPC("FinishDrift", RpcTarget.All);
        }
        else
        {
            yield return new WaitForSeconds(1f);
            _mainTimerHolder--;
            _timerText.text = $"{_mainTimerHolder} seconds left";
            StartCoroutine(TimerToGetPoints());
        }
    }
    [PunRPC]
    private void FinishDrift()
    {
        //Camera.main.backgroundColor = UnityEngine.Color.yellow; //dbg
        var points = GameObject.Find("PlayerCar").GetComponent<PointsManager>().FinishDrift();
        _timerText.text = $"Drift is finished. You have {points} points";

        CashForDriftPoints(points);
    }

    private void CashForDriftPoints(int points)
    {
        _finalNote.SetActive(true);
        _finalText.text = $"Drift is finished, you got {points} points";
        _cashToAdd = points;
    }
    
    private void CashSavingProcedures()
    {
        var savingSystem = GameObject.Find("SavingSystemProcessed").GetComponent<SavingSystem>(); //replace t ocash buttons
        savingSystem.Save(_cashToAdd);
        var cashText = GameObject.Find("PremiumCoinsField").GetComponent<TextMeshProUGUI>();
        cashText.text = $"{cashText.text} $ + <color=red>{_cashToAdd} $</color>";
        _cashToAdd = 0;
    }
}
