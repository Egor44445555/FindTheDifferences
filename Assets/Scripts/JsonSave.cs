using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using System.Runtime.InteropServices;

public class JsonSave : MonoBehaviour
{
    public static JsonSave main;
    public string mainJson = "";

    [DllImport("__Internal")] static extern void SavePlayerData(string jsonData, string mainJson);

    TowerProjectileItemArray dataWeb;

    void Awake()
    {
        main = this;
    }

    void Start()
    {
        TowerProjectileItemArray projectileArray = LoadFileProjectileArray();
        List<PurchasedProjectiles> newPurchasedProjectiles = new List<PurchasedProjectiles>();
        List<TowerProjectileItem> towerProjectileItems = new List<TowerProjectileItem>();
        List<ResourcesItem> resourceItems = new List<ResourcesItem>();

        if (projectileArray == null)
        {
            resourceItems.Add(new ResourcesItem(0, "Bamboo"));
            resourceItems.Add(new ResourcesItem(0, "Crystal"));
            resourceItems.Add(new ResourcesItem(0, "Fang"));
            resourceItems.Add(new ResourcesItem(0, "Feather"));

            SaveProjectileArray(towerProjectileItems.ToArray(), resourceItems.ToArray(), newPurchasedProjectiles.ToArray(), 0);
        }
    }

    public void ResetFileProjectileArray()
    {
        TowerProjectileItemArray projectileArray = LoadFileProjectileArray();
        List<PurchasedProjectiles> newPurchasedProjectiles = new List<PurchasedProjectiles>();
        List<TowerProjectileItem> towerProjectileItems = new List<TowerProjectileItem>();
        List<ResourcesItem> resourceItems = new List<ResourcesItem>();

        resourceItems.Add(new ResourcesItem(0, "Bamboo"));
        resourceItems.Add(new ResourcesItem(0, "Crystal"));
        resourceItems.Add(new ResourcesItem(0, "Fang"));
        resourceItems.Add(new ResourcesItem(0, "Feather"));

        SaveProjectileArray(towerProjectileItems.ToArray(), resourceItems.ToArray(), newPurchasedProjectiles.ToArray(), 0);
        PlayerPrefs.DeleteAll();
    }

    public void FirstSaveProjectileArray()
    {
        TowerProjectileItemArray projectileArray = LoadFileProjectileArray();
        SaveProjectileArray(projectileArray.items, projectileArray.resources, projectileArray.purchasedProjectiles, projectileArray.currentLevel);
    }

    public void SaveProjectileArray(TowerProjectileItem[] array, ResourcesItem[] resourcesArray, PurchasedProjectiles[] purchasedProjectilesArray, int _currentLevel)
    {
        TowerProjectileItemArray wrapper = new TowerProjectileItemArray { currentLevel = _currentLevel, resources = resourcesArray, items = array, purchasedProjectiles = purchasedProjectilesArray };
        string json = JsonUtility.ToJson(wrapper);
        string path = Path.Combine(Application.persistentDataPath, "playerData.json");

        File.WriteAllText(path, json);

        if (GamePushManager.main != null)
        {
            GamePushManager.main.SetPlayerProgress(json);
        }
        else
        {
            #if UNITY_WEBGL
                SavePlayerData(json, mainJson);
            #endif
        }
    }

    public TowerProjectileItemArray LoadFileProjectileArray()
    {
        if (!string.IsNullOrEmpty(mainJson))
        {
            dataWeb = JsonUtility.FromJson<TowerProjectileItemArray>(mainJson);
            return dataWeb != null ? dataWeb : null;
        }
        else
        {
            string path = Path.Combine(Application.persistentDataPath, "playerData.json");

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);

                if (GamePushManager.main != null)
                {
                    string loadedProgress = GamePushManager.main.GetPlayerProgress();

                    if (loadedProgress != "0" && loadedProgress != "")
                    {
                        json = loadedProgress;
                    }
                }

                TowerProjectileItemArray data = JsonUtility.FromJson<TowerProjectileItemArray>(json);

                return data != null ? data : null;
            }
            else  
            {
                return null;
            }   
        }
    }
}