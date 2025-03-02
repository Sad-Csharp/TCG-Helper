using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace TCG_Helper.Utils;

public static class UI
{
    public static GUISkin UniversalSkin;

    public static void TryLoadSkin()
    {
        try
        {
            byte[] bundleData = LoadSkinFromMemory("darkskin");
            AssetBundle asset = AssetBundle.LoadFromMemory(bundleData);
            UniversalSkin = asset.LoadAsset<GUISkin>("DarkSkin");
        }
        catch (Exception ex)
        {
            Debug.LogError("Unable to load skin, error: " + ex.Message);
        }
    }

    private static Byte[] LoadSkinFromMemory(string resourceName)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        using Stream stream = assembly.GetManifestResourceStream(assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith(resourceName)));

        if (stream == null) 
            return null;

        byte[] data = new byte[stream.Length];
        _ = stream.Read(data, 0, data.Length);
        return data;
    }

    public static Rect CenterWindow(Rect windowRect)
    {
        windowRect.x = (Screen.width - windowRect.width) / 2;
        windowRect.y = (Screen.height - windowRect.height) / 2;
        return windowRect;
    }
    
    public static bool IncrementedSlider(ref float value, float min, float max, float increment = 1, params GUILayoutOption[] options)
    {
        float newVal = GUILayout.HorizontalSlider(value, min, max, options);
        if (Mathf.Approximately(newVal, value)) 
            return false;
        
        value = Mathf.Round(newVal / increment) * increment;
        return true;
    }

    public static bool ButtonToggle(ref bool value, string label, params GUILayoutOption[] options)
    {
        Color prevColor = GUI.backgroundColor;
        GUI.backgroundColor = value ? Color.green : Color.red;
        bool clicked = GUILayout.Button(label, options);
        if (clicked)
            value = !value;
        GUI.backgroundColor = prevColor;
        return clicked;
    }

    public static bool Toggle(ref bool value, GameObject targetObject, string label, params string[] fieldNames)
    {
        if (targetObject == null)
        {
            Debug.LogError("Target object is null");
            return false;
        }
        
        Color prevColor = GUI.backgroundColor;
        GUI.backgroundColor = value ? Color.green : Color.red;

        bool previousValue = value;
        bool newValue = GUILayout.Toggle(value, label);

        GUI.backgroundColor = prevColor;

        if (newValue == previousValue)
            return false;

        value = newValue;
        Debug.Log($"Toggled '{label}' to {value}");

        bool anyFieldModified = false;

        foreach (Component component in targetObject.GetComponents<Component>())
        {
            Type type = component.GetType();
            foreach (string fieldName in fieldNames)
            {
                FieldInfo field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null && field.FieldType == typeof(bool))
                {
                    field.SetValue(component, !value);
                    Debug.Log($"Field '{fieldName}' in '{type.Name}' set to {value}");
                    anyFieldModified = true;
                    continue;
                }

                PropertyInfo property = type.GetProperty(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (property != null && property.PropertyType == typeof(bool) && property.CanWrite)
                {
                    property.SetValue(component, value);
                    Debug.Log($"Property '{fieldName}' in '{type.Name}' set to {value}");
                    anyFieldModified = true;
                }
            }
        }

        if (!anyFieldModified)
            Debug.LogWarning($"No valid boolean fields/properties found for '{label}' in any components.");

        return true;
    }
}