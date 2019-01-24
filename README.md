# ZXing HoloLens Test

Sample project which uses [ZXing.Net](https://github.com/micjahn/ZXing.Net) and the [Microsoft HoloLens](https://www.microsoft.com/en-us/hololens) for scanning QRCodes.
This app will scan permantly for codes and print the result to the debug output.

Important while using PhotoCapture: call `PhotoFrame.Dispose()` before taking the next photo. [See here](https://forum.unity.com/threads/accessviolationexception-using-copyrawimagedataintobuffer.497928/) 