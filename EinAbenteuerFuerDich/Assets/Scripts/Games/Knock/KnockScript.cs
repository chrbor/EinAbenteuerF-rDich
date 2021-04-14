using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static GameManager;
using static BubbleScript;
using static MenuScript;

public class KnockScript : MonoBehaviour
{
    public float[] knockSign_1 = new float[4]{ .25f, .5f, .25f, .5f };
    public float[] knockSign_2 = new float[9]{ .5f, .125f, .125f, .125f, .5f, .5f, .5f, .25f, .25f };

    public bool isExample;
    public float thresh_time;

    public AudioClip sign1, sign2;
    public Material transition;
    public Material transition_next;

    public GameObject nextSeq;

    private bool inRange;

    private void Start()
    {
        if (isExample) return;
        StartCoroutine(PlaySequence());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 14/*Touch*/)
        {
            if (!other.GetComponent<TouchSensor>().tipped) return;
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

        while(inRange)
        {
            aSrc.Play();
            yield return new WaitForSeconds(5f);
        }

        yield break;
    }

    IEnumerator PlayKnockGame()
    {
        //StartCoroutine(VoiceScript.FetchAmpOnly(freq: 32000, sampleRate: 0.02f, fraction: 8));
        runGame = true;
        float[] knockSign = difficulty == 0 ? knockSign_1 : knockSign_2;
        bool correctTiming = false;
        float timeCount;
        float timeSum;
        float scale;
        while (runGame)
        {
            //VoiceScript.Signal.amplitude = 0;
            yield return new WaitUntil(()=> (Input.touchCount > 0 /*&& VoiceScript.Signal.amplitude > thresh_sound*/) || !runGame);
            if (!runGame) break;

            //Nehme das klopfsignal auf:
            scale = 5f;
            timeSum = 0;
            timeCount = 0;
            foreach(float pause in knockSign)
            {
                timeSum += pause;
                for(; timeCount < timeSum + thresh_time + .2f && Input.touchCount > 0; timeCount += Time.fixedDeltaTime)//warte auf release
                    yield return new WaitForFixedUpdate();

                //VoiceScript.Signal.amplitude = 0;
                correctTiming = false;
                for(; timeCount < timeSum + thresh_time + .2f && !(Input.touchCount > 0 /*&& VoiceScript.Signal.amplitude > thresh_sound*/); timeCount += Time.fixedDeltaTime)
                {
                    yield return new WaitForFixedUpdate();
                    correctTiming = Mathf.Abs(timeCount - timeSum) < thresh_time;
                }
                if (!correctTiming) break;
                else
                {
                    scale -= .25f;
                    Camera.main.orthographicSize = scale;
                    Camera.main.transform.Rotate(Vector3.forward, 5);
                }
            }
            if (correctTiming) break;
            StartCoroutine(ReplyKnock(KnockType.wrongKnock));
            Camera.main.orthographicSize = 5;
            Camera.main.transform.eulerAngles = Vector3.forward * 90;
            yield return new WaitForSeconds(1f);
        }
        Camera.main.orthographicSize = 5;
        Camera.main.transform.eulerAngles = Vector3.forward * 90;

        if (correctTiming)
        {
            StartCoroutine(ReplyKnock(KnockType.correctKnock));
            LoadSave.LoadProgress();
            if(progress.millState % 3 == 0) progress.millState++;
            LoadSave.SaveProgress();
            yield return new WaitForSeconds(1);
            menu.SetTransition(transition);
            menu.DoTransition(1, 1, false);
            Mat_Intro = transition_next;
            yield return new WaitForSeconds(2.4f);
            DontDestroyOnLoad(Instantiate(nextSeq, Vector3.zero, Quaternion.identity));
            SceneManager.LoadScene("World");
        }

        yield break;
    }
    
    enum KnockType { wrongKnock, correctKnock}

    IEnumerator ReplyKnock(KnockType type)
    {
        Animator anim = GetComponent<Animator>();
        anim.Play("Shake");
        switch (type)
        {
            case KnockType.wrongKnock:
                switch (Random.Range(0, 4))
                {
                    case 0: speachBubble.Say("Faaalsch!", Bubbles.Shouting, fontSize: 35, speed: .2f, autoPlay: true); break;
                    case 1: speachBubble.Say("Nope!", Bubbles.Shouting, fontSize: 35, speed: .2f, autoPlay: true); break;
                    case 2: speachBubble.Say("Eine Milli-\nsekunde zu\nlangsam!", Bubbles.Shouting, fontSize: 30, speed: .2f, autoPlay: true); break;
                    case 3: speachBubble.Say("Nicht ganz\nrichtig!", Bubbles.Shouting, fontSize: 35, speed: .2f, autoPlay: true); break;
                }             
                break;
            case KnockType.correctKnock:
                speachBubble.Say("Richtig!", Bubbles.Normal, fontSize: 35, speed: .2f, autoPlay: true, target: gameObject);
                break;
        }
        yield return new WaitForSeconds(1);
        anim.Play("Empty");
        yield break;
    }

