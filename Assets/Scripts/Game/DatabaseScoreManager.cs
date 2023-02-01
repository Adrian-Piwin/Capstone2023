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

    async public Task<List<PlayerScore>> GetScores()
    {
        List<PlayerScore> pScores = new List<PlayerScore>();

        await FirebaseDatabase.DefaultInstance
           .GetReference("scores")
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
                       pScores.Add(JsonUtility.FromJson<PlayerScore>(place.GetRawJsonValue()));
                   }
               }
           });

        return pScores;
    }

    async public Task<PlayerScore> GetScore(string id)
    {
        PlayerScore pScore = null;

        await FirebaseDatabase.DefaultInstance
           .GetReference("scores")
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
                       PlayerScore score = (JsonUtility.FromJson<PlayerScore>(place.GetRawJsonValue()));
                       if (score.id == id)
                           pScore = score;
                   }
               }
           });

        return pScore;
    }

    public void SaveScore(string id, string name, int score) 
    {
        PlayerScore entry = new PlayerScore(id, name, score);
        Dictionary<string, System.Object> entryValues = entry.ToDictionary();
        Dictionary<string, System.Object> childUpdates = new Dictionary<string, System.Object>();
        childUpdates["/scores/" + id] = entryValues;
        //childUpdates["/user-scores/" + id] = entryValues;
        mDatabase.UpdateChildrenAsync(childUpdates);
    }
}
