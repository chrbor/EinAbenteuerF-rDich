using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static CameraScript;
using static MenuScript;
using static BubbleScript;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PotatoGame : MonoBehaviour
{
    public AudioClip gameMusic, winClip;
    public AccessoirSet[] accessoirSets;
    public Vector2[] potatoPos;
    public GameObject potatoPrefab;

    [Header("MoveVar:")]
    public float thresh_maxAngle;
    public float thresh_run;
    public float velocity;
    private float realVel;

    private List<GameObject> potatos = new List<GameObject>();
    [HideInInspector]
    public int potatoCount;
    public int potatoGoal;
    [HideInInspector]
    public bool digging;
    private bool isMoving;

    public static PotatoGame potatoGame;
    private bool running_game;
    public Vibration vib;
    [HideInInspector]
    public float minDist;
    int vibID, vibID_current;

    public GameObject pipeBubbles;
    public Material transition;
    public GameObject nextSeq;

    Animator anim, anim_player, anim_comp;

    void Start()
    {
        potatoGame = this;
        realVel = velocity * Time.fixedDeltaTime;

        /*
        staticCam = true;
        pauseMove = true;
        runGame = true;
        cScript.transform.rotation = Quaternion.identity;
        StartCoroutine(RunPotatoGame());
        //*/

        //Setup der Vibration:
        vib = new Vibration();
        vib.SetVibrationEffect(new long[2] { 1, 10 }, 0);
        vib.SetVibrationEffect(new long[2] { 1, 5 }, 0);
        vib.SetVibrationEffect(new long[2] { 1, 2 }, 0);
        vib.SetVibrationEffect(new long[2] { 1, 1 }, 0);
        vib.SetVibrationEffect(new long[2] { 5, 1 }, 0);
        vib.SetVibrationEffect(new long[2] { 10, 1 }, 0);
    }

    private void OnDestroy()
    {
        vib.DestroyVibration();
    }

    IEnumerator UpdateVibrator()
    {
        while (running_game)
        {
            if (minDist == 99) { vib.Cancel(); vibID_current = 44; }
            else
            {
                vibID = minDist >= 10 ? 5 : (int)(minDist / 2);

                if (vibID != vibID_current) { vib.Cancel(); vib.Vibrate(vibID); }
                vibID_current = vibID;
                minDist = 99;
            }

            yield return new WaitForSeconds(.1f);
        }
        yield break;
    }

    IEnumerator RunIntroSequence()
    {
        yield return new WaitForSeconds(.1f);
        if (pauseGame) yield break;
        staticCam = true;
        pauseMove = true;
        runGame = true;
        cScript.transform.rotation = Quaternion.identity;

        yield return new WaitForSeconds(.1f); 
        player.GetComponent<PlayerScript>().enabled = false;
        yield return new WaitForSeconds(.1f);
        anim = GetComponent<Animator>();
        anim_player = player.transform.GetChild(0).GetComponent<Animator>();
        anim_comp = companion.transform.GetChild(0).GetComponent<Animator>();
        GameObject farmer = transform.GetChild(1).gameObject;
        Animator anim_farmer = farmer.transform.GetChild(0).GetComponent<Animator>();
        anim_player.SetBool("inAir", false);

        //Nehme Liste raus:
        StartCoroutine(TasklistScript.SetTaskList(false));

        cScript.offset = new Vector2(0, 3);
        cScript.target = farmer;
        StartCoroutine(cScript.SetZoom(8, 2));
        StartCoroutine(cScript.SetRotation(rotTime: 2));

        anim_farmer.Play("Squeesh");
        yield return new WaitForSeconds(1);
        anim_farmer.Play("lookAngry");
        yield return new WaitForSeconds(1);

        speachBubble.Say("Hey!!!\nIhr da!", Bubbles.Shouting, fontSize: 35);
        yield return new WaitForSeconds(.2f);
        anim_player.Play("JumpScare");
        yield return new WaitForSeconds(.1f);
        anim_comp.Play("JumpScare");
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        //player und companion bewegen sich vor den bauern:
        anim_comp.SetBool("moving", true);
        anim_comp.SetTrigger("startMove");
        StartCoroutine(MoveCompanion());

        anim_player.SetBool("moving", true);
        anim_player.SetTrigger("startMove");
        float moveSpeed = 0.05f  * Mathf.Sign(235 - player.transform.position.x);
        while(Mathf.Abs(player.transform.position.x - 235 ) > Mathf.Abs(moveSpeed) )
        {
            player.transform.position += Vector3.right * moveSpeed;
            yield return new WaitForFixedUpdate();
        }
        anim_player.SetBool("moving", false);
        anim_comp.SetBool("moving", false);
        yield return new WaitForSeconds(1f);

        speachBubble.Say("Habt ihr meine\nKartoffeln geklaut?", Bubbles.Shouting, target: farmer);
        anim_comp.Play("lookScared");
        anim_player.Play("lookSad");
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        speachBubble.Say("Meine schöne\nKartoffelernte ist\nwie vom Erdboden\nverschluckt", Bubbles.Shouting, target: farmer);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_farmer.Play("lookFurious");
        speachBubble.Say("Ihr habt doch\nnichts damit\nzu tun", Bubbles.Shouting, target: farmer);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        speachBubble.Say("ODER???", Bubbles.Shouting, fontSize: 35);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("Squeesh");
        player.GetComponent<VoiceScript>().PlayQuestion();
        yield return new WaitForSeconds(.5f);
        anim_player.Play("lookSad");
        yield return new WaitForSeconds(1);

        anim_farmer.Play("lookPuzzled");
        speachBubble.Say("Wie war das?\nIhr seit auch auf\nder Suche nach\nKartoffeln?", Bubbles.Normal, target: farmer);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim.Play("potatoHeist1");
        anim_farmer.Play("lookAnnoyed");
        speachBubble.Say("Nun, hier werdet\nihr keine finden!", Bubbles.Normal, target: farmer);
        speachBubble.Say("Die wenigen\nKartoffeln, die übrig\nsind reichen kaum\nnoch für mein Mittag!", Bubbles.Normal, target: farmer);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_comp.Play("lookSad");
        anim_farmer.Play("lookFocused");
        yield return new WaitForSeconds(2);
        anim_farmer.Play("Squeesh");
        GameObject bulb = transform.GetChild(2).gameObject;
        bulb.SetActive(true);
        bulb.transform.position = farmer.transform.position + Vector3.up;
        bulb.GetComponent<Animator>().Play("Idea");
        yield return new WaitForSeconds(1);
        bulb.SetActive(false);

        anim_farmer.Play("lookNormal");
        speachBubble.Say("Ich habe eine\nIdee!", Bubbles.Normal, target: farmer);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim.Play("potatoHeist2");
        yield return new WaitForSeconds(1);

        speachBubble.Say("Wenn ihr meine\nKartoffeln\nwiederfindet,", Bubbles.Normal, target: farmer);
        speachBubble.Say("Dann gebe ich\neuch ein paar\nKartoffeln ab", Bubbles.Normal, target: farmer);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_comp.SetTrigger("blink");
        speachBubble.Say("Was haltet\nihr davon?", Bubbles.Normal, target: farmer);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("Squeesh");
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(1);

        anim_comp.Play("lookPuzzled");
        speachBubble.Say("die Kartoffeln\nwurden also\ngeklaut?", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_farmer.SetTrigger("blink");
        yield return new WaitForSeconds(1);
        speachBubble.Say("Ähm,\nja schon...", Bubbles.Normal, target: farmer);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        //Verkleide den Spieler und dessen Companion zu Detektive:
        Transform parent_companionAcc = companion.transform.GetChild(0).GetChild(0).GetChild(0);
        manager.ChangeClothes(accessoirSets[0], accessoirSets[1]);
        yield return new WaitForSeconds(2.25f);

        //Füge Wasserpfeife- Blubberblasen hinzu:
        Instantiate(pipeBubbles, parent_companionAcc.GetChild(1)).transform.localPosition = new Vector3(.5f, -.5f);

        anim_comp.Play("lookFocused");
        speachBubble.Say("Gehe ich recht in\nder Annahme, dass Sie uns\nals Privatdetektive\nanheuern wollen?", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_farmer.SetTrigger("Blink");
        yield return new WaitForSeconds(.2f);
        anim_farmer.SetTrigger("blink");
        speachBubble.Say("...Bitte was?", Bubbles.Normal, target: farmer);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_comp.Play("lookPuzzled");
        yield return new WaitForSeconds(1);
        speachBubble.Say("...Der Fall ist schwer,\naber wir sind die besten\nauf unserem Gebiet", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookHappy");
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(1);

        speachBubble.Say("Wir kümmern uns darum!", Bubbles.Normal, target: companion);
        speachBubble.Say("Betrachten sie\nden Fall als so gut\nwie gelöst.", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_farmer.SetTrigger("blink");
        yield return new WaitForSeconds(.2f);
        anim_farmer.SetTrigger("blink");
        yield return new WaitForSeconds(.2f);
        anim_farmer.SetTrigger("blink");
        speachBubble.Say("...Nun, falls ihr die\nKartoffeln findet\ndann bringt sie mir\nbitte zum Lagerhaus", Bubbles.Normal, target: farmer);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        //Player und Companion bewegen sich weg:
        cScript.target = player;
        cScript.offset = Vector2.up * 2;
        cScript.strength = .05f;

        player.transform.localScale = new Vector3(-1, 1, 1);
        companion.transform.localScale = new Vector3(-1, 1, 1);
        Vector3 step = Vector3.left * .05f;
        anim_player.SetBool("moving", true);
        anim_player.SetTrigger("startMove");
        anim_comp.SetBool("moving", true);
        anim_comp.SetTrigger("startMove");
        for (float count = 0; count < 4; count += Time.fixedDeltaTime)
        {
            player.transform.position += step;
            companion.transform.position += step;
            yield return new WaitForFixedUpdate();
        }
        anim_player.SetBool("moving", false);

        yield return new WaitForSeconds(1);
        anim_comp.Play("lookFocused");
        speachBubble.Say("...Hhhmmm...\nalle Spuren verlaufen\nim Sand.", Bubbles.Normal, target: companion);
        speachBubble.Say("Ich schätze,\nda müssen wir etwas\ntiefer graben", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("Squeesh");
        bulb.SetActive(true);
        bulb.transform.position = companion.transform.position;
        bulb.GetComponent<Animator>().Play("Idea");
        yield return new WaitForSeconds(1);
        bulb.SetActive(false);

        speachBubble.Say("In solchen Momenten\nverlässt sich der\nDetektiv", Bubbles.Normal, target: companion);
        speachBubble.Say("immer auf seinen\nsiebten Sinn!", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookSad");
        player.GetComponent<VoiceScript>().PlayQuestion();
        yield return new WaitForSeconds(1);

        anim_comp.Play("lookHappy");
        speachBubble.Say("Laufe einfach\nüber das Feld\nund grabe dort", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_player.Play("lookAnnoyed");
        speachBubble.Say("wo dein Detektiv-\nSinn am stärksten\nausschlägt", Bubbles.Normal, target: companion);
        speachBubble.Say("mache mit dem\nHandy eine Schaufel-\nbewegung, um die", Bubbles.Normal, target: companion);
        speachBubble.Say("Kartoffeln auszugraben", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_comp.SetTrigger("startMove");
        anim_comp.SetBool("moving", false);

        StartCoroutine(RunPotatoGame());
        yield break;
    }

    IEnumerator RunPotatoGame()
    {
        //Rotiere die Camera um 90°:
        StartCoroutine(cScript.SetRotation(90));
        StartCoroutine(cScript.SetZoom(8));
        StartCoroutine(menu.SetMenuRotation(_isPortrait: true));
        yield return new WaitForSeconds(1);

        Input.gyro.enabled = true;
        //yield return new WaitUntil(() => Input.gyro.gravity.x > -.4f);

        //Zeige Kartoffelcounter an:
        potatoGoal += difficulty;
        CanvasGroup potCountSprite = canvas.transform.GetChild(canvas.transform.childCount - 3).GetChild(1).GetComponent<CanvasGroup>();
        for (int i = 0; i < potCountSprite.transform.childCount; i++)
        {
            if(i >= potatoGoal) Destroy(potCountSprite.transform.GetChild(i).gameObject);
            else potCountSprite.transform.GetChild(i).GetComponent<Image>().color = new Color(.5f, .5f, .5f, .25f);
        }
        potCountSprite.gameObject.SetActive(true);
        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            potCountSprite.alpha = count;
            yield return new WaitForFixedUpdate();
        }

        //Verteile die Kartoffeln:
        foreach(var pos in potatoPos)
            potatos.Add(Instantiate(potatoPrefab, (Vector2)transform.position + pos, Quaternion.identity));

        transform.GetChild(0).gameObject.SetActive(true);

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        Animator anim = player.transform.GetChild(0).GetComponent<Animator>();
        anim.Rebind();
        cScript.target = player;
        Camera.main.orthographicSize = 8;
        Input.gyro.enabled = true;

        cScript.aSrc.clip = gameMusic;
        cScript.aSrc.loop = true;
        cScript.aSrc.Play();

        running_game = true;
        potatoCount = 0;
        //*
        StartCoroutine(UpdateVibrator());
        while (runGame && potatoCount < potatoGoal)
        {
            if (pauseGame)
            {
                StartCoroutine(cScript.SetRotation());
                yield return new WaitUntil(() => !pauseGame);
            }

            //Laufen:
            if (Input.gyro.gravity.x > -0.2f)
            {
                Camera.main.transform.rotation = Quaternion.Euler(0, 0, 90);
                anim.SetBool("moving", false);

                //Grab-Geste:
                yield return new WaitUntil(() => !runGame || Input.gyro.gravity.x < -.2f);

                digging = true;
                anim.Play("dig");
                yield return new WaitForSeconds(1f);
                anim.SetTrigger("startMove");
                digging = false;
            }
            else
            {
                float angle = Mathf.Atan(Input.gyro.gravity.y / Input.gyro.gravity.x) * Mathf.Rad2Deg;
                if (Mathf.Abs(angle) > thresh_maxAngle) angle = Mathf.Sign(angle) * thresh_maxAngle;

                float moveStrength = angle / 90;

                cScript.offset = new Vector2(moveStrength * 10, 1);
                Camera.main.transform.eulerAngles = new Vector3(0, 0, 90 - angle);

                if (Mathf.Abs(angle) > thresh_run)
                {
                    rb.position += Vector2.right * realVel * moveStrength;
                    player.transform.localScale = new Vector3(Mathf.Sign(moveStrength), 1, 1);
                    companion.transform.localScale = new Vector3(Mathf.Sign(player.transform.position.x - companion.transform.position.x), 1, 1);
                    anim.SetBool("moving", true);
                }
                else anim.SetBool("moving", false);
            }

            yield return new WaitForFixedUpdate();
        }
        StartCoroutine(cScript.SetBGM(winClip));
        cScript.aSrc.loop = false;

        progress.potatoDone = true;
        LoadSave.SaveProgress();


        //Clear Game:       
        vib.Cancel();
        transform.GetChild(0).gameObject.SetActive(false);
        //*/
        //Nehme KartoffelCounter raus:
        for (float count = 1; count > 0; count -= Time.fixedDeltaTime)
        {
            potCountSprite.alpha = count;
            yield return new WaitForFixedUpdate();
        }
        potCountSprite.gameObject.SetActive(false);

        //Rotiere die Camera um 90°:
        StartCoroutine(cScript.SetRotation(0, 2));
        StartCoroutine(cScript.SetZoom(5, 2));
        StartCoroutine(menu.SetMenuRotation(_isPortrait: false, changeTime: 2));
        cScript.offset = Vector2.up;
        yield return new WaitForSeconds(3);
        cScript.aSrc.clip = gameMusic;
        cScript.aSrc.loop = true;
        cScript.aSrc.Play();


        companion.GetComponent<FollowScript>().enabled = false;
        yield return new WaitForSeconds(.1f);

        anim_comp.SetBool("moving", true);
        Vector3 posStep = Mathf.Sign(companion.transform.position.x - player.transform.position.x) * .1f * Vector3.right;
        while (Mathf.Abs(player.transform.position.x - companion.transform.position.x) < 5)
        {
            companion.transform.position += posStep; 
            yield return new WaitForFixedUpdate();
        }
        anim_comp.SetBool("moving", false);

        anim_comp.Play("Squeesh");
        speachBubble.Say("Gute Arbeit,\nWatson!", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_comp.Play("lookFocused");
        speachBubble.Say("Es war ein\ndreckiger Job", Bubbles.Normal, target: companion);
        speachBubble.Say("Aber irgendwer\nmusste ihn halt\nmachen", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookFocused");
        player.GetComponent<VoiceScript>().PlayMumble();
        yield return new WaitForSeconds(1);

        anim_comp.Play("Squeesh");
        yield return new WaitForSeconds(.6f);
        anim_comp.Play("lookPuzzled");
        speachBubble.Say("Aber warum wurden\ndie Kartoffeln\n überhaupt wieder\nvergraben?", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookPuzzled");
        player.GetComponent<VoiceScript>().PlayQuestion();
        yield return new WaitForSeconds(1);

        anim_comp.Play("Squeesh");
        GameObject bulb = transform.GetChild(2).gameObject;
        bulb.SetActive(true);
        bulb.transform.position = companion.transform.position + Vector3.down;
        bulb.GetComponent<Animator>().Play("Idea");
        yield return new WaitForSeconds(1);
        bulb.SetActive(false);

        anim_player.Play("lookAnnoyed");
        speachBubble.Say("Es müssen Piraten\ngewesen sein!", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        player.GetComponent<VoiceScript>().PlayName();
        yield return new WaitForSeconds(1);

        anim_comp.Play("lookPuzzled");
        speachBubble.Say("Wenn Du das Unmögliche\nausgeschlossen hast,\ndann ist das,\nwas übrig bleibt,", Bubbles.Normal, target: companion);
        speachBubble.Say("die Wahrheit,\nwie unwahrscheinlich\nsie auch ist.", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_comp.Play("lookAngry");
        speachBubble.Say("Und nur Piraten\nvergraben ihre Beute", Bubbles.Shouting, fontSize: 30);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_comp.Play("lookNormal");
        speachBubble.Say("Wie auch immer", Bubbles.Normal, target: companion);
        speachBubble.Say("Lass uns erstmal\ndie Kartoffeln\nzum Lagerhaus\nzurück bringen", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("Squeesh");
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(1);
        //*/
        menu.SetTransition(transition);
        menu.DoTransition(1, 1, false);
        Instantiate(nextSeq, Vector3.zero, Quaternion.identity).transform.parent = transform;

        anim_comp.SetBool("moving", true);
        anim_player.SetBool("moving", true);
        anim_comp.SetTrigger("startMove");
        anim_player.SetTrigger("startMove");
        posStep = Vector3.right * .1f;
        for(float count = 0; count < 2; count+= Time.fixedDeltaTime)
        {
            player.transform.position += posStep;
            companion.transform.position += posStep;
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(2.5f);
        Destroy(this);
        yield break;
    }

    IEnumerator MoveCompanion()
    {
        float moveSpeed = 0.05f * Mathf.Sign(232 - companion.transform.position.x);
        while (Mathf.Abs(companion.transform.position.x - 232) > Mathf.Abs(moveSpeed))
        {
            companion.transform.position += Vector3.right * moveSpeed;
            yield return new WaitForFixedUpdate();
        }

        companion.transform.GetChild(0).GetComponent<Animator>().SetBool("moving", false);
        yield break;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.GetComponent<TouchSensor>().tipped || runGame || pauseGame || other.gameObject.layer != 14/*Touch*/) return;

        StartCoroutine(RunIntroSequence());
    }

    private void OnDrawGizmosSelected()
    {
        foreach (Vector2 pos in potatoPos)
            Gizmos.DrawWireSphere(transform.position + (Vector3)pos, 10);
    }
}
