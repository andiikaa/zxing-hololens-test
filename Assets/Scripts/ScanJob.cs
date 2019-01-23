
using System;
using System.Threading.Tasks;
using UnityEngine;
using ZXing;

public class ScanJob
{
    private Color32[] barcodeBitmap;
    private readonly int textureWidth;
    private readonly int textureHeight;

    public Result ScanResult { get; private set; }
    public bool IsFinished { get; private set; }
    public bool IsDataReady { get { return ScanResult != null; } }

    public ScanJob(Color32[] barcodeBitmap, int textureWidth, int textureHeight)
    {
        if (barcodeBitmap == null)
        {
            throw new ArgumentNullException("texture cant be null");
        }

        this.barcodeBitmap = barcodeBitmap;
        this.textureWidth = textureWidth;
        this.textureHeight = textureHeight;

        IsFinished = false;
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
        ScanResult = reader.Decode(barcodeBitmap, textureWidth, textureHeight);

        // do something with the result
        if (ScanResult != null)
        {
            Debug.Log(ScanResult.BarcodeFormat.ToString());
            Debug.Log(ScanResult.Text);
        }

        IsFinished = true;
        barcodeBitmap = null;
    }
}
