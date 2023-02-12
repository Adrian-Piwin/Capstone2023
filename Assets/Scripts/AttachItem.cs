using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachItem : MonoBehaviour
{
    public ItemManager itemManager;
    public Transform itemParent;

    private GameObject attachedObj;

    // Start is called before the first frame update
    void Start()
    {
        string selectedItem = itemManager.GetSelectedItem();
        if (selectedItem == null) return;

        attachedObj = Instantiate(itemManager.GetItemObject(selectedItem), itemParent);
    }

    public void SetNewItem(string item) 
    {
        if (attachedObj != null)
            Destroy(attachedObj);

        attachedObj = Instantiate(itemManager.GetItemObject(item), itemParent);
    }
}
