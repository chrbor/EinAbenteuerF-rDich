using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;
using static MenuScript;

public class BubbleScript : MonoBehaviour
{
    public enum Bubbles { Normal, Shouting}
    public static BubbleScript speachBubble;
    private int textBeforeCall;
    private int activeText;

    private Animator anim;
    private static Text text;
    private static GameObject bubble;
    [HideInInspector]
    public bool finished;


    bool clear { get; set; }
    public bool Clear { get { return clear; } set { clear = value; if (clear) { textBeforeCall = 0; activeText = 0; } } }

    bool turnOff = false;

    // Start is called before the first frame update
    void Start()
    {
        speachBubble = this;
    }

    public void Say(string textToSay, Bubbles bubbleType = Bubbles.Normal, float speed = -1, int fontSize = 25, GameObject target = null, bool autoPlay = false) => StartCoroutine(PlayText(textToSay, bubbleType, speed, fontSize, target, autoPlay));

    private IEnumerator PlayText(string textToSay, Bubbles bubbleType, float textSpeed, int fontSize, GameObject target, bool autoPlay) 
    {
        int textNumber = textBeforeCall;
        textBeforeCall += 1;

        yield return new WaitUntil(() => (textNumber == activeText && !turnOff) || clear);
        if (clear) { yield return new WaitForFixedUpdate(); clear = false; yield break; }

        if(textNumber == 0)
        {
            finished = false;
            switch (bubbleType)
            {
                case Bubbles.Normal:
                    bubble = transform.GetChild(0).GetChild(0).gameObject;
                    bubble.SetActive(true);

                    GameObject pointer = bubble.transform.GetChild(0).gameObject;
                    if(target == null)
                    {
                        //Zeige ins off:
                        pointer.GetComponent<RectTransform>().anchoredPosition = new Vector2(-180, -25);
                        pointer.transform.localRotation = Quaternion.Euler(0, 0, 110);  
                    }
                    else
                    {
                        //zeige auf target:
                        pointer.GetComponent<RectTransform>().anchoredPosition = Vector2.up * 90;//halte y-position
                        float diff_x = target.transform.position.x - pointer.transform.position.x;
                        pointer.transform.localPosition = new Vector3(diff_x * Camera.main.pixelWidth / (Camera.main.orthographicSize * 12), pointer.transform.localPosition.y);
                        Vector2 diff = target.transform.position - pointer.transform.position;

                        pointer.transform.eulerAngles = new Vector3(0,0, Mathf.Rad2Deg * Mathf.Atan2(-diff.x, 3));
                    }

                    anim = bubble.GetComponent<Animator>();
                    anim.Play("BlobIn");
                    break;
                case Bubbles.Shouting:
                    bubble = transform.GetChild(1).GetChild(0).gameObject;
                    bubble.SetActive(true);
                    anim = bubble.GetComponent<Animator>();
                    anim.Play("Shake");
                    break;
            }
        }

        bubble.SetActive(true);
        text = bubble.transform.GetChild(bubble.transform.childCount - 1).GetComponent<Text>();
        text.text = "";
        text.fontSize = fontSize;   

        float realSpeed = textSpeed > 0 ? textSpeed / textToSay.Length : Time.fixedDeltaTime;
        int i = 0;
        while(i < textToSay.Length && ((Input.touchCount == 0 && !Input.GetMouseButton(1) && !menu.decisionMade) || autoPlay))
        {
            text.text += textToSay[i++];
            yield return new WaitForSeconds(realSpeed);
        }
        for (int j = i; j < textToSay.Length; j++) text.text += textToSay[j];
        if (i != textToSay.Length) yield return new WaitUntil(() => Input.touchCount == 0 && !Input.GetMouseButton(1));

        yield return new WaitUntil(() => ((Input.touchCount > 0 || Input.GetMouseButton(1) || autoPlay) && !pauseGame) || clear || menu.decisionMade);
        if (clear) { yield return new WaitForFixedUpdate(); clear = false; textNumber = -1; }
        else if (autoPlay) yield return new WaitForSeconds(textSpeed);
        else yield return new WaitUntil(() => Input.touchCount == 0 && !Input.GetMouseButton(1));

        if (textNumber + 1 == textBeforeCall)
        {
            textBeforeCall = 0;
            activeText = 0;
            turnOff = true;
            anim.Play("BlobOut");
            yield return new WaitForSeconds(.5f);
            bubble.SetActive(false);
            finished = true;
            turnOff = false;
        }
        else activeText++;
        yield break;
    }
}
