using System.Collections.Generic;
using HarmonyLib;
using TCG_Helper.Utils;
using UnityEngine;

namespace TCG_Helper.Patcher;

public class WorkersPatch
{
    private static readonly Dictionary<string, float[]> WorkerStats = new();
    
    [HarmonyPatch(typeof(Worker), "EvaluateWorkerAttribute")]
    [HarmonyPostfix]
    public static void CacheWorkerStats(ref Worker __instance)
    {
        if (__instance == null)
            return;
        
        if (WorkerStats.ContainsKey(__instance.name))
            return;
        
        WorkerStats.Add(__instance.name, [__instance.m_GiveChangeTime, __instance.m_ScanItemTime, __instance.m_RestockTime]); 
        Debug.Log($"{__instance.name} stats: {WorkerStats[__instance.name][0]}, {WorkerStats[__instance.name][1]}, {WorkerStats[__instance.name][2]} cached!");
    }

    [HarmonyPatch(typeof(Worker), nameof(Worker.Update))]
    [HarmonyPostfix]
    public static void UpdatePostfix(ref Worker __instance)
    {
        if (__instance == null)
            return;
        
        if (Config.Instance.IsWorkerUpdatePatch)
        {
            __instance.m_GiveChangeTime = 0.3f;
            __instance.m_ScanItemTime = 0.3f;
            __instance.m_RestockTime = 0.3f;
            __instance.m_ExtraSpeedMultiplier = 50f;
        }
        else
        {
            __instance.m_GiveChangeTime = WorkerStats[__instance.name][0];
            __instance.m_ScanItemTime = WorkerStats[__instance.name][1];
            __instance.m_RestockTime = WorkerStats[__instance.name][2];
        }
    }
}