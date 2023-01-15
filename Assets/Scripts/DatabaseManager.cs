using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using System;
using Firebase.Extensions;

public class DatabaseManager : MonoBehaviour
{
    public DatabaseReference dbReference;

    // Start is called before the first frame update
    void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public List<POI> GetPOILocations() 
    {
        List<POI> locations = new List<POI>();

        FirebaseDatabase.DefaultInstance
            .GetReference("POI")
            .GetValueAsync().ContinueWithOnMainThread(task => {
                if (task.IsFaulted)
                {
                    // Error handling
                    Debug.Log(task.Exception);
                }
                else if (task.IsCompleted)
                {
                    // Read snapshot
                    DataSnapshot snapshot = task.Result;
                    foreach (var place in snapshot.Children)
                    {
                        locations.Add(JsonUtility.FromJson<POI>(place.GetRawJsonValue()));
                    }

                }
            });

        return locations;
    }
}
