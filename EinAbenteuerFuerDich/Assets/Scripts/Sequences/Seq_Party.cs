using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static CameraScript;
using static BubbleScript;

public class Seq_Party : Sequence
{

    // Start is called before the first frame update
    void Start()
    {
        runGame = true;
        pauseMove = true;
        staticCam = true;
        StartCoroutine(PlaySequence());
    }

    protected override IEnumerator PlaySequence()
    {
        StartCoroutine(Setup());
        yield return new WaitForSeconds(2f);

        anim_player.Play("JumpScare");
        yield return new WaitForSeconds(.1f);
        anim_comp.Play("JumpScare");
        speachBubble.Say("Das ist aber ein\ngroßes Fest", Bubbles.Normal, target: companion);
        speachBubble.Say("Und schaue sich erst\neiner diesen rießigen\nKartoffelkuchen an!", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        cScript.offset += Vector2.left * 10;
        anim_comp.Play("lookHappy");
        yield return new WaitForSeconds(3);
        cScript.offset += Vector2.right * 10;

        yield return new WaitForSeconds(1);
        speachBubble.Say("Und so viele\nGeschenke!", Bubbles.Normal, target: companion);
        speachBubble.Say("Ich kann es kaum\nerwarten sie alle\nauszupacken", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookAnnoyed");
        player.GetComponent<VoiceScript>().PlayExplain();
        yield return new WaitForSeconds(1);

        anim_comp.Play("lookAnnoyed");
        speachBubble.Say("Jaja,\nnatürlich werde ich\nmich bei jedem\nbedanken", Bubbles.Normal, target: companion);
        speachBubble.Say("Du hörst dich\nschon fast so an\nwie Mama!", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_player.Play("lookNormal");

        anim_comp.Play("Squeesh");
        yield return new WaitForSeconds(.5f);
        anim_comp.Play("lookPuzzled");

        speachBubble.Say("Ich frage mich\nwie viele Leute\ndas sind", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("lookHappy");
        speachBubble.Say("Lass uns am besten\nhier ein bisschen\numgucken!", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookHappy");
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(1);

        EndSequence(false);
        yield break;
    }
}
