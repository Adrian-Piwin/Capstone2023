using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public List<GameObject> itemList;

    private List<string> GetItemNames()
    {
        List<string> names = new List<string>();
        foreach (var item in itemList) 
        {
            names.Add(item.name);
        }
        return names;
    }

    public List<string> GetUnlockedItemNames()
    {
        string namesRaw = PlayerPrefs.GetString("playerItems", null);
        if (namesRaw == null || namesRaw == "")
            return null;

        string[] namesArray = namesRaw.Split("/");
        List<string> names = new List<string>(); 
        foreach (var name in namesArray)
        {
            if (name != "" || name != " ")
                names.Add(name);
        }

        return names;
    }

    public string UnlockRandomItem() 
    {
        List<string> currentlyUnlocked = GetUnlockedItemNames();
        List<string> allItems = GetItemNames();

        List<string> canBeUnlocked = new List<string>();
        if (currentlyUnlocked != null)
            canBeUnlocked = allItems.Except(currentlyUnlocked).ToList();
        else
            canBeUnlocked = allItems;

        string itemToUnlock = canBeUnlocked[UnityEngine.Random.Range(0, canBeUnlocked.Count - 1)];
        PlayerPrefs.SetString("playerItems", PlayerPrefs.GetString("playerItems") == "" ? itemToUnlock : PlayerPrefs.GetString("playerItems") + "/" + itemToUnlock);

        return itemToUnlock;
    }

    public string GetSelectedItem() 
    {
        string selection = PlayerPrefs.GetString("playerItemSelected", null);
        return selection == null || selection == "" ? null : selection;
    }

    public void SetSelectedItem(string name) 
    {
        PlayerPrefs.SetString("playerItemSelected", name);
    }

    public GameObject GetItemObject(string name) 
    {
        GameObject obj = itemList.Where(o => o.name == name).FirstOrDefault();
        return obj;
    }
}
