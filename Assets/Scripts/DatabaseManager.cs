using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System.Threading.Tasks;

public class DatabaseManager : MonoBehaviour
{
    public bool isActive;
    public List<POI> locations;

    private void Start()
    {
        locations = new List<POI>();

        GetPOILocations();
    }

    public void GetPOILocations()
    {

        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            FirebaseDatabase.DefaultInstance
           .GetReference("POI")
           .GetValueAsync().ContinueWith(task =>
           {
               if (task.IsFaulted)
               {
                    // Error handling
                    Debug.Log("Firebase error");
               }
               else if (task.IsCompleted)
               {

                    // Read snapshot
                   Debug.Log("Firebase success");
                   DataSnapshot snapshot = task.Result;
                   foreach (var place in snapshot.Children)
                   {
                       locations.Add(JsonUtility.FromJson<POI>(place.GetRawJsonValue()));
                   }
                   Debug.Log(locations[0]);
                   isActive = true;
               }
           });
        });
    }

}
