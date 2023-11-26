using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;
using TMPro;

public class QRCodeController : MonoBehaviour
{
    public QuestController questController;

    private WebCamTexture webcamTexture;
    private Rect screenRect;
    public bool isScanning = false;

    // This is the text we are looking for to validate scan
    public string targetValue;

    private void Start()
    {
        screenRect = new Rect(410, 0, Screen.width, Screen.height);
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
                        questController.validScan();
                    }
                }
            }
        }
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
