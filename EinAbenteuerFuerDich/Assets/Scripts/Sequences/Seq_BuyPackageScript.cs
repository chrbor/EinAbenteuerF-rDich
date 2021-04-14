using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static MenuScript;
using static GameManager;
using static CameraScript;
using static BubbleScript;

public class Seq_BuyPackageScript : MonoBehaviour
{
    private GameObject camFocus;
    private GameObject guy; 
    public GameObject box;

    public AudioClip music;
    public AnimationCurve curve;

    private void OnTriggerEnter2D(Collider2D other)
    {
        runGame = true;
        pauseMove = true;
        staticCam = true;
        camFocus = transform.GetChild(0).gameObject;
        cScript.target = camFocus;
        StartCoroutine(RunSequence());
    }
    /*
    private void Start()
    {
        runGame = true;
        pauseMove = true;
        staticCam = true;
        camFocus = transform.GetChild(0).gameObject;
        cScript.target = camFocus;
        StartCoroutine(RunSequence());
    }
    //*/
    IEnumerator RunSequence()
    {
        yield return new WaitForFixedUpdate();
        StartCoroutine(TasklistScript.SetTaskList(false));
        GameObject guy = transform.GetChild(1).gameObject;
        Animator anim_player = player.transform.GetChild(0).GetComponent<Animator>();
        Animator anim_comp = companion.transform.GetChild(0).GetComponent<Animator>();

        StartCoroutine(cScript.SetRotation());
        StartCoroutine(cScript.SetZoom(5));
        StartCoroutine(cScript.SetBGM(music));
        cScript.offset = Vector2.zero;
        camFocus.transform.localPosition = new Vector3(7, 3);
        yield return new WaitForSeconds(2);

        companion.GetComponent<FollowScript>().enabled = false;
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        yield return new WaitForSeconds(.1f);
        player.transform.GetChild(0).GetComponent<Animator>().SetBool("inAir", false);

        anim_comp.SetBool("moving", true);
        anim_comp.SetTrigger("startMove");
        while(companion.transform.position.x - player.transform.position.x < 4)
        {
            companion.transform.position += Vector3.right * .1f;
            yield return new WaitForFixedUpdate();
        }
        anim_comp.SetBool("moving", false);


        /* Guy redet: 
         * Hey! Hey du da! Wie heißt du? 
         * 
         * Hey, ich mein dich! Wie heißt du?
         * 
         * Sag schon, wie heißt du?
        */
        speachBubble.Say("Hey! Hey du da!\nWie heißt du?", Bubbles.Normal, target: guy);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("lookScared");

        AudioSource aSrc_player = player.GetComponent<AudioSource>();
        menu.useLeftDecision = true;
        while(menu.useLeftDecision)
        {
            canvas.transform.GetChild(1).gameObject.SetActive(true);
            StartCoroutine(VoiceScript.GetSoundSample(triggerThresh: 1e-5f));
            yield return new WaitUntil(()=>VoiceScript.fetching);
            while (VoiceScript.fetching)
            {
                float waittime = Random.Range(10, 15);
                for(float count = 0; count < waittime && VoiceScript.fetching; count += Time.fixedDeltaTime) yield return new WaitForFixedUpdate();
                if (!VoiceScript.fetching) continue;
                //VoiceScript.pauseFetching = true;
                //Guy fragt noch mal:
                switch(Random.Range(0, 3))
                {
                    case 0: speachBubble.Say("Hey! Hey du da!\nWie heißt du?", Bubbles.Normal, target: guy, speed: .25f, autoPlay: true); break;
                    case 1: speachBubble.Say("Hey, ich mein dich!\nWie heißt du?", Bubbles.Normal, target: guy, speed: .25f, autoPlay: true); break;
                    case 2: speachBubble.Say("Sag schon,\nwie heißt du?", Bubbles.Normal, target: guy, speed: .25f, autoPlay: true); break;
                }
                //VoiceScript.pauseFetching = false;
            }
            canvas.transform.GetChild(1).gameObject.SetActive(false);
            speachBubble.Say("Wie war das?\nDu heißt <...> ?", Bubbles.Normal, target: guy);
            yield return new WaitForSeconds(.3f);
            aSrc_player.Play();
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);

            yield return new WaitWhile(() => aSrc_player.isPlaying);
            yield return new WaitForSeconds(.5f);

            menu.SetDecisionActive(true);
            yield return new WaitForSeconds(.1f);
            yield return new WaitUntil(() => menu.decisionMade);
            yield return new WaitForSeconds(1.5f);
        }
        playerCry.data = new float[VoiceScript.fetchedDataLength];
        aSrc_player.clip.GetData(playerCry.data, 0);
        LoadSave.SaveSound();


        speachBubble.Say("Ein sehr\nausgefallener Name.\n", Bubbles.Normal, target: guy);
        speachBubble.Say("Ich habe gehört\ndu hast heute\nGeburtstag", Bubbles.Normal, target: guy);
        speachBubble.Say("Ich habe hier quasi\nein Geburtstags-\ngeschenk für dich", Bubbles.Normal, target: guy);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_comp.Play("lookNormal");
        speachBubble.Say("Eine geheime Kiste mit\ngeheimen Sachen drinnen", Bubbles.Normal, target: guy);
        speachBubble.Say("Für nur 15 Schmankerl\ngehört sie dir", Bubbles.Normal, target: guy);
        speachBubble.Say("Na, was ist?\nSind wir im Geschäft?", Bubbles.Normal, target: guy);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        yield return new WaitForSeconds(1.5f);

        anim_comp.Play("lookPuzzled");
        speachBubble.Say("Ich frage mich,\nwas in der Box\ndrinnen ist", Bubbles.Normal, target: companion);
        speachBubble.Say("Vielleicht wird\nja aus dem Einkauf\ndoch noch ein\nAbenteuer draus", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        yield return new WaitForSeconds(1);
        anim_comp.Play("Squeesh");
        speachBubble.Say("Lass uns die\nKiste kaufen", Bubbles.Normal, target: companion);
        speachBubble.Say("Schließlich hat\nMama uns erlaubt,\ndass wir uns etwas\neigenes holen dürfen", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        yield return new WaitForSeconds(1);

        aSrc_player.GetComponent<VoiceScript>().PlayShout();
        player.transform.GetChild(0).GetComponent<Animator>().Play("Squeesh");
        for (int i = 2; i < 5; i++) { StartCoroutine(GiveObject(transform.GetChild(i).gameObject, player, guy)); yield return new WaitForSeconds(.5f); }

        speachBubble.Say("Verkauft!", Bubbles.Normal, target: guy);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        yield return new WaitForSeconds(1);

        StartCoroutine(cScript.SetZoom(8, 2));

        GameObject myBox = Instantiate(box, guy.transform.position + Vector3.up * 5, Quaternion.identity);
        myBox.GetComponent<Rigidbody2D>().velocity = new Vector2(-3, 10);

        SpriteRenderer sprite_myBox = myBox.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
        sprite_myBox.color = new Color(1, 1, 1, 0);
        for(float count = 0; count < 1; count += Time.fixedDeltaTime) { sprite_myBox.color = new Color(1, 1, 1, count); yield return new WaitForFixedUpdate(); }

        yield return new WaitForSeconds(1);

        runGame = false;
        staticCam = false;
        pauseMove = false;
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        anim_player.SetTrigger("startMove");
        anim_comp.SetTrigger("startMove");
        companion.GetComponent<FollowScript>().enabled = true;
        cScript.target = player;
        Destroy(this);
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
        for (float count = 0; count < 1; count += timeStep)
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
