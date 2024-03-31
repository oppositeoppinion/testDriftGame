using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavingData 
{
    public int PremiumCoins = 999;
    public Color32 CarColor = Color.white;
    public bool IsShopWhellsBuyed = false;
    public bool IsShopSpoilerBuyed = false;
    public EquippedState Wheels = EquippedState.Usuall;
    public EquippedState Spoiler = EquippedState.Usuall;
    public override string ToString()
    {
        return PremiumCoins.ToString() + CarColor.ToString() + IsShopWhellsBuyed.ToString() + IsShopSpoilerBuyed.ToString() + Wheels.ToString() + Spoiler.ToString();
    }
    
}
public enum EquippedState
{
    Premium,
    Usuall
}
