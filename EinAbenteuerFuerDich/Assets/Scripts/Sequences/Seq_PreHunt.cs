using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;
using static CameraScript;
using static BubbleScript;

public class Seq_PreHunt : MonoBehaviour
{
    private GameObject camFocus;
    private GameObject chicken;

    public AudioClip Seq_clip;

    private void OnTriggerEnter2D(Collider2D other)
    {
        runGame = true;
        pauseMove = true;
        staticCam = true;
        camFocus = transform.GetChild(0).gameObject;
        cScript.target = camFocus;
        StartCoroutine(RunSequence());
    }

    IEnumerator RunSequence()
    {
        StartCoroutine(TasklistScript.SetTaskList(false));
        cScript.aSrc.Stop();
        yield return new WaitForEndOfFrame();
        cScript.aSrc.clip = Seq_clip;
        cScript.aSrc.Play();

        StartCoroutine(cScript.SetRotation());
        StartCoroutine(cScript.SetZoom(10));
        cScript.strength = .02f;
        cScript.offset = Vector2.zero;
        camFocus.transform.localPosition = new Vector3(0, 1);
        yield return new WaitForSeconds(1);

        companion.GetComponent<FollowScript>().enabled = false;
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        yield return new WaitForSeconds(.1f);
        player.transform.GetChild(0).GetComponent<Animator>().SetBool("inAir", false);
        yield return new WaitForSeconds(.25f);


        //Nach den Hühnern schauen:
        camFocus.transform.localPosition = new Vector3(11, 1);
        companion.transform.parent = transform;
        player.transform.parent = transform;
        Animator anim = GetComponent<Animator>();
        Animator anim_comp = companion.transform.GetChild(0).GetComponent<Animator>();
        Animator anim_player = player.transform.GetChild(0).GetComponent<Animator>();


        anim_comp.SetBool("moving", true);
        anim_comp.SetTrigger("startMove");
        Vector3 step = Vector3.right * .1f;
        while(companion.transform.localPosition.x < 3)
        {
            companion.transform.position += step;
            yield return new WaitForFixedUpdate();
        }
        anim_comp.SetBool("moving", false);
        anim.Rebind();

        anim_comp.Play("lookPuzzled");
        yield return new WaitForSeconds(2);
        anim_comp.Play("lookNormal");
        yield return new WaitForSeconds(.3f);

        camFocus.transform.localPosition = new Vector3(-3, 1);
        anim.Play("moveLeft");
        anim_comp.SetTrigger("startMove");
        anim_comp.SetBool("moving", true);
        yield return new WaitForSeconds(3);
        anim_comp.SetBool("moving", false);
        anim_comp.Play("lookFocused");
        yield return new WaitForSeconds(2);
        anim_comp.Play("lookNormal");
        yield return new WaitForSeconds(.3f);
        anim_comp.Play("lookAnnoyed");

        speachBubble.Say("Die Hühner sind alle weg\nKeine einzige Feder ist hier.", Bubbles.Normal, target: companion);
        speachBubble.Say("Ob das das Werk\neines Fuchs ist?", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("Squeesh");
        player.GetComponent<VoiceScript>().PlayQuestion();
        yield return new WaitForSeconds(.5f);
        anim_player.Play("lookSad");
        yield return new WaitForSeconds(.5f);

        speachBubble.Say("Stimmt!\nHätte der Fuchs sie alle\ngefressen,", Bubbles.Normal, target: companion);
        speachBubble.Say("dann hätte er sie\nvorher rupfen und\nbraten müssen", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("lookPuzzled");
        cScript.strength = 0.1f;
        cScript.offset = Vector3.up * 2;
        StartCoroutine(cScript.SetRotation(20, 0.5f));
        StartCoroutine(cScript.SetZoom(5, .5f));
        cScript.target = companion;
        yield return new WaitForSeconds(.5f);

        speachBubble.Say("Aber hier ist\nnirgendswo\nein OFEN!!!", Bubbles.Normal, target: companion, fontSize: 35);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        cScript.target = camFocus;
        cScript.strength = 0.03f;
        cScript.offset = Vector3.zero;
        StartCoroutine(cScript.SetRotation());
        StartCoroutine(cScript.SetZoom(10, 2));
        camFocus.transform.localPosition = Vector3.up * 1;

        anim_comp.Play("lookNormal");
        anim_player.Play("lookAnnoyed");
        yield return new WaitForSeconds(1);
        player.GetComponent<VoiceScript>().PlayMumble();
        yield return new WaitForSeconds(1);
        anim_player.Play("lookNormal");


        anim_comp.SetBool("moving", true);
        anim_comp.SetTrigger("startMove");
        anim.Play("moveRight");
        yield return new WaitForSeconds(3.5f);
        anim_comp.SetBool("moving", false);
        yield return new WaitForSeconds(.5f);
        anim_comp.SetBool("moving", true);
        anim.Play("moveLeftAgain");
        yield return new WaitForSeconds(2.5f);
        anim_comp.SetBool("moving", false);
        anim_comp.Play("lookFocused");

        speachBubble.Say("Hhhhmmm......\nWas ist mit den\nHühnern passiert?", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        yield return new WaitForSeconds(2);
        anim_comp.Play("lookNormal");
        yield return new WaitForSeconds(.3f);

        anim_comp.Play("Squeesh");
        anim.Play("getIdea");
        yield return new WaitForSeconds(2);

        anim_comp.Play("lookScared");
        cScript.strength = 0.1f;
        cScript.offset = Vector3.up * 2;
        StartCoroutine(cScript.SetRotation(-20, 0.5f));
        StartCoroutine(cScript.SetZoom(5, .5f));
        cScript.target = companion;
        yield return new WaitForSeconds(.5f);

        speachBubble.Say("Sie müssen alle\nvon ALIENS entführt\nworden sein!", Bubbles.Shouting, target: companion, fontSize: 35);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        cScript.strength = .05f;
        StartCoroutine(cScript.SetRotation());
        StartCoroutine(cScript.SetZoom(8));
        cScript.target = player;

        anim_player.Play("lookAngry");
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(1);
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(1);
        anim_player.Play("lookNormal");

        StartCoroutine(cScript.SetZoom(10));

        speachBubble.Say("Sicher, dass es keine\nAuserirdischen waren?", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("lookSad");
        yield return new WaitForSeconds(1);
        anim_player.Play("lookAnnoyed");
        yield return new WaitForSeconds(.1f);
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(3);
        anim_comp.Play("lookNormal");
        yield return new WaitForSeconds(.3f);
        anim_comp.Play("Squeesh");
        yield return new WaitForSeconds(2);
        anim_player.Play("lookNormal");

        speachBubble.Say("Wahrscheinlich\nhast du recht...", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("lookAngry");
        anim.Play("UfoFlyBy");

        speachBubble.Say("So eine Hühnerkacke!\nWie sollen wir jetzt an\ndie Eier und die\nBrühe kommen?", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);


        player.GetComponent<VoiceScript>().PlayQuestion();
        anim_player.Play("lookSad");
        yield return new WaitForSeconds(.1f);
        anim_player.Play("Squeesh");
        yield return new WaitForSeconds(1);
        anim_comp.Play("lookSad");

        StartCoroutine(cScript.SetZoom(8));

        speachBubble.Say("Sieht so aus, als ob\nwir doch nicht zu unserem\nKartoffelkuchen kommen", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookNormal");
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(.2f);
        anim_player.Play("JumpScare");
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("ChickenHunt");
        yield break;
    }
}
