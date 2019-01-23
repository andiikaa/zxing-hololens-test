using UnityEngine;
using UnityEngine.XR.WSA.WebCam;


public class ZXingCode : MonoBehaviour {

    PhotoCapture photoCaptureObject = null;
    Resolution cameraResolution;

    //creating 2D Texture eats memory, so we use only one and update the content after each PhotoCapture
    Texture2D targetTexture;

    ScanJob scanJob;

    bool captureStarted = false;
    bool firstScan = true;

    // Use this for initialization
    void Start () {
        Debug.Log("Starting the scanning script");

        //the resolution is recommended for hololens by microsoft
        cameraResolution = new Resolution() { width = 1280, height = 720 };

        targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height, TextureFormat.BGRA32, false);
        Debug.LogFormat("Take picture with w:{0} x h:{1}", cameraResolution.width, cameraResolution.height);
    }

	// Update is called once per frame
	void Update ()
    {
        //there can only be one active PhotoCapture
        if (!captureStarted)
        {
            captureStarted = true;
            StartInit();
        }

        //- firstScan indicates, if an image was already captured
        //- there is one ScanJob which uses the captured image and tries to decode the barcode
        //- we do this async or the framerate will drop to 2-3 fps :(
        if (!firstScan)
        {
            if(scanJob == null || scanJob.IsFinished)
            {
                if (scanJob != null && scanJob.IsDataReady)
                {
                    //do whatever you want with the data
                    Debug.Log("#### data is ready ####");
                }


                lock (targetTexture)
                {
                    scanJob = new ScanJob(targetTexture.GetPixels32(), cameraResolution.width, cameraResolution.height);
                }

                scanJob.ScanAsync();
            }
        }
    }

    void StartInit()
    {
        // Create a PhotoCapture object
        PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject) {
            photoCaptureObject = captureObject;
            CameraParameters cameraParameters = new CameraParameters();
            cameraParameters.hologramOpacity = 0.0f;
            cameraParameters.cameraResolutionWidth = cameraResolution.width;
            cameraParameters.cameraResolutionHeight = cameraResolution.height;
            cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

            // Activate the camera
            photoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate (PhotoCapture.PhotoCaptureResult result) {
                // Take a picture
                photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
            });
        });
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        // Copy the raw image data into the target texture
        lock(targetTexture)
        {
            photoCaptureFrame.UploadImageDataToTexture(targetTexture);
            firstScan = false;
        }

        // Deactivate the camera
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        // Shutdown the photo capture resource
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
        captureStarted = false;
    }
}
