using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TGCH_Helper;

public static class Loops
{
    public static bool isPayBillsLoopEnabled;
    public static bool isCleanCustomersLoopEnabled;
    private static Scene currentScene_ = SceneManager.GetActiveScene();
    
    
    public static IEnumerator PayBills()
    {
        yield return new WaitForSeconds(30f);
            
        if (!isPayBillsLoopEnabled)
            yield break;
            
        if (currentScene_.name != "Start")
            yield break;

        if (!isPayBillsLoopEnabled)
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
    
    public static IEnumerator CleanCustomers()
    {
        yield return new WaitForSeconds(30f);
        
        if (!isCleanCustomersLoopEnabled)
            yield break;
       
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