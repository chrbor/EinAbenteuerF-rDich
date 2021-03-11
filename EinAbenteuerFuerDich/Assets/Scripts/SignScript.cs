using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class SignScript : MonoBehaviour
{
    public enum Type { Normal, Info, Direction}
    public Type type;
    [TextArea(5, 7)]
    public string text_1;
    [TextArea(5, 7)]
    public string text_2;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.GetComponent<TouchSensor>().tipped) return;
        pauseGame = true;

        GameObject sign = null;
        switch (type)
        {
            case Type.Normal:       sign = GameObject.Find("Canvas").transform.GetChild(0).GetChild(0).gameObject; break;
            case Type.Direction:    sign = GameObject.Find("Canvas").transform.GetChild(0).GetChild(1).gameObject; break;
            case Type.Info:         sign = GameObject.Find("Canvas").transform.GetChild(0).GetChild(2).gameObject; break;
        }
        sign.SetActive(true);
        sign.GetComponent<Animator>().Play("ZoomIn");
    }

}
