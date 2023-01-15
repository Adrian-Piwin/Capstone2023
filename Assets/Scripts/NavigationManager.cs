using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class NavigationManager : MonoBehaviour
{
    public Transform arrow;
    public List<GameObject> btnStartGame;
    public TextMeshProUGUI txtLocationName;

    private DatabaseManager dbManager;
    private List<POI> locations;

    private double currentLat;
    private double currentLong;
    private double targetLat;
    private double targetLong;

    private GPSGetLocation gpsGetLocation;

    // Start is called before the first frame update
    void Start()
    {
        dbManager = GetComponent<DatabaseManager>();
        gpsGetLocation = GetComponent<GPSGetLocation>();

        locations = dbManager.GetPOILocations();
        int day = Convert.ToInt32((DateTime.Today - new DateTime(2000, 1, 1)).TotalDays);
        System.Random rnd = new System.Random(day);
        int todaysPOI = rnd.Next(0, locations.Count);

        SetupPOI(locations[todaysPOI]);
    }

    private void SetupPOI(POI poi) 
    {
        // Setup lat / long
        currentLat = gpsGetLocation.userLocationLatitude;
        currentLong = gpsGetLocation.userLocationLongitude;
        targetLat = Convert.ToDouble(poi.latitude);
        targetLong = Convert.ToDouble(poi.latitude);

        // Setup other stuff
        txtLocationName.text = poi.name;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
