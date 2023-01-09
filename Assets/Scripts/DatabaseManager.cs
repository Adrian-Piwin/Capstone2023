using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using System;

public class DatabaseManager : MonoBehaviour
{
    public DatabaseReference dbReference;

    // Start is called before the first frame update
    void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public IEnumerator GetPOILocations(Action<List<POI>> onCallback) 
    {
        var poiData = dbReference.Child("POI").GetValueAsync();

        yield return new WaitUntil(predicate: () => poiData.IsCompleted);

        if (poiData != null)
        {
            DataSnapshot snapshot = poiData.Result;
            onCallback.Invoke((List<POI>)snapshot.Value);
        }
    } 
}
