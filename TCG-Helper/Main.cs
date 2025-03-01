using System.Collections.Generic;
using BepInEx;
using TCG_Helper.Patcher;
using TCG_Helper.Utils;
using UnityEngine;
using Input = UnityEngine.Input;
using Patch = TCG_Helper.Patcher.Patch;

namespace TCG_Helper;

[BepInPlugin("com.tcghelper.mod", "TCG-Helper", "1.0.0")]
public class Main : BaseUnityPlugin
{
    #region Misc

    private float setMoneyValue_ = 1000f;
    
    #endregion
    #region Window

    private Rect windowRect_ = new Rect(0, 0, 200, 200);
    private bool showWindow_;

    #endregion
    
    private void Start()
    {
        Debug.Log("TCG-Helper started.");
        Utils.Config.Instance.TryLoadConfig();
        Patch.Init();
        Patch.TryPatch(typeof(WorkersPatch));
        Patch.TryPatch(typeof(CustomersPatch));
        UI.TryLoadSkin();
        windowRect_ = UI.CenterWindow(windowRect_);
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
        
        if (UI.UniversalSkin != null)
            GUI.skin = UI.UniversalSkin;
        
        windowRect_ = GUILayout.Window(GetHashCode(), windowRect_, DrawWindow, "TCG-Helper");
    }

    private void DrawWindow(int windowID)
    {
        using (new GUILayout.VerticalScope("box"))
        {
            Color previousColor = GUI.backgroundColor;

            GUILayout.Label("<b>Patches</b>");
            
            // Customers dont smell patch
            GUI.backgroundColor = Utils.Config.Instance.IsCustomerSmellyPatch ? Color.green : Color.red;
            Utils.Config.Instance.IsCustomerSmellyPatch = GUILayout.Toggle(Utils.Config.Instance.IsCustomerSmellyPatch, "Customers Smell Patch");
            GUI.backgroundColor = previousColor;
        
            // Workers stats (speed) patch
            GUI.backgroundColor = Utils.Config.Instance.IsWorkerUpdatePatch ? Color.green : Color.red;
            Utils.Config.Instance.IsWorkerUpdatePatch = GUILayout.Toggle(Utils.Config.Instance.IsWorkerUpdatePatch, "Worker Update Patch");
            GUI.backgroundColor = previousColor;
        
            // Shop customer count patch
            GUI.backgroundColor = Utils.Config.Instance.IsShopCustomerCountPatch ? Color.green : Color.red;
            Utils.Config.Instance.IsShopCustomerCountPatch = GUILayout.Toggle(Utils.Config.Instance.IsShopCustomerCountPatch, "Shop Customer Count Patch");
            GUI.backgroundColor = previousColor;
            
            // Customers fast patch
            GUI.backgroundColor = Utils.Config.Instance.IsCustomerFastPatch ? Color.green : Color.red;
            Utils.Config.Instance.IsCustomerFastPatch = GUILayout.Toggle(Utils.Config.Instance.IsCustomerFastPatch, "Customers Fast Patch");
            GUI.backgroundColor = previousColor;
        }
        
        GUILayout.Label($"FOV: {Utils.Config.Instance.SetFOV}");
        if (UI.IncrementedSlider(ref Utils.Config.Instance.SetFOV, 0, 75))
        {
            foreach (Camera camera in Camera.allCameras)
            {
                camera.fieldOfView = Utils.Config.Instance.SetFOV;
            }
        }
        
        GUILayout.Label($"Add Coins: {setMoneyValue_:N0}");
        UI.IncrementedSlider(ref setMoneyValue_, 0, 10000, 100);
        if (GUILayout.Button("Add Coins"))
        {
            if (setMoneyValue_ != 0f)
            {
                CEventManager.QueueEvent(new CEventPlayer_AddCoin(setMoneyValue_));
                Debug.LogWarning("Added " + setMoneyValue_ + " coins.");
            }
            else
                Debug.LogWarning("No value set.");
        }

        if (GUILayout.Button("Open God Pack"))
        {
            CardOpeningSequence openPacks = CardOpeningSequence.Instance;

            if (openPacks == null)
            {
                Debug.LogError("CardOpeningSequence is null.");
                return;
            }
            
            openPacks.OpenScreen(collectionPackType: ECollectionPackType.GhostPack, false);
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

        if (UI.ButtonToggle(ref Loops.IsCleanCustomersLoopEnabled, "Clean Customers Loop"))
        {
            if (Loops.IsCleanCustomersCoroutineRunning)
                Loops.IsCleanCustomersCoroutineRunning = false;
        }
        
        if (UI.ButtonToggle(ref Loops.IsPayBillsLoopEnabled, "Pay Bills Loop"))
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