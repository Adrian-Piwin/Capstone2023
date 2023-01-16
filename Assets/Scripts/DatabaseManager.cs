using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System.Threading.Tasks;
using UnityEngine.UI;
using Firebase.Storage;
using UnityEngine.Networking;
using System.Collections;
using Firebase.Extensions;
using System;

public class DatabaseManager : MonoBehaviour
{
    public bool isLocationReady;
    public bool isImageReady;

    public List<POI> locations;
    public Texture2D image;

    private void Start()
    {
        locations = new List<POI>();

        GetPOILocations();
    }

    private void GetPOILocations()
    {
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;

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
                   DataSnapshot snapshot = task.Result;
                   foreach (var place in snapshot.Children)
                   {
                       locations.Add(JsonUtility.FromJson<POI>(place.GetRawJsonValue()));
                   }

                   isLocationReady = true;
               }
           });
        });
    }

    public void GetPOIImage(string path) 
    {
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        var storageReference = storage.GetReferenceFromUrl("gs://capstone2023-83950.appspot.com/");
        StorageReference imageRef = storageReference.Child(path);
        Debug.Log(path);

        string localUrl = "file:///local/images/" + path;

        if (System.IO.File.Exists(localUrl))
        {
            // Use image in local filesystem
            CreateImage(localUrl);
        }
        else
        {
            // Download to the local filesystem
            imageRef.GetFileAsync(localUrl).ContinueWithOnMainThread(task =>
            {
                if (!task.IsFaulted && !task.IsCanceled)
                {
                    Debug.Log("downloaded");
                    CreateImage(localUrl);
                }
            });
        }
    }

    private void CreateImage(string localUrl) 
    {
        byte[] fileData = System.IO.File.ReadAllBytes(localUrl);
        Debug.Log("WHA1");

        Texture2D texture = new Texture2D(2, 2);
        Debug.Log("WHA2");

        texture.LoadImage(fileData);
        Debug.Log("WHA3");

        image = texture;
        Debug.Log("WHA4");

        isImageReady = true;
    }

}
