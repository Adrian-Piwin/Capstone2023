using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestController : MonoBehaviour
{
    public GameObject poiView;
    public GameObject mapView;
    public GameObject leaderboardView;

    public RawImage mapImage;
    public RawImage poiImage;

    public TextMeshProUGUI poiName;
    public TextMeshProUGUI poiDesc;

    private PlayerProcesses playerProcesses;
    private CampusProcesses campusProcesses;
    private POIProcesses poiProcesses;

    // Start is called before the first frame update
    async void Start()
    {
        // Start on map view
        toggleView("map");

        // Get context of code
        string code = PlayerPrefs.GetString("lobbyCode", "");

        DBService dbContext = new DBService();
        FirebaseService fbContext = new FirebaseService();

        playerProcesses = new PlayerProcesses(dbContext, code);
        campusProcesses = new CampusProcesses(dbContext, code);

        Campus campus = campusProcesses.getCampus();
        poiProcesses = new POIProcesses(fbContext, dbContext, campus.id.ToString());

        POI poi = poiProcesses.getPOI();
        loadImage(mapImage, await poiProcesses.getImage(poi.id.ToString(), "map", poi.map));
        loadImage(poiImage, await poiProcesses.getImage(poi.id.ToString(), "img", poi.image));

        poiName.text = poi.name;
        poiDesc.text = poi.description;
    }

    private void loadImage(RawImage dest, byte[] img)
    {
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(img);
        dest.texture = texture;
    }

    public void toggleView(string view)
    {
        mapView.SetActive(view == "map");
        poiView.SetActive(view == "poi");
        leaderboardView.SetActive(view == "leaderboard");
    }
}
