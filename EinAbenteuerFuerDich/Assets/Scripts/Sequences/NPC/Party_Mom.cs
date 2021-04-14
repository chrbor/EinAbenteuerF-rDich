using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static CameraScript;
using static BubbleScript;

public class Party_Mom : TalkScript
{
    protected override IEnumerator PlaySequence()
    {
        StartCoroutine(Setup());
        StartCoroutine(cScript.SetZoom(6));
        yield return new WaitForSeconds(1);

        anim_char.Play("JumpScare");
        speachBubble.Say("Huch, wo kommen\ndenn all die\nLeute her?", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("lookInPain");
        yield return new WaitForSeconds(.1f);
        anim_player.Play("lookAnnoyed");
        speachBubble.Say("Nur ein paar Leute\ndie wir beim\nEinkaufen getroffen\nhaben!", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        player.GetComponent<VoiceScript>().PlayName();
        yield return new WaitForSeconds(1);

        anim_char.Play("lookAnnoyed");
        speachBubble.Say("Der Supermarkt muss\nziemlich voll\ngewesen sein", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("lookNormal");
        anim_player.Play("lookNormal");
        anim_char.Play("lookHappy");
        speachBubble.Say("Aber es freut mich\ndass ihr so viele\nneue Freunde\ngefunden habt", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_char.Play("lookNormal");
        StartCoroutine(cScript.SetZoom(8));
        yield return new WaitForSeconds(1);
        EndSequence();
        yield break;
    }
}