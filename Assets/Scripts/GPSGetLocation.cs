using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GPSGetLocation : MonoBehaviour
{
    public bool isActive;
    public bool isUnityRemote;
    public float gpsUpdateInterval;
    public double userLocationLongitude;
    public double userLocationLatitude;


    // Start is called before the first frame update
   public void Start()
    {
        isActive = false;
        StartCoroutine(GPSLoc());
    }

    IEnumerator GPSLoc()
    {
        if (isUnityRemote)
        {
            yield return new WaitForSeconds(1);
        }

        // Check if location is enabled
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location services not enabled");
            yield break;
        }

        // Start service
        Input.location.Start();

        // Wait time for service to start
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        
        // Service timed out
        if (maxWait < 1)
        {
            Debug.Log("Time out");
            yield break;
        }

        // Connection failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Device Location not found");

            yield break;
        }
        // Success
        else
        {
            Debug.Log("Success");
            InvokeRepeating("UpdateGPSData", gpsUpdateInterval, 1f);
        }
    }//end of gpsloc

   private void UpdateGPSData()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            isActive = true;
            userLocationLatitude = Input.location.lastData.latitude;
            userLocationLongitude = Input.location.lastData.longitude;
        }
        else
        {
            // Service stopped
            Debug.Log("Service Stopped");
        }
    }
}
