using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccelScript : MonoBehaviour
{
    public Text text;

    private bool vibrating;
    List<long[]> vib_waves = new List<long[]>();
    Vibration vib;

    private void Start()
    {
        vib = new Vibration();
        Input.gyro.enabled = true;

        vib_waves.Add(new long[2] { 1, 1 });
        vib_waves.Add(new long[2] { 1, 5 });
        vib_waves.Add(new long[50] { 1, 1, 1, 1, 1, 1, 1, 2, 1, 2, 1, 2, 1, 3, 1, 4, 1, 5, 1, 6, 1, 7, 1, 8, 1, 9, 1, 10, 1, 11, 1, 12, 1, 13, 1, 14, 1, 15, 1, 16, 1, 17, 1, 18, 1, 19, 1, 20, 1, 20 });
        vib_waves.Add(new long[92] { 1, 1, 1, 1, 1, 1, 1, 2, 1, 2, 1, 2, 1, 3, 1, 4, 1, 5, 1, 6, 1, 7, 1, 8, 1, 9, 1, 10, 1, 11, 1, 12, 1, 13, 1, 14, 1, 15, 1, 16, 1, 17, 1, 18, 1, 19, 1, 20, 1, 20,
                                      1, 20, 1, 19, 1, 18, 1, 17, 1, 16, 1, 15, 1, 14, 1, 13, 1, 12, 1, 11, 1, 10, 1, 9, 1, 8, 1, 7, 1, 6, 1, 5, 1, 4, 1, 3, 1, 2, 1, 2, 1, 2});
        vib.SetVibrationEffect(vib_waves[0], 0);
        vib.SetVibrationEffect(vib_waves[1], 0);
        vib.SetVibrationEffect(vib_waves[2], 48);
        vib.SetVibrationEffect(vib_waves[3], 0);
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "";
        if (SystemInfo.supportsAccelerometer)
            text.text += "accel: " + Input.acceleration;
        else text.text += "accel: not supported";
        text.text += "\n";
        if (SystemInfo.supportsGyroscope)
            text.text += "gyro: " + Input.gyro.rotationRate;//new Vector3(Input.gyro.rotationRate.x * 1e6f, Input.gyro.rotationRate.y * 1e6f, Input.gyro.rotationRate.z * 1e6f);
        else text.text += "gyro: not supported";

        //CheckVibration();
    }

    public void Vibrate()
    {
        vib.Vibrate(0);
    }

    public void Click()
    {
        vib.Vibrate(1);
    }

    public void DoubleClick()
    {
        vib.Vibrate(2);
    }

    public void HeavyClick()
    {
        vib.Vibrate(3);
    }

    public void Tick()
    {
        vib.Vibrate(4);
    }

    public void StopVibration()
    {
        vib.Cancel();
    }
    /*
    public void SetFlashLight(bool active)
    {
        flashlight.SetFlashlight(active);
    }
    */
}
