using UnityEngine;

public static class TouchAndScreen
{
    /*
    //Spiel läuft immer in Landscape-Modus, daher ist die Bildschirmhöhe immer referenz
    public static Vector2 PixelToWorld(Vector3 pixelPos) => (Vector2)Camera.main.transform.position
        - Camera.main.orthographicSize * new Vector2(Camera.main.aspect, 1)
        + 2 * Camera.main.orthographicSize * new Vector2(Camera.main.aspect * pixelPos.x/Camera.main.pixelWidth, pixelPos.y/Camera.main.pixelHeight);
        */

    public static Vector2 PixelToWorld(Vector3 pixelPos)
    {
        float rot = Camera.main.transform.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 oPos = 2 * Camera.main.orthographicSize * new Vector2(Camera.main.aspect * (pixelPos.x / Camera.main.pixelWidth -.5f), pixelPos.y / Camera.main.pixelHeight -.5f);
        return (Vector2)Camera.main.transform.position + new Vector2(oPos.x * Mathf.Cos(rot) - oPos.y * Mathf.Sin(rot), 
                                                                     oPos.y * Mathf.Cos(rot) + oPos.x * Mathf.Sin(rot));
    }
}
