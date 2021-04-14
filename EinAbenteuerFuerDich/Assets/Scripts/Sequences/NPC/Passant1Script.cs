using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static CameraScript;
using static BubbleScript;

public class Passant1Script : TalkScript
{
    protected override IEnumerator PlaySequence()
    {
        StartCoroutine(Setup());
        StartCoroutine(cScript.SetZoom(6));
        yield return new WaitForSeconds(1);

        anim_char.Play("lookHappy");
        speachBubble.Say("Ich liebe es\nmit dem Bus zu\nfahren!", Bubbles.Normal, target: gameObject);
        speachBubble.Say("Es gibt Tage an denen\nich nur mit dem Bus\nhin- und her fahre!", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("lookPuzzled");
        speachBubble.Say("Ok,\ndas ist ein\nkomisches Hobby", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("smallJump");
        speachBubble.Say("Da fällt mir ein:\nWir haben doch auch\nFahrkarten!", Bubbles.Normal, target: companion);
        speachBubble.Say("Wir müssen bloß\ndie Haltestelle\nantippen um den nächsten\nBus zu nehmen", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("smallJump1");
        yield return new WaitForSeconds(.5f);
        anim_comp.Play("smallJump");
        yield return new WaitForSeconds(.5f);
        anim_comp.Play("smallJump1");

        speachBubble.Say("Dann müssen wir\nnicht immer so\nweit laufen", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookHappy");
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(1);

        anim_char.Play("lookNormal");
        StartCoroutine(cScript.SetZoom(8));
        yield return new WaitForSeconds(1);
        EndSequence();
        yield break;
    }
}
