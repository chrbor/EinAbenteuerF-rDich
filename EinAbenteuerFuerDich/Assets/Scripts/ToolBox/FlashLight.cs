using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

//Lib:
//https://developer.android.com/reference/android/hardware/camera2/package-summary
//Implementierung (depreciated):
//https://stackoverflow.com/questions/25848519/how-turn-on-off-android-flashlight-using-c-sharp-only-in-unity3d
//Implementierung (current):
//https://stackoverflow.com/questions/6068803/how-to-turn-on-front-flash-light-programmatically-in-android
public class FlashLight
{
    public AndroidJavaClass unityPlayer;
    public AndroidJavaObject currentActivity;
    private AndroidJavaObject camManager;

    private string flashCamName;

    public FlashLight()
    {
        //camManager = new AndroidJavaClass("android.hardware.camera2.CameraManager");
#if UNITY_ANDROID
        unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        camManager = currentActivity.Call<AndroidJavaObject>("getSystemService", "CameraManager");

        string camName = "";
        foreach (var device in WebCamTexture.devices)
        {
            AndroidJavaObject camchars = camManager.Call<AndroidJavaObject>("getCameraCharacteristics", device.name);
            if (camchars.GetStatic<bool>("FLASH_INFO_AVAILABLE")) { camName = device.name; break; }
        }
        if (camName == "") { return; } 
        flashCamName = camName;
#endif
    }

    ~FlashLight()
    {
        SetFlashlight(false);
    }

    public void SetFlashlight(bool active)
    {
        if (flashCamName == null) return;

        camManager.Call("SetTorchMode", flashCamName, active);
    }
}
