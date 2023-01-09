using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class NavigationManager : MonoBehaviour
{
    public List<GameObject> btnStartGame;
    public TextMeshProUGUI txtLocationName;

    private DatabaseManager dbManager;

    // Start is called before the first frame update
    void Start()
    {
        dbManager = GetComponent<DatabaseManager>();
        StartCoroutine(dbManager.GetPOILocations((List<POI> locations) =>
        {
            Debug.Log(locations);
        }));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
