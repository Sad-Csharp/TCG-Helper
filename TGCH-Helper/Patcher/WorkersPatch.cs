using HarmonyLib;
using TGCH_Helper.Utils;
using UnityEngine;

namespace TGCH_Helper.Patcher;

[HarmonyPatch]
public class WorkersPatch
{
    [HarmonyPatch(typeof(Worker), "EvaluateWorkerAttribute")]
    [HarmonyPostfix]
    public static void Postfix(ref Worker __instance)
    {
        if (!Config.Instance.IsWorkerPatch)
        {
            Debug.Log("Worker stats not patched. It's disabled in the config.");
            return;
        }
        
        if (__instance == null)
            return;
        
        __instance.m_GiveChangeTime = 0.3f;
        __instance.m_ScanItemTime = 0.3f;
        __instance.m_RestockTime = 0.3f;
        Debug.LogWarning("Worker stats patched.");
    }
}