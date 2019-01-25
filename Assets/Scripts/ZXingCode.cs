using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.WebCam;


public class ZXingCode : MonoBehaviour
{

    PhotoCapture photoCaptureObject = null;
    Resolution cameraResolution;

    TextMesh text;

    ScanJob scanJob;

    List<byte> imageBuffer = new List<byte>();

    bool captureStarted = false;
    bool firstScan = true;

    // Use this for initialization
    void Start()
    {
        Debug.Log("Starting the scanning script");

        text = gameObject.transform.Find("AppHint").GetComponent<TextMesh>();

        //the resolution is recommended for hololens by microsoft
        cameraResolution = new Resolution() { width = 1280, height = 720 };
        Debug.LogFormat("Take picture with w:{0} x h:{1}", cameraResolution.width, cameraResolution.height);
    }

    // Update is called once per frame
    void Update()
    {
        StartCapture();
        HandlePresentImage();
    }

    void OnDestroy() { Debug.Log("destroying the qrcode scanning object"); }
    void OnDisable() { Debug.Log("disabling the qrcode scanning object"); }

    //- firstScan indicates, if an image was already captured
    //- there is one ScanJob which uses the captured image and tries to decode the barcode
    //- we do this async or the framerate will drop to 2-3 fps :(
    private void HandlePresentImage()
    {
        if (firstScan) return;
        if (scanJob != null && !scanJob.IsFinished) return;

        HandleExistingQRCode();
        TryToReadQRCode();
    }

    private void HandleExistingQRCode()
    {
        if (scanJob == null || !scanJob.IsDataReady) return;

        //do whatever you want with the data
        Debug.Log("#### data is ready ####");
        Debug.Log(scanJob.ScanResult.BarcodeFormat.ToString());
        string tmpString = scanJob.ScanResult.Text;
        Debug.Log(tmpString);
        if (!string.IsNullOrEmpty(tmpString)) text.text = "QR: " + tmpString;
    }

    //tries to read the qrcode async
    private void TryToReadQRCode()
    {
        lock (imageBuffer)
        {
            byte[] tmpBuffer = new byte[imageBuffer.Count];
            imageBuffer.CopyTo(tmpBuffer);
            scanJob = new ScanJob(tmpBuffer, cameraResolution.width, cameraResolution.height);
        }

        scanJob.ScanAsync();
    }

    #region photo capture

    private void StartCapture()
    {
        //there can only be one active PhotoCapture
        if (captureStarted) return;
        captureStarted = true;

        // Create a PhotoCapture object
        PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject)
        {
            photoCaptureObject = captureObject;
            CameraParameters cameraParameters = new CameraParameters();
            cameraParameters.hologramOpacity = 0.0f;
            cameraParameters.cameraResolutionWidth = cameraResolution.width;
            cameraParameters.cameraResolutionHeight = cameraResolution.height;
            cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

            // Activate the camera
            photoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate (PhotoCapture.PhotoCaptureResult result)
            {
                // Take a picture
                photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
            });
        });
    }

    private void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        // Copy the raw image data into the target texture
        lock (imageBuffer)
        {
            imageBuffer.Clear();
            photoCaptureFrame.CopyRawImageDataIntoBuffer(imageBuffer);
            //IMPORTANT: Dispose the capture frame, or the app will crash after a while with access violation 
            photoCaptureFrame.Dispose();
            firstScan = false;
        }

        // Deactivate the camera
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    private void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        // Shutdown the photo capture resource
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
        captureStarted = false;
    }

    #endregion
}
