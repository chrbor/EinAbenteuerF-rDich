using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameManager;
using static CameraScript;
using static MenuScript;
using static BubbleScript;

public class Seq_PostStart : Sequence
{
    bool touched;

    void Start() => StartCoroutine(PlaySequence());

    protected override IEnumerator PlaySequence()
    {
        runGame = true;
        StartCoroutine(Setup());

        //Setze Camera:
        cScript.target = player;
        cScript.offset = Vector2.up * 2;
        cScript.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
        Camera.main.orthographicSize = 6;

        yield return new WaitForSeconds(2);

        anim_comp.Play("lookAngry");
        speachBubble.Say("Wie soll Einkaufen\ndenn ein Abenteuer\nsein?", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_comp.Play("lookPuzzled");
        speachBubble.Say("Die sind doch bloß\nzu faul selbst\neinkaufen zu gehen!", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_comp.Play("smallJump");
        speachBubble.Say("Und dann noch diese\nEinkaufsliste!", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        //Zeige Liste:
        StartCoroutine(TasklistScript.SetTaskList(true));
        GameObject shopList = transform.GetChild(0).gameObject;
        shopList.SetActive(true);
        SpriteRenderer sprite = shopList.GetComponent<SpriteRenderer>();
        sprite.color = new Color(1, 1, 1, 0);
        Color stepColor = Color.black * Time.fixedDeltaTime;
        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            sprite.color += stepColor;
            yield return new WaitForFixedUpdate();
        }

        speachBubble.Say("Alles bis auf\ndie letzte Sache\nist klar", Bubbles.Normal, target: companion);
        anim_player.Play("lookSad");
        speachBubble.Say("Aber...", Bubbles.Normal, target: companion, speed: .1f);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_comp.Play("lookAngry");
        speachBubble.Say("Der ganze\nandere Kram!?", Bubbles.Shouting, fontSize: 35);
        speachBubble.Say("Was soll\ndas heißen?", Bubbles.Shouting, fontSize: 35);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        //Lasse Liste verschwinden:
        StartCoroutine(TasklistScript.SetTaskList(false));
        for (float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            sprite.color -= stepColor;
            yield return new WaitForFixedUpdate();
        }
        Destroy(shopList);

        anim_player.Play("lookNormal");
        anim_comp.Play("lookFocused");
        speachBubble.Say("Wenigstens dürfen\nwir uns eine\nSüßware kaufen...", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_comp.Play("lookNormal");
        speachBubble.Say("Ok, lass uns\nzum Supermarkt\ngehen", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        yield return new WaitForSeconds(4);

        anim_comp.Play("lookPuzzled");
        speachBubble.Say("Ähm, hallo?\n hast du das gehen\nverlernt,\noder was?", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        yield return new WaitForSeconds(1);

        anim_player.Play("lookSad");
        StartCoroutine(cScript.SetRotation(-10, .5f));
        StartCoroutine(cScript.SetZoom(2, .5f));
        yield return new WaitForSeconds(1);
        StartCoroutine(cScript.SetRotation(rotTime: .5f));
        StartCoroutine(cScript.SetZoom(6, .5f));
        yield return new WaitForSeconds(1);
        
        anim_comp.Play("JumpScare");
        speachBubble.Say("Moment,\nWie bitte\n???", Bubbles.Shouting, fontSize: 35);
        speachBubble.Say("Du hast\ndas gehen\nverlernt?", Bubbles.Shouting, fontSize: 35);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("lookAngry");
        speachBubble.Say("Na toll!\nUnd wie kommen\nwir jetzt bitte zum\nSupermarkt?", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        speachBubble.Say("Lies mich", Bubbles.Normal);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("JumpScare");
        yield return new WaitForSeconds(.1f);
        anim_player.Play("JumpScare");

        speachBubble.Say("Hast du auch\ndie Stimme gehört?", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookNormal");
        anim_comp.Play("lookScared");
        speachBubble.Say("...Ist...\n...ist...das...\n...ein Geist?", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        GameObject schildy = transform.GetChild(0).gameObject;
        Animator anim_schild = schildy.transform.GetChild(0).GetComponent<Animator>();
        SignScript signScript = schildy.transform.GetChild(0).GetChild(0).GetComponent<SignScript>();
        signScript.enabled = false;
        anim_schild.Play("Squeesh");
        yield return new WaitForSeconds(.75f);
        anim_schild.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        anim_schild.Play("lookNormal");
        yield return new WaitForSeconds(.5f);

        speachBubble.Say("Ich bin es bloß:\nSchildie", Bubbles.Normal, target: schildy);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("JumpScare");
        yield return new WaitForSeconds(.1f);
        anim_player.Play("JumpScare");

        speachBubble.Say("AAAHHHHH!!!", Bubbles.Shouting, fontSize: 35);
        speachBubble.Say("Ein\nMonster!", Bubbles.Shouting, fontSize: 35);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("lookScared");
        yield return new WaitForSeconds(.1f);
        anim_schild.Play("lookSad");
        speachBubble.Say("Das ist aber nicht\nsehr nett!", Bubbles.Normal, target: schildy);
        speachBubble.Say("Dabei wollte ich\neuch sogar helfen", Bubbles.Normal, target: schildy);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_schild.Play("Squeesh");
        speachBubble.Say("Wenn ihr ein Schild\nseht, dann tippt es an\num es zu lesen", Bubbles.Normal, target: schildy);
        speachBubble.Say("Es stehen mitunter\ninteressante Sachen\nauf uns Schildern", Bubbles.Normal, target: schildy);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("Squeesh");
        yield return new WaitForSeconds(.75f);
        anim_comp.Play("lookPuzzled");

        speachBubble.Say("Aber stimmen\nauch die Dinge\ndie auf den Schildern\nstehen?", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_schild.Play("lookAngry");
        speachBubble.Say("Absolut!\nBei der Ehre aller\nSchilder", Bubbles.Normal, target: schildy);
        speachBubble.Say("garantiere ich\neuch, dass alles\nauf den Schildern\nwahr ist", Bubbles.Normal, target: schildy);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_schild.Play("smallJump");
        speachBubble.Say("Nur zu:\nProbiert es\nbei mir aus!", Bubbles.Normal, target: schildy);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        //Warte darauf, dass Schildy angtippt wird und zeige dann die Nachricht:
        touched = false;
        yield return new WaitUntil(() => touched);
        GameObject sign = canvas.transform.GetChild(canvas.transform.childCount - 2).GetChild(0).gameObject;
        sign.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = signScript.text_1;
        sign.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        Canvas.ForceUpdateCanvases();

        sign.SetActive(true);
        Animator anim_sign = sign.GetComponent<Animator>();
        anim_sign.Play("ZoomIn");
        yield return new WaitForSeconds(.5f);
        yield return new WaitUntil(() => Input.touchCount == 0);
        yield return new WaitUntil(() => Input.touchCount != 0);
        yield return new WaitUntil(() => Input.touchCount == 0);
        anim_sign.Play("ZoomOut");
        yield return new WaitForSeconds(.5f);
        sign.SetActive(false);


        anim_comp.Play("lookNormal");
        speachBubble.Say("Es scheint,\nals hättest du\nnicht gelogen", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("lookSad");
        speachBubble.Say("Tut mir leid,\ndass ich dir nicht\ngeglaubt habe!", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_schild.Play("lookHappy");
        speachBubble.Say("Ich kann nicht lügen!\nEs steht mir halt\nimmer ins Gesicht\ngeschrieben", Bubbles.Normal, target: schildy);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_schild.Play("lookNormal");


        anim_comp.Play("smallJump");
        yield return new WaitForSeconds(.6f);
        anim_comp.Play("smallJump1");
        yield return new WaitForSeconds(.6f);
        anim_comp.Play("smallJump");
        yield return new WaitForSeconds(.6f);
        speachBubble.Say("Ok, jetzt aber\nauf zum\nSupermarkt", Bubbles.Normal, target: companion);
        StartCoroutine(TasklistScript.SetTaskList(true));
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        yield return new WaitForSeconds(.5f);

        signScript.enabled = true;
        schildy.transform.parent = null;
        EndSequence();
        yield break;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        touched = true;
    }
}
