using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static CameraScript;
using static MenuScript;
using static BubbleScript;
using static TasklistScript;
using UnityEngine.Events;
using UnityEngine.UI;

public class MillScript : MonoBehaviour
{
    public AudioClip gameMusic;
    public UnityAction ButtonPressed, ButtonReleased;

    [Header("Construct-Game:")]
    public GameObject touchpoint;
    private List<GameObject> activeTouchPoints = new List<GameObject>();
    public Vector2[] touchPointPos;
    public float waitTime = 2;
    int partsBuild = -1;

    public AnimationCurve fallCurve;

    private Vector2[] partPos;
    private Transform partHolder;

    [Header("BlowingGame:")]
    public float goal;
    public float gain;
    public float loss;
    public float thresh_sound;
    public GameObject flour;
    private float loss_real;
    private float goal_real;

    private bool buttonHold, buttonReleased, game_running;
    public static bool flourTaken;
    public static GameObject millSeq_Object;

    Animator anim, anim_player, anim_comp, anim_ham, anim_screw, anim_saw, anim_farmer;
    GameObject hammer, screwdriver, saw;

    Vibration vib;

    private void Awake()
    {
        millSeq_Object = transform.parent.gameObject;
        anim = GetComponent<Animator>();

        vib = new Vibration();
        vib.SetVibrationEffect(new long[50] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 });
        
    }

    private void OnDestroy()
    {
        vib.DestroyVibration();
    }

    private void Start()
    {
        partHolder = transform.GetChild(1);
        partPos = new Vector2[partHolder.childCount];
        for (int i = 0; i < partPos.Length; i++) partPos[i] = partHolder.GetChild(i).localPosition;
        partHolder.gameObject.SetActive(false);

        ButtonPressed += OnButtonPressed;
        ButtonReleased += OnButtonReleased;

        hammer = transform.parent.GetChild(5).gameObject;
        screwdriver = transform.parent.GetChild(4).gameObject;
        saw = transform.parent.GetChild(3).gameObject;


        anim = transform.parent.GetComponent<Animator>();
        anim_player = player.transform.GetChild(0).GetComponent<Animator>();
        anim_comp = companion.transform.GetChild(0).GetComponent<Animator>();
        anim_saw = saw.transform.GetChild(0).GetComponent<Animator>();
        anim_screw = screwdriver.transform.GetChild(0).GetComponent<Animator>();
        anim_ham = hammer.transform.GetChild(0).GetComponent<Animator>();
        anim_farmer = transform.parent.GetChild(1).GetChild(0).GetComponent<Animator>();

        StartCoroutine(SetMillState());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.GetComponent<TouchSensor>().tipped || runGame || pauseGame || other.gameObject.layer != 14/*Touch*/) return;

        StartCoroutine(StartSequence());
    }

    IEnumerator SetMillState()
    {
        if (progress.millState % 3 == 2)
        {
            GetComponent<SpriteRenderer>().enabled = true;
            transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
            transform.GetChild(0).eulerAngles = new Vector3(0, 0, -102);
            partHolder.gameObject.SetActive(false);
            partsBuild = -1;
            yield break;
        }

        GetComponent<SpriteRenderer>().enabled = false;
        transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        partHolder.gameObject.SetActive(true);

        Rigidbody2D rb_part;

        partHolder.GetChild(0).localPosition = partPos[0];
        for (int i = partsBuild == -1 ? partHolder.childCount-1 : partsBuild; i > 0; i--) {
            partHolder.GetChild(i).localPosition = partPos[i];
            rb_part = partHolder.GetChild(i).GetComponent<Rigidbody2D>();
            rb_part.GetComponent<Collider2D>().enabled = true;
            rb_part.bodyType = RigidbodyType2D.Dynamic;
            rb_part.velocity = new Vector2(rb_part.transform.localPosition.y, rb_part.transform.localPosition.x);
            rb_part.angularVelocity = Random.Range(-10f, 10f);
        }
        partsBuild = -1;
        yield break;
    }

    IEnumerator BuildPart(int partNumber)
    {
        Transform part = transform.GetChild(1).GetChild(partNumber);
        part.GetComponent<Collider2D>().enabled = false;
        part.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        Vector2 diff_pos = part.localPosition;
        float diff_rot = part.localRotation.eulerAngles.z;

        for(float count = 1; count > 0; count -= Time.fixedDeltaTime)
        {
            part.localPosition = partPos[partNumber]  + count * diff_pos;
            part.localRotation = Quaternion.Euler(0,0,diff_rot * count);
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }

    IEnumerator MillGame()
    {
        yield return new WaitForSeconds(.1f);
        if (pauseGame) yield break;
        runGame = true;
        staticCam = true;
        pauseMove = true;
        cScript.offset = Vector2.up * 8;
        cScript.transform.rotation = Quaternion.identity;
        cScript.target = transform.parent.gameObject;
        StartCoroutine(SetTaskList(false));

        //Rotiere die Camera um 90°:
        StartCoroutine(cScript.SetRotation(90));
        StartCoroutine(cScript.SetZoom(7));
        cScript.offset = Vector2.up * 8;
        StartCoroutine(menu.SetMenuRotation(_isPortrait: true));
        anim.SetTrigger("reset");

        yield return new WaitForSeconds(1);

        activeTouchPoints.Clear();
        yield return new WaitUntil(() => Input.touchCount == 0);
        cScript.aSrc.Stop();
        cScript.aSrc.loop = true;
        cScript.aSrc.clip = gameMusic;
        cScript.aSrc.Play();
        yield return new WaitForSeconds(2f);

        float timeCount;
        buttonReleased = false;
        float touchStepNumber = touchPointPos.Length + 2 * (difficulty - 1);
        int oldBuildPart = 0;
        int animDivider = Mathf.FloorToInt(touchStepNumber / 4f) + 1;
        anim.SetTrigger("next");
        int counter = 0;
        for(int i = 0; i < touchStepNumber && !buttonReleased ; i++)
        {
            //Bauanimation:
            partsBuild = Mathf.CeilToInt((i + 1) * (10f / touchStepNumber));
            for(int j = oldBuildPart; j < partsBuild; j++) StartCoroutine(BuildPart(j));
            oldBuildPart = partsBuild;
            if (i % animDivider == 1) { anim.SetTrigger("next"); }

            //Update die touchpoints:
            activeTouchPoints.Insert(0, Instantiate(touchpoint, touchPointPos[i] + (Vector2)transform.position, Quaternion.identity));
            activeTouchPoints[0].GetComponent<InGameButton>().SetCallback(ButtonPressed, ButtonReleased);
            buttonHold = false;
            yield return new WaitUntil(() => buttonHold || buttonReleased);
            vib.Vibrate(0);
            timeCount = 0;
            while(timeCount < waitTime && !buttonReleased) { yield return new WaitForEndOfFrame(); timeCount += Time.deltaTime; }
        }

        foreach (var obj in activeTouchPoints) obj.GetComponent<InGameButton>().active = false;
        manager.PlayNormal();

        StartCoroutine(cScript.SetRotation());
        StartCoroutine(menu.SetMenuRotation(_isPortrait: false));
        yield return new WaitForSeconds(1);

        //Wenn einer der Buttons released wurde, dann hat der Spieler das Spiel verloren, ansonsten hat er das Spiel gewonnen:
        if (buttonReleased)
            StartCoroutine(BuildLose());
        else
        {
            progress.millState++;
            LoadSave.SaveProgress();
            StartCoroutine(BuildWin());
        }

        StartCoroutine(SetMillState());
        yield return new WaitForFixedUpdate();
        foreach (GameObject obj in activeTouchPoints) StartCoroutine(DestroyButton(obj));
        activeTouchPoints.Clear();

        yield break;
    }

    IEnumerator DestroyButton(GameObject gameButton)
    {
        SpriteRenderer sprite = gameButton.transform.GetChild(0).GetComponent<SpriteRenderer>();
        float timeStep = Time.fixedDeltaTime / 4;
        Color colorStep = Color.black * timeStep;
        for(float count = 0; count < 1; count += timeStep)
        {
            sprite.color -= colorStep;
            yield return new WaitForFixedUpdate();
        }

        Destroy(gameButton);
        yield break;
    }

    IEnumerator ClearGame()
    {
        //Clear Game:
        //yield return new WaitUntil(() => speachBubble.finished);
        //yield return new WaitUntil(() => !speachBubble.finished);
        //Rotiere die Camera zurück:
        StartCoroutine(cScript.SetRotation());
        StartCoroutine(menu.SetMenuRotation(_isPortrait: false));
        StartCoroutine(cScript.SetZoom(8));


        yield return new WaitUntil(() => Input.touchCount == 0);
        player.GetComponent<Rigidbody2D>().simulated = true;
        player.GetComponent<PlayerScript>().enabled = true;
        companion.GetComponent<FollowScript>().enabled = true;
        anim_player.SetTrigger("startMove");
        runGame = false;
        staticCam = false;
        pauseMove = false;
        cScript.target = player;
        StartCoroutine(SetTaskList(true));

        yield break;
    }

    IEnumerator BlowMill()
    {
        //Rotiere die Camera um 90°:
        StartCoroutine(cScript.SetRotation(90));
        StartCoroutine(cScript.SetZoom(7));
        cScript.target = transform.parent.gameObject;
        cScript.offset = Vector2.up * 8;
        StartCoroutine(menu.SetMenuRotation(_isPortrait: true));

        loss_real = loss * Time.fixedDeltaTime;
        goal_real = goal * Time.fixedDeltaTime;
        StartCoroutine(VoiceScript.ReportFrequencies());

        game_running = true;
        float rotation = 0;
        bool win = false;
        Transform wings = transform.GetChild(0);
        while (game_running)
        {
            rotation += VoiceScript.Signal.power > thresh_sound ? gain * VoiceScript.Signal.power : -loss_real;
            if (rotation < 0) rotation = 0;
            wings.Rotate(Vector3.forward, rotation);
            if (win = rotation > goal_real) game_running = false;
            yield return new WaitForFixedUpdate();
        }
        VoiceScript.runFetch = false;

        //Aktion, die bei Sieg/ Niederlage ausgeführt wird:
        if (win) StartCoroutine(BlowWin());

        //Clear Game:
        yield return new WaitUntil(() => speachBubble.finished);
        yield return new WaitUntil(() => !speachBubble.finished);
        runGame = false;
        staticCam = false;
        pauseMove = false;
        cScript.target = player;
        yield break;
    }

    IEnumerator BlowWin()
    {
        cScript.target = transform.parent.gameObject;


        //Spawn Mehlsack:
        Instantiate(flour, transform.position, Quaternion.identity);

        //Lasse die Mühle wegfliegen:
        Transform wings = transform.GetChild(0);
        Vector3 step = Vector3.up * 42 * Time.fixedDeltaTime / 6;
        flourTaken = progress.millState < 3;
        for(float count = 0; count < 6; count += Time.fixedDeltaTime)
        {
            wings.Rotate(Vector3.forward, goal_real);
            transform.position += step;
            yield return new WaitForFixedUpdate();
        }

        StartCoroutine(ClearGame());

        if(progress.millState < 3)
            yield return new WaitUntil(() => flourTaken);
        else
            yield return new WaitForSeconds(5f);

        progress.millState = 3;
        LoadSave.SaveProgress();

        //Mühle kracht auf die Erde:
        transform.Rotate(Vector3.forward, 180);
        for (float count = 0; count < 4f; count += Time.fixedDeltaTime)
        {
            wings.Rotate(Vector3.forward, goal_real);
            transform.position -= step;
            yield return new WaitForFixedUpdate();
        }

        StartCoroutine(SetMillState());

        Rigidbody2D rb_part;
        Transform[] tmp = new Transform[partHolder.childCount];
        for (int i = partHolder.childCount - 1; i >= 0 ; i--)
        {
            tmp[i] = partHolder.GetChild(i);
            tmp[i].transform.localPosition = partPos[i];            
            if (i == 0) { tmp[i].parent = null; continue; }

            rb_part = tmp[i].GetComponent<Rigidbody2D>();
            rb_part.GetComponent<Collider2D>().enabled = true;
            rb_part.bodyType = RigidbodyType2D.Dynamic;
            rb_part.velocity = new Vector2(-rb_part.transform.localPosition.y, rb_part.transform.localPosition.x);
            rb_part.angularVelocity = Random.Range(-10f, 10f);
            tmp[i].parent = null;
        }

        anim_farmer.Play("JumpScare");

        //Bewege basis zur Ausgangsposition
        step = (new Vector3(68.67f, 7.3f) - tmp[0].transform.position) * Time.fixedDeltaTime;
        float rotation = (-90 - tmp[0].transform.eulerAngles.z) * Time.fixedDeltaTime;
        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            tmp[0].position += step;
            tmp[0].Rotate(Vector3.forward, rotation);
            yield return new WaitForFixedUpdate();
        }
        transform.position += Vector3.down * 12;
        transform.Rotate(Vector3.forward, -180);

        for (int i = 0; i < tmp.Length; i++)
            tmp[i].parent = partHolder;


        anim_farmer.Play("lookFurious");
        yield break;
    }

    IEnumerator StartSequence()
    {
        player.GetComponent<PlayerScript>().enabled = false;
        player.GetComponent<Rigidbody2D>().simulated = false;
        companion.GetComponent<FollowScript>().enabled = false;

        yield return new WaitForSeconds(.1f);
        anim_player.SetBool("inAir", false);
        anim_player.SetBool("moving", false);
        anim_comp.SetBool("inAir", false);
        anim_comp.SetBool("moving", false);

        yield return new WaitForSeconds(2);

        switch (progress.millState % 3)
        {
            case 0://Mühle kaputt, niemand da:
                cScript.target = transform.parent.GetChild(1).gameObject;
                StartCoroutine(cScript.SetZoom(3, .5f));
                StartCoroutine(cScript.SetRotation(-10, .5f));
                cScript.offset = Vector2.up * 2;
                yield return new WaitForSeconds(1);

                anim_farmer.Play("lookFurious");
                speachBubble.Say("Meine schöne\nMühle!!!", Bubbles.Shouting);
                speachBubble.Say("Arrrgh", Bubbles.Shouting);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                StartCoroutine(cScript.SetZoom(6, 2f));
                StartCoroutine(cScript.SetRotation(rotTime: 2));
                cScript.target = companion;
                yield return new WaitForSeconds(1);

                anim_comp.Play("lookSad");
                yield return new WaitForSeconds(.1f);
                speachBubble.Say("Wir müssen die Mühle\nirgendwie wieder aufbauen", Bubbles.Normal, target: companion);
                speachBubble.Say("Sonst bekommen\nwir kein Mehl", Bubbles.Normal, target: companion);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                player.GetComponent<VoiceScript>().PlayMumble();
                anim_player.Play("lookAngry");
                yield return new WaitForSeconds(1);

                anim_comp.Play("lookAnnoyed");
                yield return new WaitForSeconds(.1f);
                speachBubble.Say("Schon gut!\nWir müssen die Mühle auch\nwieder aufbauen,", Bubbles.Normal, target: companion);
                speachBubble.Say("weil wir sie kaputt\ngemacht haben", Bubbles.Normal, target: companion);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                anim_comp.Play("lookSad");
                speachBubble.Say("Alleine schaffen wir es\nnie die Mühle wieder\naufzubauen", Bubbles.Normal, target: companion);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                player.GetComponent<VoiceScript>().PlayName();
                anim_player.Play("lookSad");
                yield return new WaitForSeconds(1);

                anim_player.Play("lookNormal");
                yield return new WaitForSeconds(.1f);
                anim_comp.Play("lookFocused");

                speachBubble.Say("Hm, wo finden wir\nbloß jemanden, der\nuns helfen kann?", Bubbles.Normal, target: companion);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                //Idee:
                anim_comp.Play("Squeesh");
                GameObject bulb = transform.parent.GetChild(2).gameObject;
                bulb.SetActive(true);
                bulb.transform.position = companion.transform.position + Vector3.up * 2;
                bulb.GetComponent<Animator>().Play("Idea");
                yield return new WaitForSeconds(1);

                speachBubble.Say("Vielleicht finden wir\njemanden in der Stadt", Bubbles.Normal, target: companion);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                StartCoroutine(ClearGame());

                break;
            case 1://Mühle kaputt, Aufbauspiel beginnt:

                //Bewege Player, bis er die Ausgangslage erreicht:
                companion.GetComponent<FollowScript>().enabled = true;
                anim_player.SetBool("moving", true);
                anim_player.SetTrigger("startMove");
                player.GetComponent<Rigidbody2D>().simulated = true;
                player.GetComponent<Rigidbody2D>().gravityScale = 0.5f;
                float x_step = .15f * Mathf.Sign(77.5f - player.transform.position.x);
                while (Mathf.Abs(player.transform.position.x - 77.5f) > Mathf.Abs(x_step))
                {
                    player.transform.position += Vector3.right * x_step;
                    if (player.transform.position.x > 70) player.GetComponent<Rigidbody2D>().gravityScale = 2;
                    yield return new WaitForFixedUpdate();
                }
                companion.GetComponent<FollowScript>().enabled = false;
                player.GetComponent<Rigidbody2D>().gravityScale = 2f;
                player.GetComponent<Rigidbody2D>().simulated = false;
                yield return new WaitForSeconds(.1f);
                anim_player.SetBool("moving", false);
                anim_player.SetBool("inAir", false);
                anim_comp.SetBool("moving", false);

                cScript.offset = Vector2.zero;
                cScript.strength = .03f;
                StartCoroutine(cScript.SetZoom(6, 3));

                //StartMoveTools();
                MoveTools(true);
                yield return new WaitForSeconds(.1f);
                anim.Play("Arrive");
                yield return new WaitForSeconds(6);
                MoveTools(false);

                cScript.strength = 0.05f;

                anim_ham.Play("lookHappy");
                speachBubble.Say("Hier sind wir!\nBereit Sachen zu bauen.", Bubbles.Normal, target: hammer);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                anim_saw.Play("smallJump");
                speachBubble.Say("Oder abzureißen", Bubbles.Normal, target: saw, speed: .5f);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);
                anim_screw.Play("lookAnnoyed");
                speachBubble.Say("Oder zu renovieren", Bubbles.Normal, target: saw, speed: .5f);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);
                anim_ham.Play("lookAnnoyed");
                speachBubble.Say("Oder zu Wände\nzu streichen", Bubbles.Normal, target: saw.gameObject, speed: .5f);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);
                anim_player.Play("lookAnnoyed");
                anim_comp.Play("lookAnnoyed");
                speachBubble.Say("Oder...", Bubbles.Normal, target: saw, speed: .2f, autoPlay: true);
                yield return new WaitForSeconds(1f);
                speachBubble.Say("...Ich glaube,\nsie haben es verstanden.", Bubbles.Normal, target: screwdriver);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);


                anim_player.Play("lookNormal");
                yield return new WaitForSeconds(.1f);
                anim_comp.Play("lookNormal");
                anim_screw.Play("lookNormal");
                yield return new WaitForSeconds(.1f);
                anim_ham.Play("lookNormal");
                yield return new WaitForSeconds(.1f);
                anim_screw.Play("JumpScare");
                yield return new WaitForSeconds(1f);
                speachBubble.Say("Was ist denn mit\nder Mühle passiert?", Bubbles.Normal, target: screwdriver);
                speachBubble.Say("Sie ist ja komplett\nzerstört!", Bubbles.Normal, target: screwdriver);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                anim_comp.Play("lookSad");
                yield return new WaitForSeconds(.1f);
                anim_player.Play("lookSad");

                speachBubble.Say("Glaubt ihr,\ndass ihr die Mühle\nreparieren könnt?", Bubbles.Normal, target: companion);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                anim_ham.Play("lookHappy");
                speachBubble.Say("Klar schaffen wir das!\nWir sind echte Profis\ndarin Sachen zu reparieren", Bubbles.Normal, target: hammer);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                anim_comp.Play("lookNormal");
                yield return new WaitForSeconds(.1f);
                anim_player.Play("lookNormal");

                anim_screw.Play("lookFocused");
                speachBubble.Say("Hm, aber die Mühle\nist so zerstört", Bubbles.Normal, target: screwdriver);
                speachBubble.Say("da werden selbst wir\nHilfe brauchen", Bubbles.Normal, target: screwdriver);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                anim_ham.Play("lookNormal");
                yield return new WaitForSeconds(.1f);
                anim_screw.Play("Squeesh");

                speachBubble.Say("Ihr müsst die Trümmer\nhalten, damit wir sie\nwieder anbauen können.", Bubbles.Normal, target: screwdriver);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                anim_ham.Play("smallJump");
                speachBubble.Say("Keine Sorge:\nWir markieren euch\ndie Trümmer, die\nihr halten sollt.", Bubbles.Normal, target: hammer);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                anim_screw.Play("lookSad");
                speachBubble.Say("Aber nicht loslassen,\n sonst stürzt alles ein!", Bubbles.Normal, target: screwdriver);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                anim_screw.Play("lookNormal");


                speachBubble.Say("Seit ihr bereit?", Bubbles.Normal, target: hammer);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);
                anim_ham.Play("smallJump");
                speachBubble.Say("Dann lasst uns die\nMühle reparieren", Bubbles.Normal, target: hammer);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                StartCoroutine(MillGame());

                break;
            case 2://Mühle repariert, Puste- Spiel beginnt:
                StartCoroutine(cScript.SetZoom(6, 2f));
                StartCoroutine(cScript.SetRotation(rotTime: 2));
                cScript.offset = Vector2.up * 2;
                cScript.target = companion;
                yield return new WaitForSeconds(1);

                anim_comp.Play("Squeesh");
                speachBubble.Say("Die Mühle ist wieder\nrepariert.", Bubbles.Normal, target: companion, speed: .5f);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                speachBubble.Say("Jetzt brauchen wir nur\nnoch etwas Wind, damit\ndas Mehl gemahlen wird.", Bubbles.Normal, target: companion, speed: 1f);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                anim_player.Play("lookHappy");
                player.GetComponent<VoiceScript>().PlayShout();
                yield return new WaitForSeconds(1);

                cScript.offset = Vector2.up * 8;
                cScript.target = gameObject;
                yield return new WaitForSeconds(1);
                anim_player.Play("lookNormal");
                yield return new WaitForSeconds(2);

                cScript.offset = Vector2.up * 2;
                cScript.target = companion;
                yield return new WaitForSeconds(1);

                anim_comp.Play("smallJump");
                yield return new WaitForSeconds(.5f);
                anim_comp.Play("smallJump1");
                yield return new WaitForSeconds(.5f);
                anim_comp.Play("lookAngry");
                yield return new WaitForSeconds(.5f);

                speachBubble.Say("Warum muss es\nausgerechnet heute\nwindstill sein?", Bubbles.Normal, target: companion, speed: 1f);
                speachBubble.Say("Dann müssen wir da\nselbst nachhelfen!", Bubbles.Normal, target: companion, speed: 1f);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                anim_player.Play("lookSad");
                player.GetComponent<VoiceScript>().PlayQuestion();
                yield return new WaitForSeconds(1);

                anim_comp.Play("Squeesh");
                speachBubble.Say("...tief einatmen...\n\n...und dann...", Bubbles.Normal, target: companion, speed: 1.5f);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                anim_comp.Play("smallJump");
                speachBubble.Say("PUUUUSSTTEEEENNN!!!", Bubbles.Shouting, target: companion, speed: 1.5f, fontSize: 30);
                yield return new WaitForSeconds(.1f);
                anim_player.Play("JumpScare");
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                StartCoroutine(BlowMill());

                break;
        }

    }

    IEnumerator BuildWin()
    {
        anim.SetTrigger("reset");
        yield return new WaitForSeconds(2);
        cScript.offset = Vector2.up * 2;

        anim_ham.Play("lookHappy");
        yield return new WaitForSeconds(.1f);
        anim_saw.Play("lookHappy");
        yield return new WaitForSeconds(.1f);
        anim_screw.Play("lookHappy");
        yield return new WaitForSeconds(.1f);

        anim_player.Play("lookHappy");
        yield return new WaitForSeconds(.1f);
        anim_comp.Play("lookHappy");


        speachBubble.Say("Hurra!\nWir haben es\ngeschafft!", Bubbles.Shouting, fontSize: 35);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        cScript.target = anim_farmer.transform.parent.gameObject;
        StartCoroutine(cScript.SetZoom(3));
        cScript.offset = Vector2.up * 2;
        yield return new WaitForSeconds(2);

        anim_farmer.Play("JumpScare");
        speachBubble.Say("Meine Mühle!\nSie steht wieder!", Bubbles.Normal, target: anim_farmer.transform.parent.gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        yield return new WaitForSeconds(.5f);
        anim_farmer.Play("lookHappy");
        yield return new WaitForSeconds(.5f);
        speachBubble.Say("Hurra!", Bubbles.Normal, target: anim_farmer.transform.parent.gameObject.gameObject, speed: .2f);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_ham.Play("lookNormal");
        anim_saw.Play("lookNormal");
        anim_screw.Play("lookNormal");
        anim_player.Play("lookNormal");
        anim_comp.Play("lookNormal");

        cScript.target = gameObject;
        StartCoroutine(cScript.SetZoom(6));
        cScript.offset = Vector2.up * 2;
        yield return new WaitForSeconds(2);

        anim_comp.Play("smallJump");
        speachBubble.Say("Danke, dass ihr\nuns geholfen habt", Bubbles.Normal, target: companion);
        yield return new WaitUntil(()=>Input.touchCount > 0);
        yield return new WaitUntil(()=>Input.touchCount == 0);
        speachBubble.Say("Ohne euch hätten wir\ndas nie geschafft!", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        speachBubble.Say("Kein Probl...", Bubbles.Normal, target: screwdriver, speed: .2f, autoPlay: true);
        yield return new WaitForSeconds(1.5f);
        anim_saw.Play("smallJump");
        speachBubble.Say("Das macht dann\n83683,87 $", Bubbles.Normal, target: saw);
        yield return new WaitForSeconds(.2f);
        anim_comp.Play("JumpScare");
        anim_player.Play("JumpScare");
        yield return new WaitForSeconds(.1f);
        anim_screw.Play("lookAnnoyed");
        yield return new WaitForSeconds(.1f);
        anim_ham.Play("lookAnnoyed");
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("lookScared");
        yield return new WaitForSeconds(.1f);
        anim_player.Play("lookSad");

        anim_screw.Play("lookAngry");
        speachBubble.Say("Hört nicht auf\ndie Nervensäge", Bubbles.Normal, target: screwdriver);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_ham.Play("lookNormal");
        speachBubble.Say("Ihr seit Mitglieder der\nHandwerker-Guilde,\noder?", Bubbles.Normal, target: screwdriver);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_player.Play("lookNormal");
        yield return new WaitForSeconds(.1f);
        anim_comp.Play("lookNormal");
        anim_screw.Play("lookNormal");
        yield return new WaitForSeconds(.1f);
        anim_comp.Play("lookInPain");

        speachBubble.Say("Öhm, natürlich!", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        speachBubble.Say("Natürlich müsst ihr\nnichts bezahlen.", Bubbles.Normal, target: screwdriver);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookNormal");
        yield return new WaitForSeconds(.1f);
        anim_comp.Play("lookNormal");

        yield return new WaitForSeconds(1);
        anim_ham.Play("lookHappy");
        speachBubble.Say("Wir bilden ein\nHammer- Team!!!", Bubbles.Normal, target: hammer);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        speachBubble.Say("Wir sollten öfter\ngemeinsam Sachen bauen", Bubbles.Normal, target: hammer);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        speachBubble.Say("Falls ihr wieder unsere Hilfe\nbraucht, dann findet ihr uns", Bubbles.Normal, target: screwdriver);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        speachBubble.Say("im Werkzeugkasten\nam anderen Ende der Stadt.", Bubbles.Normal, target: screwdriver);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        MoveTools(true);
        //StartMoveTools();
        anim.Play("Leave");

        StartCoroutine(ClearGame());

        yield break;
    }

    IEnumerator BuildLose()
    {
        cScript.offset = Vector2.zero;
        Vector3 pos_ham = hammer.transform.position;
        Vector3 pos_screw = screwdriver.transform.position;
        Vector3 pos_saw = saw.transform.position;
        //anim.Play("Empty");
        anim.enabled = false;
        //yield return new WaitForSeconds(.1f);
        hammer.SetActive(true);
        screwdriver.SetActive(true);
        saw.SetActive(true);
        hammer.transform.position = pos_ham;
        screwdriver.transform.position = pos_screw;
        saw.transform.position = pos_saw;

        StartCoroutine(ToolFall(hammer, new Vector3(-8, -2.5f)));
        StartCoroutine(ToolFall(screwdriver, new Vector3(6.4f, -1.9f)));
        StartCoroutine(ToolFall(saw, new Vector3(-3.3f, -.6f)));
        yield return new WaitForSeconds(.5f);
        anim_comp.Play("JumpScare");
        yield return new WaitForSeconds(.1f);
        anim_player.Play("JumpScare");
        yield return new WaitForSeconds(1);

        anim_screw.Play("lookInPain");
        yield return new WaitForSeconds(.1f);
        anim_ham.Play("lookSad");
        yield return new WaitForSeconds(.1f);
        anim_saw.Play("lookSad");

        anim_player.Play("lookSad");
        yield return new WaitForSeconds(.1f);
        anim_comp.Play("lookSad");

        yield return new WaitUntil(() => speachBubble.finished);
        yield return new WaitForSeconds(.5f);
        while(Input.touchCount == 0)
        {
            switch(Random.Range(0, 3))
            {
                case 0: speachBubble.Say(Random.Range(0, 2) == 0? "Aua!" : "Ahhh!", Bubbles.Normal, speed: .25f, target: hammer, autoPlay: true); break;
                case 1: speachBubble.Say(Random.Range(0, 2) == 0 ? "Diese Schmerzen!" : "Ruft einen\nDoktor!", Bubbles.Normal, speed: .25f, target: screwdriver, autoPlay: true); break;
                case 2: speachBubble.Say(Random.Range(0, 2) == 0 ? "Hilfe!" : "Ich kann meine Beine\nnicht mehr spüren!!!", Bubbles.Normal, speed: .25f, target: saw, autoPlay: true); break;
            }
            for (float i = Random.Range(3, 5); i > 0 && Input.touchCount == 0; i -= Time.fixedDeltaTime) yield return new WaitForFixedUpdate();
        }

        saw.transform.rotation = Quaternion.identity;
        anim_saw.Play("smallJump");
        yield return new WaitForSeconds(.1f);
        hammer.transform.rotation = Quaternion.identity;
        anim_ham.Play("smallJump");
        yield return new WaitForSeconds(.1f);
        screwdriver.transform.rotation = Quaternion.identity;
        anim_screw.Play("smallJump");
        yield return new WaitForSeconds(.1f);

        anim_comp.Play("lookNormal");
        yield return new WaitForSeconds(.1f);
        anim_player.Play("lookNormal");


        speachBubble.Say("...", Bubbles.Normal, target: hammer);
        speachBubble.Say("Wollen wir es\nnochmal probieren?", Bubbles.Normal, target: hammer);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim.enabled = true;
        anim.Play("Stay");

        //Treffe Entscheidung:
        menu.SetDecisionActive(true);
        yield return new WaitUntil(() => menu.decisionMade);
        menu.SetDecisionActive(false);
        yield return new WaitForSeconds(1);
        //speachBubble.Clear = true;
        //yield return new WaitForSeconds(1);

        if (menu.useLeftDecision)//Go Home:
        {
            anim_ham.Play("lookHappy");
            speachBubble.Say("Alles Klar!\nWenn du es später\nnochmals probieren\n möchtest", Bubbles.Normal, target: hammer);
            speachBubble.Say("Dann kommen wir wieder\nvorbei und helfen dir.", Bubbles.Normal, target: hammer);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);

            anim_saw.Play("smallJump");
            speachBubble.Say("FEIERABEND", Bubbles.Shouting, target: saw, fontSize: 35);
            yield return new WaitForSeconds(.2f);
            anim_player.Play("JumpScare");
            yield return new WaitForSeconds(.1f);
            anim_comp.Play("JumpScare");
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);
            anim_saw.Play("lookHappy");
            speachBubble.Say("Yay!", Bubbles.Normal, target: hammer);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);
            anim_screw.Play("lookSad");
            speachBubble.Say("Moment, sind wir jetzt\neinmal für nichts\nquer durch die\nStadt gelaufen?", Bubbles.Normal, target: screwdriver);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);

            MoveTools(true);
            //StartMoveTools();
            anim.Play("Leave");

            cScript.target = anim_farmer.transform.parent.gameObject;
            StartCoroutine(cScript.SetZoom(3));
            cScript.offset = Vector2.up * 2;
            yield return new WaitForSeconds(2);

            anim_farmer.Play("JumpScare");
            speachBubble.Say("Halt, Stop!!!\nMeine Mühle ist noch\nnicht repariert!", Bubbles.Normal, target: anim_farmer.transform.parent.gameObject);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);
            yield return new WaitForSeconds(.5f);
            anim_farmer.Play("lookSad");
            yield return new WaitForSeconds(.5f);
            speachBubble.Say("Mein schöne Mühle...", Bubbles.Normal, target: anim_farmer.transform.parent.gameObject);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);

            cScript.target = gameObject;
            StartCoroutine(cScript.SetZoom(6));
            cScript.offset = Vector2.zero;
            yield return new WaitForSeconds(2);

            anim_farmer.Play("lookNormal");

            anim_comp.Play("lookNormal");
            speachBubble.Say("...Und weg sind sie...", Bubbles.Normal, target: companion);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);

            StartCoroutine(ClearGame());
        }
        else
        {
            speachBubble.Say("Ok,Bereit?\nLos gehts!", Bubbles.Normal, target: hammer);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);

            StartCoroutine(MillGame());
        }

        yield break;
    }

    void StartMoveTools()
    {
        anim_ham.SetTrigger("startMove");
        anim_screw.SetTrigger("startMove");
        anim_saw.SetTrigger("startMove");
    }
    void MoveTools(bool moving)
    {
        /*
        anim_ham.SetBool("moving", moving);
        anim_screw.SetBool("moving", moving);
        anim_saw.SetBool("moving", moving);
        */
        if (moving)
        {
            anim_ham.Play("Move");
            anim_screw.Play("Move");
            anim_saw.Play("Move");
        }
        else
        {
            anim_ham.Play("Idle");
            anim_screw.Play("Idle");
            anim_saw.Play("Idle");
        }
    }
    IEnumerator ToolFall(GameObject tool, Vector3 goalPosition)
    {
        Vector3 diff = goalPosition - tool.transform.localPosition;
        float x_step = diff.x * Time.fixedDeltaTime;
        Vector3 scale_step = Vector3.one * (1 - tool.transform.GetChild(0).localScale.x) * Time.fixedDeltaTime;
        float rotation = Random.Range(-2, 2);
        yield return new WaitForSeconds(Random.Range(0, .5f));

        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            tool.transform.GetChild(0).localScale += scale_step;
            tool.transform.localPosition += Vector3.right * x_step;
            tool.transform.localPosition = new Vector3(tool.transform.localPosition.x, goalPosition.y - diff.y * fallCurve.Evaluate(count));
            tool.transform.Rotate(Vector3.forward, rotation);
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }


    public void OnButtonPressed() => buttonHold = true;
    public void OnButtonReleased() => buttonReleased = true;

    public void SetButtonHold() => buttonHold = true;

    private void OnDrawGizmosSelected()
    {
        foreach(Vector2 pos in touchPointPos)
            Gizmos.DrawWireSphere(transform.position + (Vector3)pos, 1.5f);
    }
}
