/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecCameraScript : MonoBehaviour
{

    static WebCamTexture cam_Texture;
    GameObject test;

    public float colorCamTime;
    public bool colorFound;
    private bool colorSearch;

    private ComputeShader calcShader;
    private ComputeBuffer texBuffer, colorBuffer;
    private float[] res_tex;

    public class HSVColor
    {
        /// <summary>(Farbwert) </summary>
        float hue;
        /// <summary>(Sättigung) </summary>
        float saturation;
        /// <summary>(Helligkeit) </summary>
        float value;

        /// <param name="h">hue(Farbwert)</param>
        /// <param name="s">saturation(Sättigung)</param>
        /// <param name="v">value(Helligkeit)</param>
        public HSVColor(float h, float s, float v)
        {
            hue = h;
            saturation = s;
            value = v;
        }

        public void SetColor(Color color) { Color.RGBToHSV(color, out hue, out saturation, out value); }
        public Color GetColor() { return Color.HSVToRGB(hue, saturation, value); }
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(gameObject);
        test = GameObject.Find("CamTester");
        ActivateCameraTex(test.GetComponent<SpriteRenderer>().material, true);

        //StartCoroutine(FindColor(new HSVColor(0.9f, 1, 1), new HSVColor(0.1f, 0.5f, 0.3f)));
    }


    IEnumerator FindColor(HSVColor minColor, HSVColor maxColor)
    {
        if(colorSearch)
        { colorSearch = false; yield return new WaitForEndOfFrame(); }
        colorSearch = true;
        colorFound = false;

        int meanID = calcShader.FindKernel("Mean");
        colorBuffer = new ComputeBuffer(4, sizeof(float));

        HSVColor currentColor;

        colorCamTime = 0;

        Texture2D tex2d = new Texture2D(1024, 512, TextureFormat.Alpha8, true);


        while (!colorFound && colorSearch)
        {
            calcShader.SetTexture(meanID, "Input", cam_Texture);
            calcShader.SetBuffer(meanID, "Result", texBuffer);
            calcShader.Dispatch(meanID, 1024, 1, 1);

            colorBuffer.GetData(res_tex);
            yield return new WaitForEndOfFrame();
        }

        colorBuffer.Release();
        yield break;
    }

    void ActivateCameraTex(Material mat, bool frontCam)
    {
        string camName = "";
        foreach(var device in WebCamTexture.devices) if (device.isFrontFacing == frontCam) { camName = device.name; break; }
        if(camName != "")
        {
            cam_Texture = new WebCamTexture(camName);
            if (!cam_Texture.isPlaying) cam_Texture.Play();
            mat.SetTexture("_CameraTex", cam_Texture);
        }
    }

    void DisableCameraTex()
    {
        if (cam_Texture.isPlaying)
            cam_Texture.Stop();
    }

    
}
*/