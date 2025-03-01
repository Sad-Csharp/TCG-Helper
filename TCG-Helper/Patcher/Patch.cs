using System;
using HarmonyLib;
using UnityEngine;

namespace TCG_Helper.Patcher;

public static class Patch
{
    private static Harmony patcher_;

    public static void Init()
    {
        if (patcher_ != null)
            return;

        patcher_ = new Harmony("tcghelper.patcher");
        Debug.LogWarning("Patcher initialized.");
    }

    public static void TryPatch(Type type)
    {
        try
        {
            patcher_.PatchAll(type);
            Debug.LogWarning("Patch applied for " + type.Name);
        }
        catch (Exception ex)
        {
             Debug.LogError("Unable to apply patches for " + type.Name + ", error: " + ex.Message);
        }
    }
}