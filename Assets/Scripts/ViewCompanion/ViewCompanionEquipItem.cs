using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewCompanionEquipItem : MonoBehaviour
{
    public ItemManager itemManager;

    private List<string> availableItemList;
    private string currentSelectedItem;
    private int itemIndex = 0;
    private AttachItem bearAttachItem;

    // Start is called before the first frame update
    void Start()
    {
        // Checked if there are items unlocked to equip
        currentSelectedItem = itemManager.GetSelectedItem();
        availableItemList = itemManager.GetUnlockedItemNames();
        if (currentSelectedItem == null && availableItemList == null) return;

        // Make nothing first in list
        if (availableItemList == null)
        {
            availableItemList = new List<string>();
        }

        availableItemList.Insert(0, "nothing");
    }

    public void SwapItemBtnClicked()
    {
        // If no items, dont do anything
        if (currentSelectedItem == null && availableItemList == null) return;
        Debug.Log("item found");

        // Find AR bear, if not found dont do anything
        if (bearAttachItem == null)
        { 
            GameObject bear = GameObject.Find("BearPrefabAR");
            if (bear == null) return;
            bearAttachItem = bear.GetComponent<AttachItem>();
        }

        // Cycle through items, selecting them
        itemIndex++;
        itemIndex = itemIndex >= availableItemList.Count ? 0 : itemIndex;
        Debug.Log($"on item {availableItemList[itemIndex]}");
        bearAttachItem.SetNewItem(availableItemList[itemIndex]);
        itemManager.SetSelectedItem(availableItemList[itemIndex]);

        Debug.Log($"equiped {availableItemList[itemIndex]}");
    }
}
