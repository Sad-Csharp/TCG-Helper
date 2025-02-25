using System.IO;
using UnityEngine;

namespace TGCH_Helper.Utils;

public class Config
{
    public static Config Instance { get; private set; }
    private static readonly string ConfigPath = Path.Combine(Directory.GetParent(Application.dataPath)?.FullName ?? string.Empty, "BepInEx", "config", "TGCH.json");
    public bool IsWorkerPatch { get; set; } = true;
    public bool IsCustomerPatch{ get; set; } = true;
    public bool IsShopCustomerCountPatch { get; set; } = true;
    
    static Config()
    {
        Instance = new Config();
    }

    public void TryLoadConfig()
    {
        if (!File.Exists(ConfigPath))
        {
            Instance = new Config();
            Instance.Save();
            Debug.LogError("Config file not found, creating new one.");
        }
        else
        {
            Instance = File.ReadAllText(ConfigPath).FromJson<Config>();
            Debug.LogWarning("Config file loaded.");
        }
    }

    public void Save()
    {
        File.WriteAllText(ConfigPath, this.ToJson());
        Debug.LogWarning("Config file saved.");
    }
}