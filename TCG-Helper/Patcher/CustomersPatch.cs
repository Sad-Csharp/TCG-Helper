using HarmonyLib;
using TCG_Helper.Utils;
using UnityEngine;

namespace TCG_Helper.Patcher;

[HarmonyPatch]
public class CustomersPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Customer), nameof(Customer.SetSmelly))]
    public static bool Prefix(ref Customer __instance)
    {
        if (!Config.Instance.IsCustomerSmellyPatch)
            return true;

        if (__instance == null)
            return true;

        return false;
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CustomerManager), "EvaluateMaxCustomerCount")]
    private static void EvaluateMaxCustomerCountPostfix(ref int ___m_CustomerCountMax, ref int ___m_PlayTableSitdownCustomerCount, ref int ___m_TotalCurrentCustomerCount)
    {
        
        if (!Config.Instance.IsShopCustomerCountPatch)
        {
            Debug.Log("[Post - EvaluateMaxCustomerCount] Plugin is disabled");
        }
        else
        {
            Debug.Log("[Post - EvaluateMaxCustomerCount] Total : 28");
            int num = ___m_CustomerCountMax - Mathf.CeilToInt(___m_PlayTableSitdownCustomerCount / 2f);
            ___m_CustomerCountMax = Mathf.Clamp(num + ___m_PlayTableSitdownCustomerCount, 3, 28);
        }
    }
}