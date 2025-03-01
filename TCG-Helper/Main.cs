using System.Collections.Generic;
using BepInEx;
using TCG_Helper.Patcher;
using TCG_Helper.Utils;
using UnityEngine;
using Input = UnityEngine.Input;
using Patch = TCG_Helper.Patcher.Patch;
using Skin = TCG_Helper.Utils.SkinHelper;

namespace TCG_Helper;

[BepInPlugin("com.tcghelper.mod", "TCG-Helper", "1.0.0")]
public class Main : BaseUnityPlugin
{
    #region Misc

    private float setMoneyValue_ = 1000f;
    private Config instance_;
    
    #endregion
    #region Window

    private Rect windowRect_ = new Rect(0, 0, 200, 200);
    private bool showWindow_;

    #endregion
    
    private void Start()
    {
        Debug.Log("TCG-Helper started.");
        instance_ = Utils.Config.Instance;
        instance_.TryLoadConfig();
        Patch.Init();
        Patch.TryPatch(typeof(WorkersPatch));
        Patch.TryPatch(typeof(CustomersPatch));
        Skin.TryLoadSkin();
        windowRect_ = Skin.CenterWindow(windowRect_);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            ToggleWindow();
        }

        if (!Loops.IsCleanCustomersCoroutineRunning)
            StartCoroutine(Loops.CleanCustomers());
        
        if (!Loops.IsPayBillsCoroutineRunning)
            StartCoroutine(Loops.PayBills());
    }

    private void OnGUI()
    {
        if (!showWindow_)
            return;
        
        if (Skin.UniversalSkin != null)
            GUI.skin = Skin.UniversalSkin;
        
        windowRect_ = GUILayout.Window(GetHashCode(), windowRect_, DrawWindow, "TCG-Helper");
    }

    private void DrawWindow(int windowID)
    {
        using (new GUILayout.VerticalScope("box"))
        {
            Color previousColor = GUI.backgroundColor;

            GUILayout.Label("<b>Patches</b>");
            
            // Customers dont smell patch
            GUI.backgroundColor = instance_.IsCustomerSmellyPatch ? Color.green : Color.red;
            instance_.IsCustomerSmellyPatch = GUILayout.Toggle(instance_.IsCustomerSmellyPatch, "Customers Smell Patch");
            GUI.backgroundColor = previousColor;
        
            // Workers stats (speed) patch
            GUI.backgroundColor = instance_.IsWorkerUpdatePatch ? Color.green : Color.red;
            instance_.IsWorkerUpdatePatch = GUILayout.Toggle(instance_.IsWorkerUpdatePatch, "Worker Update Patch");
            GUI.backgroundColor = previousColor;
        
            // Shop customer count patch
            GUI.backgroundColor = instance_.IsShopCustomerCountPatch ? Color.green : Color.red;
            instance_.IsShopCustomerCountPatch = GUILayout.Toggle(instance_.IsShopCustomerCountPatch, "Shop Customer Count Patch");
            GUI.backgroundColor = previousColor;
        }
        
        GUILayout.Label($"Add Money: {setMoneyValue_:N0}");
        Skin.IncrementedSlider(ref setMoneyValue_, 0, 10000, 100);
        if (GUILayout.Button("Add Money"))
        {
            if (setMoneyValue_ != 0f)
            {
                CEventManager.QueueEvent(new CEventPlayer_AddCoin(setMoneyValue_));
                Debug.LogWarning("Added " + setMoneyValue_ + " coins.");
            }
            else
                Debug.LogWarning("No value set.");
        }

        if (GUILayout.Button("Pay Bills"))
        {
            CPlayerData.SetBill(EBillType.Electric, 0,0f);
            CPlayerData.SetBill(EBillType.Rent, 0,0f);
            CPlayerData.SetBill(EBillType.Employee, 0,0f);
            Debug.Log("Bills paid.");
        }

        if (GUILayout.Button("Make All Customers Buy"))
        {
            List<Customer> customers = CustomerManager.Instance.GetCustomerList();
            foreach (Customer customer in customers)
            {
                if (customer == null)
                    continue;
                
                customer.SetState(ECustomerState.WantToBuyCard);
                customer.AttemptFindCardShelf();
            }
        }
        
        if (GUILayout.Button("Make All Customers Rich"))
        {
            List<Customer> customers = CustomerManager.Instance.GetCustomerList();
            foreach (Customer customer in customers)
            {
                if (customer == null)
                    continue;

                customer.m_MaxMoney = 1000000f;
            }
        }

        if (Skin.BoolButton(ref Loops.IsCleanCustomersLoopEnabled, "Clean Customers Loop"))
        {
            if (Loops.IsCleanCustomersCoroutineRunning)
                Loops.IsCleanCustomersCoroutineRunning = false;
        }
        
        if (Skin.BoolButton(ref Loops.IsPayBillsLoopEnabled, "Pay Bills Loop"))
        {
            if (Loops.IsPayBillsCoroutineRunning)
                Loops.IsPayBillsCoroutineRunning = false;
        }
        GUI.DragWindow();
    }

    private void ToggleWindow()
    {
        showWindow_ = !showWindow_;
        Cursor.visible = showWindow_;
        Cursor.lockState = showWindow_ ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void OnApplicationQuit()
    {
        Utils.Config.Instance.Save();
    }
}