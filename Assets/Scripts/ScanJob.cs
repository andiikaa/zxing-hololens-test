using System.Threading.Tasks;
using UnityEngine;
using ZXing;

public class ScanJob
{
    private readonly int textureWidth;
    private readonly int textureHeight;
    private byte[] imageBufferArray;

    //dont know how unity/uwp handles async stuff
    //in case they will execute it in a different thread, 
    //we mark this as volatile so the main thread will see the current data
    private volatile bool _isFinished;
    private volatile Result _scanResult;

    public Result ScanResult { get { return _scanResult; } }
    public bool IsFinished { get { return _isFinished; } }
    public bool IsDataReady { get { return _scanResult != null; } }

    public ScanJob(byte[] imageBufferArray, int textureWidth, int textureHeight)
    {
        this.imageBufferArray = imageBufferArray;
        this.textureWidth = textureWidth;
        this.textureHeight = textureHeight;

        _isFinished = false;
    }

    public async void ScanAsync()
    {
        await Task.Run(() => InternalScan());
    }

    private void InternalScan()
    {
        // create a barcode reader instance
        IBarcodeReader reader = new BarcodeReader();
        reader.Options.TryHarder = true;

        //detect and decode the barcode inside the Color32 array
        _scanResult = reader.Decode(imageBufferArray, textureWidth, textureHeight, RGBLuminanceSource.BitmapFormat.BGR32);

        _isFinished = true;
        //cleanup as fast as possible to let the GC do its job
        imageBufferArray = null;
    }
}
