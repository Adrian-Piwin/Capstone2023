using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;
using TMPro;

public class QRCodeController : MonoBehaviour
{
    private PlayerProcesses playerProcesses;
    private POIProcesses poiProcesses;

    private WebCamTexture webcamTexture;
    private Rect screenRect;
    private bool isScanning = false;

    // This is the text we are looking for to validate scan
    private string targetValue;

    private void Start()
    {
        DBService dbContext = new DBService();

        PlayerPrefs.SetString("lobbyCode", "12345");
        PlayerPrefs.SetInt("campusID", 10);
        PlayerPrefs.SetInt("playerID", 6);

        string code = PlayerPrefs.GetString("lobbyCode", "");
        string campusID = PlayerPrefs.GetInt("campusID", -1).ToString();
        string playerID = PlayerPrefs.GetInt("playerID", -1).ToString();

        poiProcesses = new POIProcesses(dbContext, campusID);
        playerProcesses = new PlayerProcesses(dbContext, code);

        // Set the target POI text
        Player player = playerProcesses.getPlayer(playerID);
        POI targetPOI = poiProcesses.getPOI((player.status + 1).ToString());
        targetValue = $"{code}:{targetPOI.name}";
        Debug.Log(targetValue);
        
        screenRect = new Rect(0, 0, Screen.width, Screen.height);
        StartQRCodeScanner();
    }

    private void StartQRCodeScanner()
    {
        // Start the camera
        webcamTexture = new WebCamTexture();
        webcamTexture.Play();

        // Begin scanning
        isScanning = true;
    }

    private void Update()
    {
        if (isScanning)
        {
            if (webcamTexture != null && webcamTexture.isPlaying)
            {
                // Capture the camera frame
                Color32[] pixels = webcamTexture.GetPixels32();

                // Create a barcode reader
                IBarcodeReader barcodeReader = new BarcodeReader();

                // Decode the QR code
                Result result = barcodeReader.Decode(pixels, webcamTexture.width, webcamTexture.height);

                if (result != null)
                {
                    if (VerifyScan(result.Text))
                    {
                        isScanning = false;
                        ScanValid();
                    }
                }
            }
        }
    }

    private void ScanValid()
    {
        // Update status
        string playerID = PlayerPrefs.GetInt("playerID", -1).ToString();
        Player player = playerProcesses.getPlayer(playerID);
        playerProcesses.updatePlayerStatus(playerID, (player.status + 1).ToString());

        // Go back to quest scene
        MsgUtility.instance.DisplayMsg("Success!", MsgType.Success);
        SceneUtility.instance.ChangeScene("Quest");
    }

    private bool VerifyScan(string value)
    {
        if (value == targetValue)
        {
            return true;
        }
        else
        {
            MsgUtility.instance.DisplayMsg("Incorrect QR code scanned", MsgType.Error);
            return false;
        }
    }

    private void OnGUI()
    {
        // Rotate the camera feed 90 degrees counterclockwise
        Matrix4x4 matrixBackup = GUI.matrix;
        GUIUtility.RotateAroundPivot(90, new Vector2(Screen.width / 2, Screen.height / 2));
        GUI.DrawTexture(screenRect, webcamTexture, ScaleMode.ScaleToFit);
        GUI.matrix = matrixBackup;
    }

    private void OnDestroy()
    {
        // Stop the camera when the script is destroyed
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }
    }
}
