using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameManager;
using static CameraScript;
using static BubbleScript;

public class Seq_postKnock : Sequence
{
    protected void Awake() => SceneManager.sceneLoaded += OnLevelLoaded;
    private void OnDestroy() => SceneManager.sceneLoaded -= OnLevelLoaded;

    protected void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(PlaySequence());
    }


    protected override IEnumerator PlaySequence()
    {
        anim_player = player.transform.GetChild(0).GetComponent<Animator>();
        anim_comp = companion.transform.GetChild(0).GetComponent<Animator>();

        runGame = true;
        pauseMove = true;
        staticCam = true;
        TasklistScript.SetTaskList(false);
        cScript.target = player;
        cScript.offset = new Vector2(-4, 2);
        cScript.transform.position = player.transform.position + new Vector3(0, 2, -10);
        player.transform.position = new Vector3(-175, -2);
        companion.transform.position = new Vector3(-172, -2);
        StartCoroutine(cScript.SetRotation());

        player.GetComponent<PlayerScript>().enabled = false;
        companion.GetComponent<FollowScript>().enabled = false;
        yield return new WaitForSeconds(.1f);
        anim_player.SetBool("moving", false);
        anim_player.SetBool("inAir", false);


        //Saw, Hammer und Screwdriver kommen raus:
        GameObject ham = transform.GetChild(0).gameObject;
        GameObject saw = transform.GetChild(1).gameObject;
        GameObject screwdriver = transform.GetChild(2).gameObject;
        Animator anim_ham = ham.transform.GetChild(0).GetComponent<Animator>();
        Animator anim_saw = saw.transform.GetChild(0).GetComponent<Animator>();
        Animator anim_screw = screwdriver.transform.GetChild(0).GetComponent<Animator>();

        screwdriver.transform.position = new Vector3(-191.5f, -2.5f);
        screwdriver.SetActive(true);
        anim_screw.Play("toolAppear");
        yield return new WaitForSeconds(.5f);
        StartCoroutine(MoveTool(screwdriver, -182));

        saw.transform.position = new Vector3(-191.5f, -2.5f);
        saw.SetActive(true);
        anim_saw.Play("toolAppear");
        yield return new WaitForSeconds(.5f);
        StartCoroutine(MoveTool(saw, -185));

        if(progress.millState % 3 == 1)
        {
            ham.transform.position = new Vector3(-191.5f, -2.5f);
            ham.SetActive(true);
            anim_ham.Play("toolAppear");
            yield return new WaitForSeconds(.5f);
            StartCoroutine(MoveTool(ham, -189));
            yield return new WaitForSeconds(1);
        }
        else
        {
            yield return new WaitForSeconds(1);
            anim_screw.Play("JumpScare");
            speachBubble.Say("Ist die Mühle\nwieder kaputt?", Bubbles.Normal, target: screwdriver);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);

            anim_comp.Play("lookHappy");
            speachBubble.Say("Nö, wir wollten\nnur mal Hallo sagen", Bubbles.Normal, target: companion);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);

            anim_screw.Play("lookAnnoyed");
            yield return new WaitForSeconds(.1f);
            anim_saw.Play("lookAnnoyed");

            speachBubble.Say("...Du machst mir\nKongurenz!", Bubbles.Normal, target: saw);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);

            anim_screw.Play("Squeesh");
            speachBubble.Say("Wir sind gerade\ndabei die Stadt\nauszubauen", Bubbles.Normal, target: screwdriver);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);
            speachBubble.Say("Da haben wir leider\nnicht so viel Zeit\nzum plaudern", Bubbles.Normal, target: screwdriver);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);

            yield return new WaitForSeconds(.5f);
            StartCoroutine(MoveTool(saw, -191.5f, 1, false));
            yield return new WaitForSeconds(.5f);
            StartCoroutine(MoveTool(screwdriver, -191.5f, 1, false));
            yield return new WaitForSeconds(2);

            EndSequence();
            yield break;
        }



        anim_screw.Play("Squeesh");
        speachBubble.Say("Hallo, ich bin\n Schraub der\nSchraubendreher", Bubbles.Normal, target: screwdriver);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        speachBubble.Say("...Das hier ist\ndie Säge...", Bubbles.Normal, target: screwdriver);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_saw.Play("smallJump");
        speachBubble.Say("...Und er ist der Hammer!", Bubbles.Normal, target: screwdriver);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_ham.Play("Squeesh");
        speachBubble.Say("Wir sind die\nGuilde der\nHandwerker", Bubbles.Normal, target: screwdriver);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("JumpScare");
        speachBubble.Say("Hey Moment mal!\nHaben wir nicht den\nHammer gerade eben erst\nam gebauten Haus\ngesehen?", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("JumpScare");
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(1);

        anim_ham.Play("lookHappy");
        speachBubble.Say("Ich bin halt\nder Hammer!", Bubbles.Normal, target: ham);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_screw.Play("lookAnnoyed");
        speachBubble.Say("Es war gerade\nMittagspause\nDa ist er besonders schnell\nwieder zurück", Bubbles.Normal, target: screwdriver);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_ham.Play("lookFocused");
        speachBubble.Say("Ich habe halt\neinen hammermäßigen\nHunger!", Bubbles.Normal, target: ham);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_ham.Play("lookNormal");
        yield return new WaitForSeconds(.1f);
        anim_screw.Play("smallJump");
        speachBubble.Say("Ihr müsst die\nneuen Mitglieder der\nGuilde sein", Bubbles.Normal, target: screwdriver);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        speachBubble.Say("Nur Mitglieder kennen\n das geheime\nKlopfzeichen", Bubbles.Normal, target: screwdriver);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("Squeesh");
        yield return new WaitForSeconds(.75f);
        anim_comp.Play("lookInPain");
        speachBubble.Say("Ähm, so ist es!", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookAnnoyed");
        player.GetComponent<VoiceScript>().PlayMumble();
        yield return new WaitForSeconds(1);

        speachBubble.Say("Wir haben auf\nden Weg hierhin\ngesehen das die Mühle\nzerstört ist.", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        speachBubble.Say("Vielleicht sollten wir\nsie als erstes wieder\naufbauen.", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookNormal");
        anim_comp.Play("lookNormal");

        anim_screw.Play("JumpScare");
        yield return new WaitForSeconds(.1f);
        anim_saw.Play("JumpScare");
        yield return new WaitForSeconds(.1f);
        anim_ham.Play("JumpScare");

        speachBubble.Say("DIE MÜHLE IST\nZERSTÖRT!?", Bubbles.Shouting, fontSize: 35);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_ham.Play("lookSad");
        speachBubble.Say("Das hört sich nach\neinem Notfall an", Bubbles.Normal, target: ham);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_saw.Play("smallJump");
        speachBubble.Say("Da müssen wir einen\nAntrag auf Erneuerung\nder Mühle stellen", Bubbles.Normal, target: saw);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_screw.Play("lookAnnoyed");
        anim_ham.Play("lookAnnoyed");
        speachBubble.Say("Ein Befunddatenblatt\nausarbeiten", Bubbles.Normal, target: saw);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_comp.Play("lookAnnoyed");
        speachBubble.Say("Ein Schadbildkatalog\nerstellen", Bubbles.Normal, target: saw);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_player.Play("lookAnnoyed");
        speachBubble.Say("Eine...", Bubbles.Normal, target: saw, speed: .2f, autoPlay: true);
        yield return new WaitForSeconds(1);

        anim_screw.Play("lookAngry");
        speachBubble.Say("Wie wäre es, wenn\nwir uns erstmal\ndie Mühle anschauen?", Bubbles.Normal, target: screwdriver);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_saw.Play("Squeesh");
        speachBubble.Say("Eine Bestandsaufnahme\nmachen, ja", Bubbles.Normal, target: saw);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        speachBubble.Say("(Was für eine\nNervensäge...)", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_screw.Play("lookAnnoyed");
        anim_ham.Play("smallJump");
        anim_comp.Play("lookNormal");
        yield return new WaitForSeconds(.1f);
        anim_player.Play("lookNormal");
        speachBubble.Say("Wir treffen uns\ndann bei der Mühle", Bubbles.Normal, target: ham);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        yield return new WaitForSeconds(.5f);
        StartCoroutine(MoveTool(screwdriver, -120, 5, false));
        yield return new WaitForSeconds(.5f);
        StartCoroutine(MoveTool(saw, -120, 5, false));
        yield return new WaitForSeconds(.5f);
        StartCoroutine(MoveTool(ham, -120, 5, false));

        yield return new WaitForSeconds(3);


        anim_comp.Play("lookHappy");
        speachBubble.Say("Zum Glück\nhaben wir jemanden\ngefunden, der uns mit der\nMühle helfen kann", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        speachBubble.Say("Jetzt können wir sie\nendlich reparieren damit\nwir das Mehl\nbekommen", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookHappy");
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(1);

        EndSequence();

        yield break;
    }

    IEnumerator MoveTool(GameObject tool, float x_goal, float time = 1, bool stay = true)
    {
        Animator anim = tool.transform.GetChild(0).GetComponent<Animator>();

        anim.SetBool("moving", true);
        anim.SetTrigger("startMove");
        Vector3 x_step = Vector3.right * (x_goal - tool.transform.position.x) * Time.fixedDeltaTime / time;
        for(float count = 0; count < time; count += Time.fixedDeltaTime)
        {
            tool.transform.position += x_step;
            yield return new WaitForFixedUpdate();
        }
        anim.SetBool("moving", false);
        if(!stay) anim.Play("toolDisappear");
        yield break;
    }
}
