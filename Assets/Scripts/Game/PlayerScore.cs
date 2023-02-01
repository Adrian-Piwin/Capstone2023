using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerScore
{
    public string id;
    public string name;
    public int score;

    public PlayerScore(string id, string name, int score)
    {
        this.id = id;
        this.name = name;
        this.score = score;
    }

    public Dictionary<string, System.Object> ToDictionary()
    {
        Dictionary<string, System.Object> result = new Dictionary<string, System.Object>();
        result["id"] = id;
        result["name"] = name;
        result["score"] = score;

        return result;
    }
}
