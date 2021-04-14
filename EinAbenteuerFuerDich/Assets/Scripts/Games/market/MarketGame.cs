using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;
using static CameraScript;
using static BubbleScript;
using static MenuScript;

/// <summary>
/// Spielablauf:
/// Spiel startet, indem der Einkaufswagen angetippt wird.
/// 
/// Ziel ist es 15 Sachen zu fangen.
/// Es dürfen nicht mehr als drei Sachen auf den Boden kommen.
/// Der Einkaufswagen kann bewegt werden, indem man das Smartphone nach links/rechts schiebt
/// </summary>
public class MarketGame : MonoBehaviour
{
    public Material transition;

    public static int lives;
    public static int pointCount;
    public int goalCount = 15;
    public AudioClip gameMusic;
    private AudioClip bgm;
    public AudioClip winClip;
    public AudioClip loseClip;
    private AudioSource aSrc;

    public AnimationCurve jumpCurve;

    public static float maxRange;

    [Header("Wichtige Scripte zur Kommunikation:")]
    public CartScript cart;
    public ThrowerScript thrower;
    public ParticleSystem clouds;
    private ParticleSystem.EmissionModule cloudsEmission;

    public static bool cartTouched;

    private Text scoreText;

    Animator anim_player, anim_comp;
    bool goHome, decisionMade;
    public static bool reset;

    // Start is called before the first frame update
    void Start()
    {
        //Lege die maximale Reichweite fest, in der die Waren fallen:
        maxRange = Camera.main.aspect * Camera.main.orthographicSize - 2.5f;
        cloudsEmission = clouds.emission;
        cloudsEmission.enabled = false;
        goalCount += difficulty * 5;

        player_startPos = new Vector3(-128, -2);

        thrower.SetParams(1.75f, new Vector2(3, 5), cart.transform.position.y);
        aSrc = GetComponent<AudioSource>();
        aSrc.loop = true;
        StartCoroutine(StartSequence());
        //StartCoroutine(WinSequence());
    }

