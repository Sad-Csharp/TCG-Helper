using System.Collections.Generic;
using BepInEx;
using TGCH_Helper.Patcher;
using TGCH_Helper.Utils;
using UnityEngine;
using Input = UnityEngine.Input;
using Patch = TGCH_Helper.Patcher.Patch;
using Skin = TGCH_Helper.Utils.SkinHelper;

namespace TGCH_Helper;

[BepInPlugin("com.tcghelper.mod", "TCGHelper", "1.0.0")]
public class Main : BaseUnityPlugin
{
    #region Misc
    
    private float setMoneyValue_;
    private Config Instance;
    
    #endregion
    #region Window

    private Rect windowRect_ = new Rect(0, 0, 200, 200);
    private bool showWirndow_;

    #endregion
    
    private void Start()
    {
        Debug.Log("TCG-Helper started.");
        Instance = Utils.Config.Instance;
        Instance.TryLoadConfig();
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

        if (!Loops.isCleanCustomersCoroutineRunning)
            StartCoroutine(Loops.CleanCustomers());
        
        if (!Loops.isPayBillsCoroutineRunning)
            StartCoroutine(Loops.PayBills());
    }

    private void OnGUI()
    {
        if (!showWirndow_)
            return;
        
        if (Skin.UniversalSkin != null)
            GUI.skin = Skin.UniversalSkin;
        
        windowRect_ = GUILayout.Window(GetHashCode(), windowRect_, DrawWindow, "TCG-Helper");
    }

    private void DrawWindow(int windowID)
    {
        GUILayout.Label("Add Money: " + setMoneyValue_);
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

        if (GUILayout.Button("Make Customers Buy"))
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
        
        if (GUILayout.Button("Make Customers Rich"))
        {
            List<Customer> customers = CustomerManager.Instance.GetCustomerList();
            foreach (Customer customer in customers)
            {
                if (customer == null)
                    continue;

                customer.m_MaxMoney = 1000000f;
            }
        }

        if (Skin.BoolButton(ref Loops.isCleanCustomersLoopEnabled, "CustomersPatch Cleaner Loop"))
        {
            if (Loops.isCleanCustomersCoroutineRunning)
                Loops.isCleanCustomersCoroutineRunning = false;
        }
        
        if (Skin.BoolButton(ref Loops.IsPayBillsLoopEnabled, "Pay Bills Loop"))
        {
            if (Loops.isPayBillsCoroutineRunning)
                Loops.isPayBillsCoroutineRunning = false;
        }
        
        Color previousColor = GUI.backgroundColor;
        GUI.backgroundColor = Utils.Config.Instance.IsCustomerPatch ? Color.green : Color.red;
        if (GUILayout.Button("Customers Smell Patch"))
        {
            Utils.Config.Instance.IsCustomerPatch = !Utils.Config.Instance.IsCustomerPatch;
        }
        GUI.backgroundColor = previousColor;
        
        GUI.backgroundColor = Utils.Config.Instance.IsWorkerPatch ? Color.green : Color.red;
        if (GUILayout.Button("Worker Stats Patch"))
        {
            Utils.Config.Instance.IsWorkerPatch = !Utils.Config.Instance.IsWorkerPatch;
        }
        GUI.backgroundColor = previousColor;
        
        GUI.backgroundColor = Utils.Config.Instance.IsShopCustomerCountPatch ? Color.green : Color.red;
        if (GUILayout.Button("Customer Count Patch"))
        {
            Utils.Config.Instance.IsShopCustomerCountPatch = !Utils.Config.Instance.IsShopCustomerCountPatch;
        }
        GUI.backgroundColor = previousColor;
        GUI.DragWindow();
    }

    private void ToggleWindow()
    {
        showWirndow_ = !showWirndow_;
        Cursor.visible = showWirndow_;
        Cursor.lockState = showWirndow_ ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void OnApplicationQuit()
    {
        Utils.Config.Instance.Save();
    }
}