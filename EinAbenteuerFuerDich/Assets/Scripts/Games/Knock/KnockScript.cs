using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class KnockScript : MonoBehaviour
{
    public static float[] knockSign_1 = new float[4]{ .25f, .5f, .25f, .5f };
    public static float[] knockSign_2 = new float[9]{ .5f, .125f, .125f, .125f, .5f, .5f, .25f, .25f, .25f };

    public Text text;

    public bool isExample;
    public float thresh_time;
    public float thresh_sound;

    public AudioClip sign1, sign2;

    private bool inRange;

    private void Start()
    {
        if (isExample) return;
        StartCoroutine(PlayKnockGame());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 14/*Touch*/)
        {
            if (!other.GetComponent<TouchSensor>().tipped) return;
            text.text = "talk";
        }
        else if(!inRange)
        {
            inRange = true;
            StartCoroutine(PlayExample());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer != 14/*Touch*/) inRange = false;
    }

    IEnumerator PlayExample()
    {
        AudioSource aSrc = GetComponent<AudioSource>();
        aSrc.clip = difficulty == 0 ? sign1 : sign2;
        Animator anim = GetComponent<Animator>();
        float[] knockSign = difficulty == 0 ? knockSign_1 : knockSign_2;
        text.text = "start example";

        while(inRange)
        {
            aSrc.Play();
            yield return new WaitForSeconds(5f);
        }

        text.text = "stop example";
        yield break;
    }

    IEnumerator PlayKnockGame()
    {
        StartCoroutine(VoiceScript.FetchAmpOnly(freq: 32000));
        runGame = true;
        float[] knockSign = difficulty == 0 ? knockSign_1 : knockSign_2;
        bool correctTiming = false;
        int counter = 0;
        float scale;
        while (runGame)
        {
            yield return new WaitUntil(()=> (Input.touchCount > 0 && VoiceScript.Signal.amplitude > thresh_sound) || !runGame);
            if (!runGame) break;

            //Nehme das klopfsignal auf:
            counter = 0;
            scale = 5f;
            foreach(float pause in knockSign)
            {
                yield return new WaitUntil(() => !(Input.touchCount > 0 && VoiceScript.Signal.amplitude > thresh_sound));
                text.text = "richtig: " + ++counter;
                correctTiming = false;
                for(float count = 0; count < pause + thresh_time + .2f && !(Input.touchCount > 0 && VoiceScript.Signal.amplitude > thresh_sound); count += Time.fixedDeltaTime)
                {
                    correctTiming = Mathf.Abs(count - pause) < thresh_time;
                    yield return new WaitForFixedUpdate();
                }
                if (!correctTiming) break;
                else { scale -= .25f; Camera.main.orthographicSize = scale; }
            }
            if (correctTiming) break;
            Camera.main.orthographicSize = 5;
        }
        Camera.main.orthographicSize = 5;

        if (correctTiming)
        {
            text.text = "You win!";
        }
        else
        {
            text.text = "You lose!";
        }

        yield break;
    }
}