    IEnumerator PlaySequence()
    {
        //Weise alles zu:
        player = transform.GetChild(0).gameObject;
        companion = transform.GetChild(1).gameObject;
        player.transform.parent = null;
        companion.transform.parent = null;
        Animator anim_player = player.transform.GetChild(0).GetComponent<Animator>();
        Animator anim_comp = companion.transform.GetChild(0).GetComponent<Animator>();
        Animator anim = GetComponent<Animator>();
        canvas = GameObject.Find("Canvas").gameObject;
        GameObject normBubble = canvas.transform.GetChild(0).GetChild(0).gameObject;
        GameObject normText = normBubble.transform.GetChild(0).GetChild(1).gameObject;
        normBubble.transform.localScale = new Vector3(-1, 1, 1);
        normText.transform.localScale   = new Vector3(-1, 1, 1);

        //Spieler und Companion kommen zur Tür
        StartCoroutine(MoveObj(player, player.transform.position.y + 10));
        yield return new WaitForSeconds(.25f);
        StartCoroutine(MoveObj(companion, companion.transform.position.y + 10));
        
        yield return new WaitForSeconds(3);

        if(progress.millState % 3 == 1)
        {
            anim_comp.SetBool("moving", true);
            speachBubble.Say("Hm,\nniemand da", Bubbles.Normal);
            speachBubble.Say("Ob schon alle bei\nder Mühle sind?", Bubbles.Normal);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);
            yield return new WaitForSeconds(1);
            anim_comp.SetBool("moving", false);
            menu.SetTransition(transition);
            menu.DoTransition(1, 1, false);
            Mat_Intro = transition_next;
            player_startPos = new Vector2(-175, -2);
            yield return new WaitForSeconds(2.4f);
            SceneManager.LoadScene("World");
            yield break;
        }

        anim.Play("Shake");
        speachBubble.Say("WER DA?", Bubbles.Shouting, fontSize: 35, speed: .5f);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim.Play("Empty");

        yield return new WaitForSeconds(1);
        anim_comp.SetBool("moving", true);
        speachBubble.Say("Ist das die\nHandwerker-\nGuilde?", Bubbles.Normal);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        switch(progress.millState % 3)
        {
            case 0:
            speachBubble.Say("Die Mühle liegt\nin Trümmern und wir\nbrauchen eure Hilfe sie\nwieder aufzbauen", Bubbles.Normal);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);

            if(progress.millState == 3)
            {
                yield return new WaitForSeconds(1);
                anim.Play("Shake");
                speachBubble.Say("Schon wieder?", Bubbles.Shouting, fontSize: 30, speed: .5f);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);
                anim.Play("Empty");
            }
                break;
            case 2:
            speachBubble.Say("Wir wollten\nnur Hallo sagen.", Bubbles.Normal);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);
                break;
        }

        anim_comp.SetBool("moving", false);

        anim_player.Play("Squeesh");
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(2);

        anim.Play("Shake");
        speachBubble.Say("OHNE KLOPFZEICHEN\nKOMMT KEINER\nREIN!", Bubbles.Shouting, fontSize: 30, speed: 1);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim.Play("Empty");

        if (progress.millState % 3 == 0)
        {
            anim_comp.Play("lookAngry");
            yield return new WaitForSeconds(1);
            speachBubble.Say("Aber wir brauchen\neure Hilfe...", Bubbles.Normal);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);

            anim.Play("Shake");
            speachBubble.Say("Kein Klopfzeichen, \nkeine Hilfe!", Bubbles.Shouting, fontSize: 30, speed: 1);
            speachBubble.Say("Sorry!\nDie Regeln\nsind da strikt.", Bubbles.Shouting, fontSize: 30, speed: 1);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);
            anim.Play("Empty");
        }


        anim_comp.Play("Squeesh");
        yield return new WaitForSeconds(1);
        anim_player.Play("lookNormal");
        speachBubble.Say("Anscheinend kommen wir\nohne das geheime\nKlopfzeichen nicht weiter", Bubbles.Normal);
        speachBubble.Say("Ich frage mich,\nob einer der Handwerker\nuns das Passwort\nverraten könnte...", Bubbles.Normal);
        speachBubble.Say("Da fällt mir ein...\nBaut die Guilde zurzeit\nnicht auf der anderen Seite\nder Stadt ein\nneues Haus?", Bubbles.Normal);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("Squeesh");
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(1);

        StartCoroutine(PlayKnockGame());
        yield break;
    }

    IEnumerator MoveObj(GameObject obj, float y_endpos)
    {
        Animator anim = obj.transform.GetChild(0).GetComponent<Animator>();
        anim.SetBool("moving", true);
        anim.SetTrigger("startMove");

        Vector3 step = Mathf.Sign(y_endpos - obj.transform.position.y) * 5f * Time.fixedDeltaTime * Vector3.up;
        while(Mathf.Abs(obj.transform.position.y - y_endpos) > Mathf.Abs(step.y))
        {
            obj.transform.position += step;
            yield return new WaitForFixedUpdate();
        }
        anim.SetBool("moving", false);
        yield break;
    }
}
