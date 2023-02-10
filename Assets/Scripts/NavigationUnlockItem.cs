using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationUnlockItem : MonoBehaviour
{
    public GameObject itemUnlockUI;
    public Transform itemParent;
    public ItemManager itemManager;

    private bool menuActive;

    public void UnlockItem() 
    {
        if (menuActive) return;
        string itemName = itemManager.UnlockRandomItem();
        Instantiate(itemManager.GetItemObject(itemName), itemParent);
        itemUnlockUI.SetActive(true);
        menuActive = true;
    }

    public void CloseWindow() 
    {
        itemUnlockUI.SetActive(false);
    }
}
