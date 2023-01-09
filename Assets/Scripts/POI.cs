using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POI
{
    public string name;
    public string description;
    public string latitude;
    public string longitude;
    public string image;

    public POI(string name, string description, string latitude, string longitude, string image) 
    {
        this.name = name;
        this.description = description;
        this.latitude = latitude;
        this.longitude = longitude;
        this.image = image;
    }
}