    IEnumerator StartSequence()
    {
        aSrc.volume = 0.25f;
        bgm = aSrc.clip;
        aSrc.Play();

        canvas = GameObject.Find("Canvas").gameObject;
        scoreText = canvas.transform.GetChild(1).GetChild(1).GetChild(1).GetComponent<Text>();
        scoreText.text = "0/" + goalCount;

        thrower.transform.position = new Vector3(-16, -7.75f);
        cart.transform.position = new Vector3(-20, -6);
        anim_player = cart.transform.GetChild(1).GetChild(0).GetComponent<Animator>();
        anim_comp = thrower.transform.GetChild(0).GetComponent<Animator>();

        cScript.offset = Vector2.down * 4f;
        cScript.strength = .05f;
        Camera.main.orthographicSize = 3;

        //Bewege Chars ins Zentrum des Bildes:
        anim_player.SetBool("moving", true);
        anim_player.SetTrigger("startMove");
        anim_comp.SetBool("moving", true);
        anim_comp.SetTrigger("startMove");
        Vector3 step = Vector3.right * 5 * Time.fixedDeltaTime;
        while(cart.transform.position.x < -3)
        {
            cart.transform.position += step;
            thrower.transform.position += step;
            yield return new WaitForFixedUpdate();
        }
        anim_player.SetBool("moving", false);
        anim_comp.SetBool("moving", false);

        yield return new WaitForSeconds(1);
        cScript.offset = Vector2.up * 4f;
        yield return new WaitForSeconds(2);
        cScript.offset = Vector2.down * 4f;
        yield return new WaitForSeconds(2);


        anim_comp.Play("Squeesh");
        yield return new WaitForSeconds(.55f);
        anim_comp.Play("lookSad");
        yield return new WaitForSeconds(1);

        speachBubble.Say("Oh weh!", Bubbles.Normal, target: thrower.gameObject);
        speachBubble.Say("Wie sollen wir nur\nbei einem so großen\nRegal an die Sachen\nrankommen?", Bubbles.Normal, target: thrower.gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("Squeesh");
        anim_player.transform.parent.GetComponent<VoiceScript>().PlayMumble();
        yield return new WaitForSeconds(1);

        //Idee:
        GameObject bulb = transform.GetChild(0).gameObject;
        bulb.SetActive(true);
        bulb.transform.position = thrower.transform.position + Vector3.up;
        bulb.GetComponent<Animator>().Play("Idea");
        anim_comp.Play("Squeesh");
        yield return new WaitForSeconds(1);

        speachBubble.Say("Dann müssen wir\nhalt das Regal\numschmeißen!", Bubbles.Normal, target: thrower.gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookAngry");
        anim_player.transform.parent.GetComponent<VoiceScript>().PlayExplain();
        yield return new WaitForSeconds(1);

        anim_comp.Play("lookAnnoyed");
        speachBubble.Say("Ja ja,\ndas war ja nur\neine Idee", Bubbles.Normal, target: thrower.gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("lookNormal");
        yield return new WaitForSeconds(.1f);
        anim_player.Play("lookNormal");
        speachBubble.Say("Ok, ich kletter\ndas Regal rauf\nund schmeiße dir\ndie Sachen runter.", Bubbles.Normal, target: thrower.gameObject);
        speachBubble.Say("Du fängst sie dann\n mit dem Wagen auf.", Bubbles.Normal, target: thrower.gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookSad");
        anim_player.transform.parent.GetComponent<VoiceScript>().PlayQuestion();
        yield return new WaitForSeconds(1);

        //Thrower fängt an das Regal raufzuspringen;
        anim_comp.SetBool("moving", false);
        anim_comp.SetBool("inAir", true);
        anim_comp.SetTrigger("startMove");
        cScript.offset = Vector2.down * .5f;
        StartCoroutine(cScript.SetZoom(3.4f));
        StartCoroutine(Jump(new Vector3(5, -2.2f), 10, .75f));
        yield return new WaitForSeconds(1);
        anim_comp.SetBool("inAir", false);
        speachBubble.Say("Klar klappt das!\nWas kann da schon\nschief gehen?", Bubbles.Normal, target: thrower.gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);


        anim_comp.SetBool("inAir", true);
        cScript.offset = Vector2.zero;
        StartCoroutine(cScript.SetZoom(3.8f));
        StartCoroutine(Jump(new Vector3(0, -.35f), 0, .55f));
        yield return new WaitForSeconds(1);
        anim_comp.SetBool("inAir", false);
        speachBubble.Say("Lege das Handy\neinfach auf einen Tisch...", Bubbles.Normal, target: thrower.gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.SetBool("inAir", true);
        cScript.offset = Vector2.up * .5f;
        StartCoroutine(cScript.SetZoom(4.2f));
        StartCoroutine(Jump(new Vector3(-2.3f, 1.1f), -10, .4f));
        yield return new WaitForSeconds(1);
        anim_comp.SetBool("inAir", false);
        speachBubble.Say("...und ziehe es in\ndie Richtung, in die der\nEinkaufswagen\nfahren soll", Bubbles.Normal, target: thrower.gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.SetBool("inAir", true);
        cScript.offset = Vector2.up;
        StartCoroutine(cScript.SetZoom(4.6f));
        StartCoroutine(Jump(new Vector3(1.7f, 2.2f), 5, .35f));
        yield return new WaitForSeconds(1);
        anim_comp.SetBool("inAir", false);
        speachBubble.Say("Ganz einfach, oder?", Bubbles.Normal, target: thrower.gameObject, autoPlay: true, speed: .25f);
        yield return new WaitForSeconds(.1f);
        for(float count = 0; count < .5f; count += Time.fixedDeltaTime)
        {
            thrower.transform.position += Vector3.down * .02f;
            thrower.transform.Rotate(Vector3.forward, 1);
            yield return new WaitForFixedUpdate();
        }
        anim_player.Play("JumpScare");
        speachBubble.Say("Ups!", Bubbles.Normal, target: thrower.gameObject, speed: .25f);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        for (float count = 0; count < .5f; count += Time.fixedDeltaTime)
        {
            thrower.transform.position += Vector3.up * .02f;
            thrower.transform.Rotate(Vector3.forward, -1);
            yield return new WaitForFixedUpdate();
        }

        anim_player.Play("lookSad");
        anim_player.transform.parent.GetComponent<VoiceScript>().PlayQuestion();
        yield return new WaitForSeconds(1);

        speachBubble.Say("Alles ok!", Bubbles.Normal, target: thrower.gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_comp.Play("lookNormal");
        speachBubble.Say("Und wenn der\nEinkaufswagen irgendwo\nhängen bleibt,", Bubbles.Normal, target: thrower.gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.SetBool("inAir", true);
        cScript.offset = Vector2.up * 1.5f;
        StartCoroutine(cScript.SetZoom(5f));
        StartCoroutine(Jump(new Vector3(0, 3.15f), 0, .3f));
        yield return new WaitForSeconds(.75f);
        anim_comp.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName = "Background";
        yield return new WaitForSeconds(.25f);
        anim_comp.SetBool("inAir", false);
        speachBubble.Say("Dann kannst du ihn\ndurch ein doppeltes Tippen\nwieder in die\nMitte schieben.", Bubbles.Normal, target: thrower.gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookNormal");
        yield return new WaitForSeconds(.1f);
        anim_player.SetTrigger("startMove");

        runGame = true;
        cart.SetActive(true);
        StartCoroutine(ListenForPosReset());

        speachBubble.Say("Probiere eifnach mal\nden Wagen nach links und\nrechts zu schieben.", Bubbles.Normal, target: thrower.gameObject);
        speachBubble.Say("Wenn du bereit\nbist tippe auf den\nEinkaufswagen", Bubbles.Normal, target: thrower.gameObject);
        speachBubble.Say("Damit ich anfange dir\ndie Sachen zuzuwerfen", Bubbles.Normal, target: thrower.gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        //Starte Hauptspiel:
        StartCoroutine(MainGame());
        yield break;
    }

    IEnumerator WinSequence()
    {
        anim_player = cart.transform.GetChild(1).GetChild(0).GetComponent<Animator>();
        anim_comp = thrower.transform.GetChild(0).GetComponent<Animator>();

        anim_comp.Play("lookHappy");
        anim_player.Play("lookHappy");
        yield return new WaitForSeconds(2);

        cScript.offset = new Vector2(0, -4);
        StartCoroutine(Jump(new Vector2(4, -7.7f), 0, 1));
        yield return new WaitForSeconds(.2f);
        anim_comp.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName = "NPC";
        anim_player.Play("lookNormal");
        yield return new WaitForSeconds(.9f);
        anim_comp.Play("Squeesh");
        yield return new WaitForSeconds(1);

        anim_comp.Play("lookHappy");
        speachBubble.Say("Wir haben es geschafft!", Bubbles.Normal, target: thrower.gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_comp.Play("lookNormal");
        speachBubble.Say("Nur Eier, Brühe,\nKartoffeln und Mehl\nhaben wir nicht bekommen.", Bubbles.Normal, target: thrower.gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_comp.Play("lookHappy");
        speachBubble.Say("Aber die Sachen kriegen\nwir irgendwie schon\nnoch!", Bubbles.Normal, target: thrower.gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        
        menu.DoTransition(1, 1, false);
        Mat_Intro = transition;
        //Bewege Chars aus Zentrum des Bildes raus:
        //*
        anim_player.SetBool("moving", true);
        anim_player.SetTrigger("startMove");
        anim_comp.SetBool("moving", true);
        anim_comp.SetTrigger("startMove");
        player_startPos = new Vector3(-120, -2);
        Vector3 step = Vector3.right * 5 * Time.fixedDeltaTime;
        while (cart.transform.position.x < 12)
        {
            cart.transform.position += step;
            thrower.transform.position += step;
            yield return new WaitForFixedUpdate();
        }//*/
        SceneManager.LoadScene("World");
        yield break;
    }

    IEnumerator LoseSequence()
    {
        yield return new WaitForSeconds(2);
        GameObject worker = transform.GetChild(1).gameObject;
        worker.transform.position = new Vector3(-17, -7);
        worker.SetActive(true);
        Animator anim_worker = worker.transform.GetChild(0).GetComponent<Animator>();

        anim_worker.SetBool("moving", true);
        anim_worker.SetTrigger("startMove");
        Vector3 step = Vector3.right * 5 * Time.fixedDeltaTime;
        while (worker.transform.position.x < -6)
        {
            worker.transform.position += step;
            yield return new WaitForFixedUpdate();
        }
        anim_worker.SetBool("moving", false);
        yield return new WaitForSeconds(1);

        cScript.offset = Vector2.down * 2;

        anim_worker.Play("JumpScare");
        yield return new WaitForSeconds(.5f);

        speachBubble.Say("Was zum Henker...", Bubbles.Normal, target: worker, speed: .5f);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_worker.Play("lookFurious");
        speachBubble.Say("Warum liegen hier\ndie ganzen Lebensmittel\nauf dem Boden?", Bubbles.Shouting);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("JumpScare");
        yield return new WaitForSeconds(.2f);
        anim_comp.Play("JumpScare");
        yield return new WaitForSeconds(.4f);

        cScript.offset = Vector2.up * 4;

        anim_player.Play("lookSad");
        yield return new WaitForSeconds(.1f);
        anim_comp.Play("lookSad");
        yield return new WaitForSeconds(.5f);

        speachBubble.Say("Uhoh...", Bubbles.Normal, target: thrower.gameObject);
        speachBubble.Say("Lass uns lieber\nschnell verschwinden", Bubbles.Normal, target: thrower.gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        cScript.offset = Vector2.down * 2;

        anim_player.Play("Squeesh");
        anim_player.transform.parent.GetComponent<VoiceScript>().PlayMumble();
        yield return new WaitForSeconds(1);

        StartCoroutine(RunAwayPlayer());
        StartCoroutine(RunAwayComp());
        //StartCoroutine(cScript.SetZoom(3, 2));
        yield return new WaitForSeconds(1.5f);
        anim_worker.SetBool("moving", true);
        anim_worker.SetTrigger("startMove");
        step = Vector3.right * 10 * Time.fixedDeltaTime;
        while (worker.transform.position.x < 1)
        {
            worker.transform.position += step;
            yield return new WaitForFixedUpdate();
        }
        anim_worker.SetBool("moving", false);
        yield return new WaitForSeconds(.5f);

        anim_worker.Play("lookAngry");
        yield return new WaitForSeconds(1);

        while (worker.transform.position.x < 12)
        {
            worker.transform.position += step;
            yield return new WaitForFixedUpdate();
        }

        //Transition:
        menu.DoTransition(1, 1, false);
        yield return new WaitForSeconds(2.4f);
        reset = true;
        yield return new WaitForSeconds(.2f);
        reset = false;
        cScript.offset = Vector2.down * 4f;
        Camera.main.orthographicSize = 3;
        worker.SetActive(false);
        scoreText.text = "0/" + goalCount;
        scoreText.transform.parent.parent.gameObject.SetActive(false);
        thrower.transform.position = new Vector3(-16, -7.75f);
        cart.transform.position = new Vector3(-20, -6); menu.DoTransition(1, -1, true);
        yield return new WaitForSeconds(2);

        //Bewege Chars ins Zentrum des Bildes:
        anim_player.SetBool("moving", true);
        anim_player.SetTrigger("startMove");
        anim_comp.SetBool("moving", true);
        anim_comp.SetTrigger("startMove");
        step = Vector3.right * 5 * Time.fixedDeltaTime;
        while (cart.transform.position.x < -3)
        {
            cart.transform.position += step;
            thrower.transform.position += step;
            yield return new WaitForFixedUpdate();
        }
        anim_player.SetBool("moving", false);
        anim_comp.SetBool("moving", false);

        anim_player.Play("lookNormal");
        yield return new WaitForSeconds(.1f);
        anim_comp.Play("Squeesh");
        yield return new WaitForSeconds(1);

        speachBubble.Say("Puh,\nda sind wir gerade\nnochmal davongekommen", Bubbles.Normal, target: thrower.gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        speachBubble.Say("Wollen wir den Einkauf\nAufs Neue probieren\noder später\nwiederkommen?", Bubbles.Normal, target: thrower.gameObject);
        menu.SetDecisionActive(true);
        decisionMade = false;
        yield return new WaitUntil(() => decisionMade);
        menu.SetDecisionActive(false);
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => speachBubble.finished);
        //speachBubble.Clear = true;

        anim_player.transform.parent.GetComponent<VoiceScript>().PlayShout();
        anim_player.Play("Squeesh");
        yield return new WaitForSeconds(1);

        if (goHome)
        {
            anim_comp.Play("lookHappy");
            speachBubble.Say("Alles klar.\nWir können es ja\nspäter nochmal\nversuchen.", Bubbles.Normal, target: thrower.gameObject);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);

            anim_player.SetBool("moving", true);
            anim_player.SetTrigger("startMove");
            anim_comp.SetBool("moving", true);
            anim_comp.SetTrigger("startMove");
            step = Vector3.right * 5 * Time.fixedDeltaTime;
            while (cart.transform.position.x < -3)
            {
                cart.transform.position += step;
                thrower.transform.position += step;
                yield return new WaitForFixedUpdate();
            }
            anim_player.SetBool("moving", false);
            anim_comp.SetBool("moving", false);

            Mat_Intro = transition;
            menu.DoTransition(1, 1, false);
            yield return new WaitForSeconds(2.4f);
            SceneManager.LoadScene("World");
        }
        else
        {
            anim_comp.Play("lookNormal");
            speachBubble.Say("Gut, dann klettere ich\nwieder auf das Regal", Bubbles.Normal, target: thrower.gameObject);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);


            anim_comp.SetBool("moving", false);
            anim_comp.SetBool("inAir", true);
            anim_comp.SetTrigger("startMove");
            cScript.offset = Vector2.down * .5f;
            StartCoroutine(cScript.SetZoom(3.4f));
            StartCoroutine(Jump(new Vector3(5, -2.2f), 10, .75f));
            yield return new WaitForSeconds(1);
            anim_comp.SetBool("inAir", false);
            yield return new WaitForSeconds(.5f);

            anim_comp.SetTrigger("startMove");
            anim_comp.SetBool("inAir", true);
            cScript.offset = Vector2.zero;
            StartCoroutine(cScript.SetZoom(3.8f));
            StartCoroutine(Jump(new Vector3(0, -.35f), 0, .55f));
            yield return new WaitForSeconds(1);
            anim_comp.SetBool("inAir", false);
            yield return new WaitForSeconds(.5f);

            anim_comp.SetTrigger("startMove");
            anim_comp.SetBool("inAir", true);
            cScript.offset = Vector2.up * .5f;
            StartCoroutine(cScript.SetZoom(4.2f));
            StartCoroutine(Jump(new Vector3(-2.3f, 1.1f), -10, .4f));
            yield return new WaitForSeconds(1);
            anim_comp.SetBool("inAir", false);
            yield return new WaitForSeconds(.5f);

            anim_comp.SetBool("inAir", true);
            cScript.offset = Vector2.up;
            StartCoroutine(cScript.SetZoom(4.6f));
            StartCoroutine(Jump(new Vector3(1.7f, 2.2f), 5, .35f));
            yield return new WaitForSeconds(1);
            anim_comp.SetBool("inAir", false);
            yield return new WaitForSeconds(.5f);

            anim_comp.SetBool("inAir", true);
            cScript.offset = Vector2.up * 1.5f;
            StartCoroutine(cScript.SetZoom(5f));
            StartCoroutine(Jump(new Vector3(0, 3.15f), 0, .3f));
            yield return new WaitForSeconds(.75f);
            anim_comp.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName = "Background";
            yield return new WaitForSeconds(.25f);
            anim_comp.SetBool("inAir", false);
            yield return new WaitForSeconds(.5f);

            anim_player.SetTrigger("startMove");
            runGame = true;
            cart.SetActive(true);
            StartCoroutine(ListenForPosReset());

            speachBubble.Say("Ich bin bereit!\nWenn du auch bereit bist\ntippe den Einkaufswagen\nan", Bubbles.Normal, target: thrower.gameObject);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);

            StartCoroutine(MainGame());
        }
        yield break;
    }

    IEnumerator Jump(Vector2 goalPos, float goalRot, float goalScale)
    {
        float y_start = thrower.transform.position.y;
        float y_diff = goalPos.y - y_start;
        bool jumpDown = y_diff < 0;
        if (jumpDown) y_start += y_diff;
        y_diff = Mathf.Abs(y_diff);
        Debug.Log(jumpDown);

        float scale_start = thrower.transform.localScale.x;
        float scale_diff = scale_start - goalScale;
        scale_diff = Mathf.Abs(scale_diff);

        float x_speed = Time.fixedDeltaTime * (goalPos.x - thrower.transform.position.x);
        float rotation = goalRot - thrower.transform.eulerAngles.z;
        while (Mathf.Abs(rotation) > 180) rotation -= Mathf.Sign(rotation) * 360;
        rotation *= Time.fixedDeltaTime;

        float percent;
        for (float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            percent = !jumpDown ? count : 1 - count;
            thrower.transform.localScale = Vector3.one * (scale_start + ((jumpDown? 1 : 0) - jumpCurve.Evaluate(percent)) * scale_diff);
            thrower.transform.Rotate(Vector3.forward, rotation);
            thrower.transform.position += Vector3.right * x_speed;
            thrower.transform.position = new Vector3(thrower.transform.position.x, (y_start + jumpCurve.Evaluate(percent) * y_diff));
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }

    IEnumerator RunAwayPlayer()
    {
        anim_player.SetBool("moving", true);
        anim_player.SetTrigger("startMove");
        Vector3 step = Vector3.right * 10 * Time.fixedDeltaTime;
        while (cart.transform.position.x < 16)
        {
            cart.transform.position += step;
            yield return new WaitForFixedUpdate();
        }
        anim_player.SetBool("moving", false);

        yield break;
    }

    IEnumerator RunAwayComp()
    {
        StartCoroutine(Jump(new Vector2(6.5f, -7.7f), 0, 1));
        yield return new WaitForSeconds(.75f);
        anim_comp.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName = "NPC";
        yield return new WaitForSeconds(.25f);
        anim_comp.Play("Squeesh");

        anim_comp.SetBool("moving", true);
        anim_comp.SetTrigger("startMove");
        Vector3 step = Vector3.right * 10 * Time.fixedDeltaTime;
        while (thrower.transform.position.x < 16)
        {
            thrower.transform.position += step;
            yield return new WaitForFixedUpdate();
        }
        anim_comp.SetBool("moving", false);

        yield break;
    }

    IEnumerator MainGame()
    {
        //Setze Score:
        scoreText.text = "0/" + goalCount;
        CanvasGroup gameInfo = canvas.transform.GetChild(1).GetComponent<CanvasGroup>();
        if (!gameInfo.gameObject.activeSelf)
        {
            gameInfo.gameObject.SetActive(true);
            for(float count = 0; count < 1; count += Time.fixedDeltaTime)
            {
                gameInfo.alpha = count;
                yield return new WaitForFixedUpdate();
            }
        }
       
        if (!aSrc.isPlaying) { aSrc.volume = 0.25f; aSrc.Play(); }

        cloudsEmission.enabled = false;
        yield return new WaitUntil(() => Input.touchCount == 0);
        //Warte, bis der Wagen berührt wird:
        cartTouched = false;
        yield return new WaitUntil(() => !pauseGame && cartTouched);


        //run game:
        bool gameWon = false;
        runGame = true;
        StartCoroutine(ListenForPosReset());
        StartCoroutine(thrower.ThrowObjects());
        aSrc.Stop();
        cScript.aSrc.loop = true;
        cScript.aSrc.clip = gameMusic;
        cScript.aSrc.Play();
        lives = 3;
        pointCount = 0;
        while (runGame)
        {
            cloudsEmission.enabled = true;
            cart.SetActive(true);
            while(!pauseGame && runGame)
            {
                yield return new WaitForFixedUpdate();
                if (lives <= 0) runGame = false;
                scoreText.text = pointCount + "/" + goalCount;
                if (pointCount >= goalCount) { runGame = false; gameWon = true; }
            }
            cart.SetActive(false);
            cloudsEmission.enabled = false;
        }


        aSrc.volume = 0;
        aSrc.Play();
        for(float volume = 1; volume > 0; volume -= 0.02f) { cScript.aSrc.volume = volume; aSrc.volume = (1 - volume)*.25f; yield return new WaitForFixedUpdate(); }
        cScript.aSrc.Stop();
        cScript.aSrc.loop = false;
        cScript.aSrc.volume = 1;
        if (gameWon)
        {
            cScript.aSrc.clip = winClip;
            cScript.aSrc.Play();
            progress.marketDone = true;
            LoadSave.SaveProgress();
            StartCoroutine(WinSequence());
        }
        else
        {
            //Hier Sequenz einfügen, die dem Spieler sagt, dass man verloren hat
            cScript.aSrc.clip = loseClip;
            cScript.aSrc.Play();
            cart.ResetPosition();
            for(int i = 0; i < cart.transform.GetChild(0).childCount; i++) Destroy(cart.transform.GetChild(0).GetChild(i).gameObject);           
            StartCoroutine(LoseSequence());
        }

        yield return new WaitWhile(() => aSrc.isPlaying);
        aSrc.clip = bgm;
        aSrc.volume = .25f;
        aSrc.loop = true;
        aSrc.Play();
        yield break;
    }

    private bool listenforPos;
    IEnumerator ListenForPosReset()
    {
        if (listenforPos) yield break;
        listenforPos = true;
        float clickIntervall = 0.3f;
        while (runGame)
        {
            if(Input.touchCount > 0)
            {
                float count = 0;
                for (; Input.touchCount != 0 && count < clickIntervall; count += Time.deltaTime) yield return new WaitForEndOfFrame();
                for (; Input.touchCount == 0 && count < clickIntervall; count += Time.deltaTime) yield return new WaitForEndOfFrame();
                if (count < clickIntervall) StartCoroutine(cart.ResetPosition());
            }
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1);
        StartCoroutine(cart.ResetPosition());
        listenforPos = false;
        yield break;
    }

    public void MakeDecision(bool _goHome) { goHome = _goHome; decisionMade = true; }
}
