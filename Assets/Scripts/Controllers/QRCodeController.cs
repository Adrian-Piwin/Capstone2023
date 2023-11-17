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
    public bool isScanning = false;

    // This is the text we are looking for to validate scan
    private string targetValue;

    private void Start()
    {
        DBService dbContext = new DBService();

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

        float cameraWidth = Screen.width * 0.6f; // Adjust the width as a percentage of the screen width (e.g., 60%)
        float cameraHeight = cameraWidth * (webcamTexture.height / (float)webcamTexture.width); // Maintain aspect ratio
        float xPosition = (Screen.width - cameraWidth) / 2; // Center horizontally
        float yPosition = (Screen.height - cameraHeight) * 0.3f; // Adjust the Y position to be lower (e.g., 30% from the top)

        screenRect = new Rect(xPosition, yPosition, cameraWidth, cameraHeight);
    }

    public void StartQRCodeScanner()
    {
        // Start the camera
        webcamTexture = new WebCamTexture();
        webcamTexture.Play();

        // Begin scanning
        isScanning = true;
    }

    public void StopQRCodeScanner()
    {
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }

        isScanning = false;
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
        if (!isScanning) return; 

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
