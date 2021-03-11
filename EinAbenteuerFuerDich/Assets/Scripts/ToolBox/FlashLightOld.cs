using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLightOld
{
    AndroidJavaObject camera = null;

    public FlashLightOld()
    {
        string camName = "";
        foreach (var device in WebCamTexture.devices) if (device.isFrontFacing == false) { camName = device.name; break; }
        Debug.Log("Camera Name:" + camName);

        Open();
    }

    ~FlashLightOld()
    {
        Release();
    }

    public void Open()
    {
        if (camera == null)
        {
            #if (UNITY_ANDROID)
			    AndroidJavaClass cameraClass = new AndroidJavaClass("android.hardware.Camera");
			    camera = cameraClass.CallStatic<AndroidJavaObject>("open");
            #endif
        }
    }

    public void Release()
    {
        if (camera != null)
        {
            LEDOff();

            camera.Call("release");
            camera = null;
        }
    }

    public void StartPreview()
    {
        if (camera != null)
        {
            Debug.Log("AndroidCamera::startPreview()");
            camera.Call("startPreview");
        }
    }

    public void StopPreview()
    {
        if (camera != null)
        {
            Debug.Log("AndroidCamera::stopPreview()");
            LEDOff();
            camera.Call("stopPreview");
        }
    }

    private void SetFlashMode(string mode)
    {
        if (camera != null)
        {
            AndroidJavaObject cameraParameters = camera.Call<AndroidJavaObject>("getParameters");
            cameraParameters.Call("setFlashMode", mode);
            camera.Call("setParameters", cameraParameters);
        }
    }

    public void LEDOn()
    {
        StartPreview();
        SetFlashMode("torch");
    }

    public void LEDOff()
    {
        SetFlashMode("off");
    }
}
