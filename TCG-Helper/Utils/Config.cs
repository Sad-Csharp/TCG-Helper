using System.IO;
using UnityEngine;

namespace TCG_Helper.Utils;

public class Config
{
    public static Config Instance { get; private set; }
    private static readonly string ConfigPath = Path.Combine(Directory.GetParent(Application.dataPath)?.FullName ?? string.Empty, "BepInEx", "config", "TCG-Helper.json");
    public bool IsWorkerUpdatePatch { get; set; }
    public bool IsCustomerSmellyPatch{ get; set; }
    public bool IsCustomerFastPatch { get; set; } = true;
    public bool IsShopCustomerCountPatch { get; set; } = true;
    public float SetFOV = 60f;
    
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