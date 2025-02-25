using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TGCH_Helper;

public static class Loops
{
    public static bool IsPayBillsLoopEnabled;
    public static bool isCleanCustomersLoopEnabled;
    public static bool isPayBillsCoroutineRunning;
    public static bool isCleanCustomersCoroutineRunning;
    
    
    public static IEnumerator PayBills()
    {
        isPayBillsCoroutineRunning = true;
        
        while (isPayBillsCoroutineRunning)
        {
            yield return new WaitForSeconds(20f);
        
            Scene currentScene_ = SceneManager.GetActiveScene();
        
            if (!IsPayBillsLoopEnabled)
                yield break;
            
            if (currentScene_.name != "Start")
                yield break;

            if (!IsPayBillsLoopEnabled)
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
        isCleanCustomersCoroutineRunning = true;

        while (isCleanCustomersCoroutineRunning)
        {
            yield return new WaitForSeconds(15f);
        
            Scene currentScene_ = SceneManager.GetActiveScene();
        
            if (!isCleanCustomersLoopEnabled)
                isCleanCustomersCoroutineRunning = false;
       
            if (currentScene_.name != "Start") 
                yield break;

            if (!isCleanCustomersLoopEnabled)
                yield break;
        
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
                Debug.LogError("Error in CustomersPatch Cleaner: " + e.Message);
            }
        }
    }
}