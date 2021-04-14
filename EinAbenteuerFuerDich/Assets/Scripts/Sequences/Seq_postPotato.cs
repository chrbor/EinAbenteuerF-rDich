using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CameraScript;
using static GameManager;
using static MenuScript;
using static BubbleScript;

public class Seq_postPotato : Sequence
{
    public AnimationCurve flightPath;


    // Start is called before the first frame update
    void Start() => StartCoroutine(PlaySequence());

    protected override IEnumerator PlaySequence()
    {
        yield return new WaitForSeconds(2.4f);//warte, bis die Blende vollständig da ist
        StartCoroutine(Setup());
        Animator anim = GetComponent<Animator>();
        GameObject farmer = transform.parent.GetChild(1).gameObject;
        Animator anim_farmer = farmer.transform.GetChild(0).GetComponent<Animator>();

        player.transform.position = new Vector3(222, -2);
        companion.transform.position = new Vector3(218, -2);
        companion.transform.localScale = Vector3.one;
        player.transform.localScale = companion.transform.localScale;

        cScript.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
        StartCoroutine(cScript.SetRotation());
        Camera.main.orthographicSize = 8;

        manager.PlayNormal();
        menu.DoTransition(1, -1, true);//nutze vorherige transition
        yield return new WaitForSeconds(1.4f);

        //Komme an:
        anim_player.SetBool("moving", true);
        anim_comp.SetBool("moving", true);
        anim_player.SetTrigger("startMove");
        anim_comp.SetTrigger("startMove");
        Vector3 stepPos = Vector3.right * 10 * Time.fixedDeltaTime / 2;
        for(float count = 0; count < 2; count += Time.fixedDeltaTime)
        {
            companion.transform.position += stepPos;
            player.transform.position += stepPos;
            yield return new WaitForFixedUpdate();
        }
        anim_player.SetBool("moving", false);
        anim_comp.SetBool("moving", false);


        speachBubble.Say("Da sind wir wieder.", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_comp.Play("lookFocused");
        speachBubble.Say("Die Kartoffeln\nwaren gut versteckt", Bubbles.Normal, target: companion);
        speachBubble.Say("Aber irgendwann\nbringen wir immer  alles\nans Licht!", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        //wirf den Korb voller Kartoffeln auf den Boden:
        anim_comp.Play("Squeesh");
        GameObject basket = anim_comp.transform.GetChild(0).GetChild(0).GetChild(2).gameObject;
        basket.transform.parent = null;
        basket.transform.rotation = Quaternion.Euler(0,0,90);
        Vector3 startPos = basket.transform.position;
        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            basket.transform.position = startPos + new Vector3(-3 * count, flightPath.Evaluate(count) * 3);
            yield return new WaitForFixedUpdate();
        }

        anim.SetTrigger("next");//shock
        yield return new WaitForSeconds(1f);
        anim_player.Play("Squeesh");
        yield return new WaitForSeconds(.3f);
        anim_farmer.Play("JumpScare");
        speachBubble.Say("Oh nein!\nIhr habt sie\nwieder ausgegraben!", Bubbles.Shouting, fontSize: 30);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookSad");
        yield return new WaitForSeconds(.1f);
        anim_comp.Play("lookPuzzled");
        speachBubble.Say("Wie wir besprochen\nhaben", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_farmer.Play("smallJump");
        yield return new WaitForSeconds(.5f);
        anim_farmer.Play("smallJump1");
        yield return new WaitForSeconds(.5f);
        anim_farmer.Play("smallJump");
        yield return new WaitForSeconds(.5f);
        speachBubble.Say("Warum, denkt ihr wohl,\nwurden die Kartoffeln\neingegraben?", Bubbles.Normal, target: farmer);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("lookNormal");
        yield return new WaitForSeconds(.1f);
        anim_comp.Play("smallJump");

        speachBubble.Say("Unsere Theorie ist,\ndass Piraten...", Bubbles.Normal, target: companion, autoPlay: true);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_farmer.Play("lookAngry");
        speachBubble.Say("Die Wühlmäuse\nhaben sie verbuddelt\ndamit noch mehr\nKartoffeln wachsen", Bubbles.Normal, target: farmer);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim.SetTrigger("next");//resume explanation
        anim_farmer.Play("lookPuzzled");
        anim_player.Play("JumpScare");
        yield return new WaitForSeconds(.1f);
        anim_comp.Play("JumpScare");
        speachBubble.Say("Anscheinend kennen\nsie sich unglaublich\ngut mit Land-\nwirtschaft aus!", Bubbles.Normal, target: farmer);
        speachBubble.Say("Sogar noch besser als ich!", Bubbles.Normal, target: farmer);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_farmer.Play("lookAnnoyed");
        speachBubble.Say("Sie versuchen mir\ngerade beizubringen\nwie man richtig Kartoffeln\nanbaut", Bubbles.Normal, target: farmer);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("lookSad");
        yield return new WaitForSeconds(.1f);
        anim_player.Play("lookSad");
        speachBubble.Say("Heißt das,\nunsere Arbeit war\nfür umsonst?", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_farmer.Play("Squeesh");
        speachBubble.Say("...wie?\nNun...\n...naja...", Bubbles.Normal, target: farmer);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_farmer.Play("lookNormal");
        yield return new WaitForSeconds(.1f);
        //Idee:
        anim_farmer.Play("Squeesh");
        GameObject bulb = farmer.transform.parent.GetChild(2).gameObject;
        bulb.SetActive(true);
        bulb.transform.position = farmer.transform.position + Vector3.up ;
        bulb.GetComponent<Animator>().Play("Idea");
        yield return new WaitForSeconds(1);
        bulb.SetActive(false);

        speachBubble.Say("Umsonst war\ndas nicht!", Bubbles.Normal, target: farmer);
        speachBubble.Say("Schließlich habt\nihr gut mein \nFeld umgegraben", Bubbles.Normal, target: farmer);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("smallJump");
        speachBubble.Say("Heißt das,\nwir dürfen ein paar\nKartoffeln behalten?", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_farmer.Play("lookHappy");
        speachBubble.Say("Aber natürlich!\nVersprochen ist\nVersprochen!", Bubbles.Normal, target: farmer);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_farmer.Play("lookNormal");

        anim_player.Play("lookHappy");
        yield return new WaitForSeconds(.1f);
        anim_comp.Play("lookHappy");

        speachBubble.Say("Hurra", Bubbles.Shouting, fontSize: 35);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        manager.ChangeClothes();
        StartCoroutine(TasklistScript.SetTaskList(true));
        yield return new WaitForSeconds(1);
        runGame = false;
        staticCam = false;
        pauseMove = false;
        companion.GetComponent<FollowScript>().enabled = true;
        player.GetComponent<PlayerScript>().enabled = true;
        player.GetComponent<Collider2D>().enabled = true;
        player.GetComponent<Rigidbody2D>().simulated = true;
        anim_player.SetTrigger("startMove");
        anim_comp.SetTrigger("startMove");
        cScript.target = player;
        cScript.offset = Vector2.up * 2;
        //EndSequence();
        yield break;
    }
}
