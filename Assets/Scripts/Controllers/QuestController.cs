using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestController : MonoBehaviour
{
    public QRCodeController qrCodeController;

    public GameObject poiView;
    public GameObject mapView;
    public GameObject leaderboardView;

    public RawImage mapImage;
    public RawImage poiImage;

    public TextMeshProUGUI poiName;
    public TextMeshProUGUI poiDesc;

    private POIProcesses poiProcesses;
    private PlayerProcesses playerProcesses;

    // Start is called before the first frame update
    async void Start()
    {

        PlayerPrefs.SetString("lobbyCode", "12345");
        PlayerPrefs.SetInt("campusID", 10);
        PlayerPrefs.SetInt("playerID", 6);

        // Start on map view
        toggleView("map");

        // Get context of code
        string lobbyID = PlayerPrefs.GetString("lobbyCode", "").ToString();
        string campusID = PlayerPrefs.GetInt("campusID", -1).ToString();
        string playerID = PlayerPrefs.GetInt("playerID", -1).ToString();

        DBService dbContext = new DBService();
        FirebaseService fbContext = new FirebaseService();

        poiProcesses = new POIProcesses(fbContext, dbContext, campusID);
        playerProcesses = new PlayerProcesses(dbContext, lobbyID);

        Player player = playerProcesses.getPlayer(playerID);
        POI poi = poiProcesses.getPOI(player.status.ToString());
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

        if (qrCodeController.isScanning) 
        {
            qrCodeController.StopQRCodeScanner();
        }
    }

    public void toggleQRScanner()
    {
        mapView.SetActive(false);
        poiView.SetActive(false);
        leaderboardView.SetActive(false);

        qrCodeController.StartQRCodeScanner();
    }
}
