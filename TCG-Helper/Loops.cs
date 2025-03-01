using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TCG_Helper;

public static class Loops
{
    public static bool IsPayBillsLoopEnabled;
    public static bool IsCleanCustomersLoopEnabled;
    public static bool IsPayBillsCoroutineRunning;
    public static bool IsCleanCustomersCoroutineRunning;
    
    
    public static IEnumerator PayBills()
    {
        IsPayBillsCoroutineRunning = true;

        Scene currentScene = SceneManager.GetActiveScene();
        while (IsPayBillsCoroutineRunning)
        {
            yield return new WaitForSeconds(20f);

            if (!IsPayBillsLoopEnabled)
            {
                IsPayBillsCoroutineRunning = false;
                IsPayBillsLoopEnabled = false;
                yield break;
            }
            
            if (currentScene.name != "Start")
                yield break;
        
            try
            {
                CPlayerData.SetBill(EBillType.Electric, 0, 0f);
                CPlayerData.SetBill(EBillType.Rent, 0, 0f);
                CPlayerData.SetBill(EBillType.Employee, 0, 0f);
                Debug.Log("Bills paid.");
            }
            catch (Exception e)
            {
                Debug.LogError("Error in Pay Bills: " + e.Message);
            }
        }
    }
    
    public static IEnumerator CleanCustomers()
    {
        IsCleanCustomersCoroutineRunning = true;

        while (IsCleanCustomersCoroutineRunning)
        {
            yield return new WaitForSeconds(15f);
        
            Scene currentScene = SceneManager.GetActiveScene();

            if (!IsCleanCustomersLoopEnabled)
            {
                IsCleanCustomersCoroutineRunning = false;
                IsCleanCustomersLoopEnabled = false;
                break;
            }
            
            if (currentScene.name != "Start") 
                break;
        
            try
            {
                List<Customer> customers = CustomerManager.Instance.GetCustomerList();
                foreach (Customer customer in customers)
                {
                    if (customer == null)
                        continue;
                    
                    customer.DeodorantSprayCheck(customer.transform.position, 10000f, 10000);
                    Debug.Log($"{customer.name} cleaned.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error in Customers Loop Cleaner: " + e.Message);
            }
        }
    }
}