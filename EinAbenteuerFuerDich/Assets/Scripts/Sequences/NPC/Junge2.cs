using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static CameraScript;
using static BubbleScript;

public class Junge2 : TalkScript
{
    protected override IEnumerator PlaySequence()
    {
        StartCoroutine(Setup());
        StartCoroutine(cScript.SetZoom(6));
        yield return new WaitForSeconds(1);

        speachBubble.Say("Was geht, Alder?\nHaste schon die\nHaltestelle abgecheckt?\nVoll krass,\nAlder!", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("Squeesh");
        speachBubble.Say("Aber siche...", Bubbles.Normal, target: companion, speed: .5f, autoPlay: true);
        speachBubble.Say("Ich mein:", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);


        anim_comp.Play("lookAnnoyed");
        speachBubble.Say("Aber siggy,\nJunge!", Bubbles.Normal, target: companion);
        speachBubble.Say("Alder,\nich latsch doch nicht\nquer über's Kartoffelfeld,\nMann!1!", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_char.Play("lookNormal");

        anim_char.Play("lookAnnoyed");
        speachBubble.Say("LOL,\ndas wäre voll uncool,\nAlder!!!", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        yield return new WaitForSeconds(3);
        speachBubble.Say("...Alder!", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_char.Play("lookNormal");
        StartCoroutine(cScript.SetZoom(8));
        yield return new WaitForSeconds(1);
        EndSequence();
        yield break;
    }
}
