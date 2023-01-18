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
    public TextMeshProUGUI txtLocationDesc;
    public TextMeshProUGUI txtLocationDistance;
    public RawImage imgLocation;
    public GameObject loadingScreen;

    private DatabaseManager dbManager;
    private POI location;

    [SerializeField] private double currentLat;
    [SerializeField] private double currentLong;
    [SerializeField] private double targetLat;
    [SerializeField] private double targetLong;

    private GPSGetLocation gpsGetLocation;
    private bool isGPSReady;
    private bool isFirebaseLocationsReady;
    private bool isFirebaseImageReady;

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
        StartCoroutine(WaitForFirebaseLocation());
        StartCoroutine(WaitForFirebaseImage());
    }

    IEnumerator WaitForFirebaseLocation()
    {
        while (dbManager.isLocationReady == false)
            yield return null;

        // Set locations
        List<POI> locations = dbManager.locations;

        // Get todays location
        int day = Convert.ToInt32((DateTime.Today - new DateTime(2000, 1, 1)).TotalDays);
        System.Random rnd = new System.Random(day);
        int todaysPOI = rnd.Next(0, locations.Count);
        location = locations[todaysPOI];

        // Get target location
        Double.TryParse(location.latitude, out double parseLat);
        Double.TryParse(location.longitude, out double parseLong);
        targetLat = parseLat;
        targetLong = parseLong;

        // Setup other stuff
        txtLocationName.text = location.name;
        txtLocationDesc.text = location.description;

        Debug.Log("Firebase location ready");
        isFirebaseLocationsReady = true;
    }

    IEnumerator WaitForFirebaseImage()
    {
        while (dbManager.isLocationReady == false)
            yield return null;

        dbManager.GetPOIImage(location.name + "/" + location.imageName);

        while (dbManager.isImageReady == false)
            yield return null;

        imgLocation.texture = dbManager.image;

        Debug.Log("Firebase image ready");
        isFirebaseImageReady = true;
    }

    IEnumerator WaitForGPS() 
    {
        while (gpsGetLocation.isGPSReady == false)
            yield return null;

        currentLat = gpsGetLocation.userLocationLatitude;
        currentLong = gpsGetLocation.userLocationLongitude;

        Debug.Log("GPS Ready");
        isGPSReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGPSReady || !isFirebaseLocationsReady || isFirebaseImageReady) return;
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
