using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;
using static CameraScript;

public class SignScript : MonoBehaviour
{
    public enum Type { Normal, Info, Direction}
    public Type type;
    [TextArea(5, 7)]
    public string text_1;
    [TextArea(5, 7)]
    public string text_2;

    public Sprite sprite;
    private bool reading;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.GetComponent<TouchSensor>().tipped || runGame || pauseGame || reading) return;
        pauseMove = true;
        staticCam = true;
        runGame = true;
        reading = true;
        StartCoroutine(cScript.SetRotation());
        StartCoroutine(ReadSign());
    }

    IEnumerator ReadSign()
    {
        Transform signs = canvas.transform.GetChild(canvas.transform.childCount - 2);

        GameObject sign = null;
        switch (type)
        {
            case Type.Normal:
                sign = signs.GetChild(0).gameObject;
                sign.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = text_1;
                if (sprite != null)
                {
                    sign.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = sprite;
                    sign.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                }
                else sign.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                sign.transform.GetChild(0).GetComponent<VerticalLayoutGroup>().enabled = false;
                Canvas.ForceUpdateCanvases();
                sign.transform.GetChild(0).GetComponent<VerticalLayoutGroup>().enabled = true;
                break;

            case Type.Direction:
                sign = signs.GetChild(1).gameObject;
                sign.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = text_1;
                sign.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = text_2;
                break;

            case Type.Info:
                sign = signs.GetChild(2).gameObject;
                sign.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = text_1;
                if(sprite != null)
                {
                    sign.transform.GetChild(1).GetComponent<Image>().sprite = sprite;
                    sign.transform.GetChild(1).gameObject.SetActive(true);
                }
                else sign.transform.GetChild(1).gameObject.SetActive(false);
                break;
        }


        sign.SetActive(true);
        Animator anim = sign.GetComponent<Animator>();
        anim.Play("ZoomIn");
        yield return new WaitForSeconds(.5f);

        yield return new WaitUntil(() => Input.touchCount == 0);
        yield return new WaitUntil(() => Input.touchCount != 0);
        yield return new WaitUntil(() => Input.touchCount == 0);
        anim.Play("ZoomOut");
        yield return new WaitForSeconds(.5f);
        sign.SetActive(false);
        pauseMove = false;
        staticCam = false;
        runGame = false;
        reading = false;
        yield break;
    }

}
