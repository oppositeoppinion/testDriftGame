using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageShop : MonoBehaviour
{
    
    public bool IsSpoierBuyed;
    public bool IsWheelsBuyed;
    public EquippedState Spoiler;
    public EquippedState Wheels;
    private GameObject _spoiler;
    private GameObject _wheels;
    private void Start()
    {
        GameObject playerCar = GameObject.Find("PlayerCar");
        _spoiler = playerCar.transform.Find("Spoiler").gameObject;
        _wheels = playerCar.transform.Find("Wheels").gameObject;
    }
    public void SetBuyedParts()
    {
        if (IsSpoierBuyed) _spoiler.SetActive(true);
        else _spoiler.SetActive(false);
    }
    public void ButtonSpoiler()
    {
        if (!IsSpoierBuyed) OpenShop();
        else
        {
            if (_spoiler.activeSelf)
            {
                _spoiler.SetActive(false);
                Spoiler = EquippedState.Usuall;
            }
            else
            {
                _spoiler.SetActive(true);
                Spoiler = EquippedState.Premium;
            }
        }
        GameObject.Find("SavingSystem").GetComponent<SavingSystem>().Save();
        //SavingSystem.Instance.Save();
    }
    public void ButtonWheels()
    {
        if (!IsWheelsBuyed) OpenShop();
        else
        {
            if (_wheels.transform.localScale.x == 1f)
            {
                _wheels.transform.localScale = new Vector3(1.12f, 1f, 1f);
                Wheels = EquippedState.Premium;
            }
            else
            {
                _wheels.transform.localScale = Vector3.one;
                Wheels = EquippedState.Usuall;
            }
        }
        GameObject.Find("SavingSystem").GetComponent<SavingSystem>().Save();
    }

    private void OpenShop()
    {
        Debug.Log("open the shop");
    }
}
