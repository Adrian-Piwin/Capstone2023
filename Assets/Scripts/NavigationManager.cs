using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class NavigationManager : MonoBehaviour
{
    [Header("Settings")]
    [Range(0f, 300f)]
    public float thresholdMax;
    [Range(0f, 50f)]
    public float thresholdMin;

    public Color farColor;
    public Color closeColor;

    [Header("References")]
    public Image indicator;
    public GameObject btnStartGame;
    public TextMeshProUGUI txtLocationName;
    public TextMeshProUGUI txtLocationDistance;
    public GameObject loadingScreen;

    private DatabaseManager dbManager;
    private List<POI> locations;

    [SerializeField] private double currentLat;
    [SerializeField] private double currentLong;
    [SerializeField] private double targetLat;
    [SerializeField] private double targetLong;

    private GPSGetLocation gpsGetLocation;
    private bool isGPSReady;
    private bool isFirebaseReady;

    // Start is called before the first frame update
    void Start()
    {
        loadingScreen.SetActive(true);
        dbManager = GetComponent<DatabaseManager>();
        gpsGetLocation = GetComponent<GPSGetLocation>();

        indicator.color = closeColor;

        // Enable gyro
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
        }

        StartCoroutine(WaitForGPS());
        StartCoroutine(WaitForFirebase());
    }

    IEnumerator WaitForFirebase()
    {
        while (dbManager.isActive == false)
            yield return null;

        // Set locations
        locations = dbManager.locations;

        // Get todays location
        int day = Convert.ToInt32((DateTime.Today - new DateTime(2000, 1, 1)).TotalDays);
        System.Random rnd = new System.Random(day);
        int todaysPOI = rnd.Next(0, locations.Count);
        POI poi = locations[todaysPOI];

        // Get target location
        bool worked = Double.TryParse(poi.latitude, out double targetLat);
        Double.TryParse(poi.longitude, out double targetLong);

        Debug.Log(worked);

        // Setup other stuff
        txtLocationName.text = poi.name;

        Debug.Log("Firebase ready");
        isFirebaseReady = true;
    }

    IEnumerator WaitForGPS() 
    {
        while (gpsGetLocation.isActive == false)
            yield return null;

        currentLat = gpsGetLocation.userLocationLatitude;
        currentLong = gpsGetLocation.userLocationLongitude;

        Debug.Log("GPS Ready");
        isGPSReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGPSReady || !isFirebaseReady) return;
        else loadingScreen.SetActive(false);

        // Update current location
        currentLat = gpsGetLocation.userLocationLatitude;
        currentLong = gpsGetLocation.userLocationLongitude;

        // Update distance
        int distance = Convert.ToInt32(LatLongHelper.DistanceBetween(currentLong, currentLat, targetLong, targetLat));
        txtLocationDistance.text = distance + " Meters";

        // Lerp color based on distance
        indicator.color = Color.Lerp(closeColor, farColor, Mathf.InverseLerp(thresholdMin, thresholdMax, (float)distance));

        // Setup game button if close enough
        if (distance < thresholdMin)
            btnStartGame.SetActive(true);
        else
            btnStartGame.SetActive(false);
    }

}
