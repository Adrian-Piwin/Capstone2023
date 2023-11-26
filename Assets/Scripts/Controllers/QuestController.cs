using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Web;

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
        // TEMP - DELETE THIS ==================================
        //PlayerPrefs.SetString("lobbyCode", "12345");
        //PlayerPrefs.SetInt("campusID", 10);
        //PlayerPrefs.SetInt("playerID", 37);
        // =====================================================

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

        // See if the game was cleared if started
        checkGameStatus();
        // Setup the scanner to know what QR code to find
        setupScannerTarget(lobbyID, playerID);
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

    // Set the target value for the scanner
    private void setupScannerTarget(string code, string playerID) 
    {
        // Set the target POI text
        Player player = playerProcesses.getPlayer(playerID);
        POI targetPOI = poiProcesses.getPOI((player.status).ToString());

        qrCodeController.targetValue = $"{code}:{targetPOI.name}";
    }

    // Called when the correct QR code is scanned
    public void validScan() 
    {
        PlayerPrefs.SetString("gameStatus", "started");
        SceneUtility.instance.ChangeScene("ARGame");
    }

    private void checkGameStatus() 
    {
        string status = PlayerPrefs.GetString("gameStatus", "");

        if (status == "cleared") 
        {
            // Update player status
            string playerID = PlayerPrefs.GetInt("playerID", -1).ToString();
            Player player = playerProcesses.getPlayer(playerID);
            playerProcesses.updatePlayerStatus(playerID, (player.status + 1).ToString());

            // Check quest complete condition
            int poiTotal = poiProcesses.getAllPOI().Count;
            if (player.status + 1 > poiTotal) 
            {
                // Quest complete, send them to final scene
                playerProcesses.stopPlayersTimer(playerID);
                SceneUtility.instance.ChangeScene("FinalLeaderboard");
            }
        }
    }
}
