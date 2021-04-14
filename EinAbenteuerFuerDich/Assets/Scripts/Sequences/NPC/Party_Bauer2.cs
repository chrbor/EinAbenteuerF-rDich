using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static CameraScript;
using static BubbleScript;

public class Party_Bauer2 : TalkScript
{
    protected override IEnumerator PlaySequence()
    {
        StartCoroutine(Setup());
        StartCoroutine(cScript.SetZoom(6));
        yield return new WaitForSeconds(1);

        anim_char.Play("lookHappy");
        speachBubble.Say("Alles gute\nund danke für\ndie Einladung", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("Squeesh");
        speachBubble.Say("Wie geht es mit\ndem Kartoffelanbau\nvoran?", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_char.Play("smallJump");
        speachBubble.Say("Sehr gut!\nJetzt, wo die\nWühlmäuse mithelfen", Bubbles.Normal, target: gameObject);
        speachBubble.Say("Bekomme ich doppelt\nso viele Kartoffeln\naus der Ernte", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_char.Play("lookFocused");
        speachBubble.Say("Leider futtern die\nWühlmäuse mir die Hälfte\nder Ernte weg", Bubbles.Normal, target: gameObject);
        speachBubble.Say("So dass es keinen\ngroßen Unterschied\nmacht", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_char.Play("lookHappy");
        speachBubble.Say("Aber zumindest\nhabe ich ein paar\ngute Freunde\ngefunden", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_char.Play("lookFocused");
        speachBubble.Say("...Gefräßige Freunde...", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_char.Play("lookNormal");
        StartCoroutine(cScript.SetZoom(8));
        yield return new WaitForSeconds(1);
        EndSequence();
        yield break;
    }
}