using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static CameraScript;
using static BubbleScript;

public class Party_Screw : TalkScript
{
    protected override IEnumerator PlaySequence()
    {
        StartCoroutine(Setup());
        StartCoroutine(cScript.SetZoom(6));
        yield return new WaitForSeconds(1);

        speachBubble.Say("Oh, hallo!", Bubbles.Normal, target: gameObject);
        speachBubble.Say("Alles gute zum\nGeburtstag!\nDas ist eine tolle Party!", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("smallJump");
        speachBubble.Say("Du wirkst etwas\nabwesend!", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_char.Play("lookPuzzled");
        speachBubble.Say("Ich schaue mir\ngerade an, wie\nweit der Hammer mit\ndem Haus ist", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_char.Play("Squeesh");
        speachBubble.Say("Er gibt immer an,\nwie schnell er Häuser\nbauen kann", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_char.Play("lookAnnoyed");
        speachBubble.Say("Aber meißtens werden\ndie Häuser von ihm\ndafür krum und schief", Bubbles.Normal, target: gameObject);
        speachBubble.Say("Euer Haus war\nübrigens das erste,\ndass der Hammer\ngebaut hat", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookAnnoyed");
        anim_comp.Play("Squeesh");
        yield return new WaitForSeconds(.5f);
        anim_comp.Play("lookAnnoyed");
        speachBubble.Say("Das erklärt einiges...", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_char.Play("lookNormal");
        StartCoroutine(cScript.SetZoom(8));
        yield return new WaitForSeconds(1);
        EndSequence();
        yield break;
    }
}
