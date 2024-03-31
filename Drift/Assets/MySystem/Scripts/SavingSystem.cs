using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavingSystem : MonoBehaviour
{
    
    public int PremiumCoins { get; private set; }

    [SerializeField] private GarageShop _garageShopScript;
    [SerializeField] private GameObject _playerCar;
    [SerializeField] private MeshRenderer _mRenderer;
    [SerializeField] private Scene _currentScene;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += NewSceneCheck;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= NewSceneCheck;
    }

    public void Save(int cashToAdd = 0)
    {
        _playerCar = GameObject.Find("PlayerCar");
        _mRenderer = _playerCar.transform.Find("Body").GetComponent<MeshRenderer>();
        Color32 color = _mRenderer.material.color;
        bool isShopWheelsBuyed;
        bool isShopSpoilerBuyed;
        EquippedState spoiler;
        EquippedState wheels;
        int cash;
        var savData = LoadSavingData();

        var currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "Garage") //if in garage
        {
            isShopWheelsBuyed = _garageShopScript.IsWheelsBuyed;
            isShopSpoilerBuyed = _garageShopScript.IsSpoierBuyed;
            spoiler = _garageShopScript.Spoiler;
            wheels = _garageShopScript.Wheels;
            cash = savData.PremiumCoins;  
        }
        else if (currentSceneName == "Level01"|| currentSceneName == "Level02") 
        {
            isShopWheelsBuyed = savData.IsShopWhellsBuyed;
            isShopSpoilerBuyed = savData.IsShopSpoilerBuyed;
            spoiler = savData.Spoiler;
            wheels = savData.Wheels;
            cash = savData.PremiumCoins+cashToAdd; 
        }
        else
        {
            
            isShopWheelsBuyed = savData.IsShopWhellsBuyed;  
            isShopSpoilerBuyed = savData.IsShopSpoilerBuyed;
            spoiler = savData.Spoiler;
            wheels = savData.Wheels;
            cash = savData.PremiumCoins;
        }
        SavingData newSave = new SavingData { CarColor = color, PremiumCoins = cash, IsShopWhellsBuyed = isShopWheelsBuyed, IsShopSpoilerBuyed = isShopSpoilerBuyed, Spoiler = spoiler, Wheels = wheels };
        var jsonString = SerializeToJson(newSave);
        File.WriteAllText(Application.dataPath + "/save.txt", jsonString);
    }
    [ContextMenu("load")]
    public void Load()
    {
        SavingData savingData = LoadSavingData();
        //Debug.Log(savingData.ToString());
        _playerCar = GameObject.Find("PlayerCar");
        _mRenderer = _playerCar.transform.Find("Body").GetComponent<MeshRenderer>();
        PremiumCoins = savingData.PremiumCoins;
        _mRenderer.material.color = savingData.CarColor;

        var spoiler = _playerCar.transform.Find("Spoiler").gameObject;
        if (savingData.Spoiler == EquippedState.Usuall)
        {
            spoiler.gameObject.SetActive(false);
        }
        if (savingData.Spoiler == EquippedState.Premium) spoiler.gameObject.SetActive(true);

        var wheels = _playerCar.transform.Find("Wheels").gameObject;
        if (savingData.Wheels == EquippedState.Usuall) wheels.transform.localScale = Vector3.one;
        if (savingData.Wheels == EquippedState.Premium) wheels.transform.localScale = new Vector3(1.12f, 1f, 1f);
        if (_currentScene.name == "Garage"|| _currentScene.name == "Level01" || _currentScene.name == "Level02")
        {
            TextMeshProUGUI coinsText = GameObject.Find("PremiumCoinsField").GetComponent<TextMeshProUGUI>();
            coinsText.text = $"Cash: {PremiumCoins}";
        }
        
        _playerCar.SetActive(true);
    }
    public SavingData LoadSavingData()
    {
        SavingData savingData;
        if (!File.Exists(Application.dataPath + "/save.txt"))
        {
            savingData = new SavingData();
        }
        else
        {
            var jsonedString = File.ReadAllText(Application.dataPath + "/save.txt");
            savingData = GetFromJson(jsonedString);
        }
        return savingData;
    }
    private string SerializeToJson(SavingData savingData)
    {
        var json = JsonUtility.ToJson(savingData);
        return json;
    }
    private SavingData GetFromJson(string jsonString)
    {
        var loadedData = JsonUtility.FromJson<SavingData>(jsonString);
        return loadedData;
    }

    private void NewSceneCheck(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"scene changed to {scene.name}");
        _playerCar = null;
        _currentScene = scene;
        if (_currentScene.name == "Garage")
        {
            _garageShopScript = GameObject.Find("GarageManager").GetComponent<GarageShop>();
            Load();
        }
        else if (_currentScene.name != "Garage" && _currentScene.name != "Lobby")
        {
            //Debug.Log("online level loaded");
        }
        else _garageShopScript = null;
    }
    public void OnlineInitialize()
    {
        _playerCar = GameObject.Find("PlayerCar");
        _playerCar.SetActive(true);
        _mRenderer = _playerCar.transform.Find("Body").GetComponent<MeshRenderer>();
        Load();
    }
}
