using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Storage;
using System.Threading.Tasks;
using System;

public class DatabaseScoreManager : MonoBehaviour
{
    public bool isLeaderboardReady;
    public List<PlayerScore> leaderboard = new List<PlayerScore>();
    private DatabaseReference mDatabase;

    public static DatabaseScoreManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void GetLeaderboard()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            FirebaseDatabase.DefaultInstance
           .GetReference("Leaderboard")
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
                       leaderboard.Add(JsonUtility.FromJson<PlayerScore>(place.GetRawJsonValue()));
                   }

                   isLeaderboardReady = true;
               }
           });
        });
    }

    public void SaveScore(string id, string name, int score) 
    {
        string key = mDatabase.Child("Scores").Push().Key;
        PlayerScore entry = new PlayerScore(id, name, score);
        Dictionary<string, System.Object> entryValues = entry.ToDictionary();
        Dictionary<string, System.Object> childUpdates = new Dictionary<string, System.Object>();
        childUpdates["/scores/" + id] = entryValues;
        childUpdates["/user-scores/" + id] = entryValues;
        mDatabase.UpdateChildrenAsync(childUpdates);
    }
}
