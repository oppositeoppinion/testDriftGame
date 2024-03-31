using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageCosmetics : MonoBehaviour
{
    private MeshRenderer _mRenderer;
    private void Start()
    {
        _mRenderer = GameObject.Find("PlayerCar").transform.Find("Body").GetComponent<MeshRenderer>();
    }
    public void ColorYellow()
    {
        _mRenderer.material.color = Color.white;
        SaveCosmetics();
    }
    public void ColorBlack()
    {
        _mRenderer.material.color = Color.black;
        SaveCosmetics();
    }
    public void ColorRed()
    {
        _mRenderer.material.color = Color.red;
        SaveCosmetics();
    }
    private void SaveCosmetics()
    {
        GameObject.Find("SavingSystem").GetComponent<SavingSystem>().Save();
    }
}

