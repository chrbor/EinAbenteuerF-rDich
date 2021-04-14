using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;
using static CameraScript;
using static BubbleScript;
using static MenuScript;

public class Seq_GameStart : Sequence
{
    public AudioClip sleepClip, boingSound;
    private Animator anim;

    public AnimationCurve curve;
    public Material transition;

    bool stopSound;


    protected void Awake() => SceneManager.sceneLoaded += OnLevelLoaded;
    protected void OnLevelLoaded(Scene scene, LoadSceneMode mode) => StartCoroutine(PlaySequence());
    private void OnDestroy() => SceneManager.sceneLoaded -= OnLevelLoaded;

    protected override IEnumerator PlaySequence()
    {
        Debug.Log("Start Sequence");
        cScript.aSrc.Stop();
        canvas.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        yield return new WaitForSeconds(.1f);
        StartCoroutine(Setup());
        anim = GetComponent<Animator>();
        player.transform.parent = transform;
        companion.transform.parent = transform;
        anim.Rebind();

        yield return new WaitForSeconds(.1f);
        anim.SetTrigger("next");//sleep

        yield return new WaitForSeconds(.15f);
        anim_player.Play("sleeping");

        cScript.target = gameObject;
        cScript.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
        cScript.offset = player.transform.localPosition;
        cScript.strength = .03f;
        StartCoroutine(cScript.SetZoom(3));

        yield return new WaitForSeconds(2);
        cScript.aSrc.clip = sleepClip;
        cScript.aSrc.Play();

        yield return new WaitForSeconds(7);
        anim.SetTrigger("next");//bedjump-dark
        StartCoroutine(cScript.SetBGM(boingSound));
        cScript.aSrc.loop = false;
        yield return new WaitForSeconds(3f);

        StartCoroutine(StartSound());
        speachBubble.Say("Wach auf,\nwach auf\nwach auf\nwach auf", Bubbles.Shouting, fontSize: 30);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        yield return new WaitForSeconds(1);

        anim.SetTrigger("next");//transition to bedjump
        yield return new WaitForSeconds(2);
        speachBubble.Say("Aufwachen!", Bubbles.Shouting, fontSize: 30);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        stopSound = true;
        cScript.aSrc.loop = true;
        yield return new WaitForSeconds(.1f);
        manager.PlayNormal();
        anim.SetTrigger("next");

        yield return new WaitForSeconds(3);
        anim_comp.Play("lookHappy");

        cScript.offset += new Vector2(-4, 0);
        StartCoroutine(cScript.SetZoom(5, 2));
        yield return new WaitForSeconds(1);
        anim_player.Play("lookNormal");
        yield return new WaitForSeconds(1);

        speachBubble.Say("Heute ist unser\nGeburtstag!", Bubbles.Normal, target: companion, fontSize: 30);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_comp.Play("lookNormal");
        speachBubble.Say("Ob Mama schon einen\nKuchen gebacken hat?", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_comp.Play("lookHappy");
        speachBubble.Say("Lass uns schnell\nnachschauen!", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("lookNormal");

        anim.SetTrigger("next");
        StartCoroutine(cScript.SetZoom(7, 5));
        Vector2 step = Vector2.down * Time.fixedDeltaTime * 4.2f;
        for (float count = 0; count < 5; count += Time.fixedDeltaTime)
        {
            cScript.offset += step;
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(2);

        GameObject mom = GameObject.Find("Mom");
        Animator anim_mom = mom.transform.GetChild(0).GetComponent<Animator>();

        anim_mom.Play("lookHappy");
        speachBubble.Say("Alles gute meine Lieben!", Bubbles.Normal, target: mom);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_mom.Play("lookNormal");
        speachBubble.Say("Seit ihr bitte so lieb\nund könnt schnell diese\nSachen aus dem\nSupermarkt holen?", Bubbles.Normal, target: mom);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_mom.Play("Squeesh");
        StartCoroutine(GiveObject(transform.GetChild(1).gameObject, mom, player));

        anim_comp.Play("lookAngry");
        speachBubble.Say("Aber Mom!\nHeute ist unser\nGeburtstag!", Bubbles.Normal, target: companion);
        speachBubble.Say("Müssen gerade wir\ndie Sachen holen?", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_mom.Play("lookSad");
        speachBubble.Say("Ohne die Zutaten kann\nich keinen\nKartoffelkuchen\nmachen", Bubbles.Normal, target: mom);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("smallJump");
        speachBubble.Say("Warum kann Papa\ndie Sachen nicht\nholen?", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        cScript.offset += Vector2.left * 5;
        yield return new WaitForSeconds(1);

        anim_player.Play("lookAnnoyed");
        anim_comp.Play("lookAnnoyed");
        anim_mom.Play("lookAnnoyed");

        GameObject dad = GameObject.Find("Dad");
        Animator anim_dad = dad.transform.GetChild(0).GetComponent<Animator>();
        dad.GetComponent<NPCScript>().enabled = false;
        anim_dad.Play("Squeesh");
        speachBubble.Say("Tut, was eure\nMutter euch sagt", Bubbles.Normal, target: dad);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        cScript.offset -= Vector2.left * 5;
        yield return new WaitForSeconds(1);
        dad.GetComponent<NPCScript>().enabled = true;
        anim_dad.SetTrigger("startMove");

        speachBubble.Say("Da habt ihrs!", Bubbles.Normal, target: mom);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_mom.Play("lookHappy");
        speachBubble.Say("Seht es als\nkleines Abenteuer\noder so...", Bubbles.Normal, target: mom);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_mom.Play("Squeesh");
        for (int i = 1; i < 5; i++) { StartCoroutine(GiveObject(transform.GetChild(i).gameObject, mom, player)); yield return new WaitForSeconds(.2f); }
        speachBubble.Say("Hier, ich gebe euch\nsogar etwas mehr\nGeld mit", Bubbles.Normal, target: mom);
        speachBubble.Say("Damit ihr euch eine\nkleine Süßigkeit\nkaufen könnt", Bubbles.Normal, target: mom);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("lookAngry");
        yield return new WaitForSeconds(.1f);
        anim_player.Play("looKNormal");
        speachBubble.Say("Na gut!\nAber nur, weil der\nKartoffelkuchen so\nlecker ist!", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim.SetTrigger("next");

        menu.SetTransition(transition);
        menu.DoTransition(1, 1, false);
        Mat_Intro = transition;
        yield return new WaitForSeconds(2.4f);
        SceneManager.LoadScene("VoiceScene");
        Destroy(gameObject);
        yield break;
    }

    IEnumerator StartSound()
    {
        stopSound = false;
        while(!stopSound)
        {
            cScript.aSrc.Stop();
            cScript.aSrc.Play();
            for (float count = 0; count < .5f; count += Time.fixedDeltaTime) { if (stopSound) break; yield return new WaitForFixedUpdate(); }
        }
        cScript.aSrc.Stop();
        yield break;
    }

    IEnumerator GiveObject(GameObject item, GameObject giver, GameObject taker)
    {
        SpriteRenderer sprite = item.GetComponent<SpriteRenderer>();
        sprite.color = new Color(1, 1, 1, 0);
        item.SetActive(true);

        //Zeige Item
        item.transform.position = giver.transform.position + Vector3.up * 5;

        float timeStep = Time.fixedDeltaTime;
        Vector3 posStep = Vector3.up * 2 * timeStep;
        Color colorStep = Color.black * timeStep;
        for(float count = 0; count < 1; count += timeStep)
        {
            sprite.color += colorStep;
            item.transform.position += posStep;
            yield return new WaitForFixedUpdate();
        }
        posStep *= -.5f;
        for (float count = 0; count < 1; count += timeStep)
        {
            item.transform.position += posStep;
            yield return new WaitForFixedUpdate();
        }

        //Bewege dich zum Nehmer:
        item.transform.parent = taker.transform;
        posStep = (-item.transform.localPosition + Vector3.up * 5) * timeStep;
        Vector3 virtPos = item.transform.localPosition;
        for (float count = 0; count < 1; count += timeStep)
        {
            virtPos += posStep;
            item.transform.localPosition = virtPos + curve.Evaluate(count) * Vector3.up;
            yield return new WaitForFixedUpdate();
        }


        posStep = Vector3.down * timeStep;
        for (float count = 0; count < 1; count += timeStep)
        {
            sprite.color -= colorStep;
            item.transform.localPosition += posStep;
            yield return new WaitForFixedUpdate();
        }

        Destroy(item);
        yield break;
    }
}
