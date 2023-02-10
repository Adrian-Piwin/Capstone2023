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
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            FirebaseDatabase.DefaultInstance
           .GetReference("POI")
           .GetValueAsync().ContinueWith(task =>
           {
               if (task.IsFaulted)
               {
                    Debug.Log("Firebase error");
               }
               else if (task.IsCompleted)
               {
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
        StorageReference storageReference = storage.GetReferenceFromUrl("gs://capstone2023-83950.appspot.com/" + path);

        storageReference.GetDownloadUrlAsync().ContinueWithOnMainThread(task => {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                UnityWebRequest www = UnityWebRequest.Get(task.Result);
                www.SendWebRequest();

                float timer = 10f + Time.time;
                while (!www.isDone) { if (timer < Time.time) break;  }

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    CreateImage(www.downloadHandler.data);
                }
            }
        });
    }

    private void CreateImage(byte[] fileData) 
    {
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);
        image = texture;

        isImageReady = true;
    }

}
