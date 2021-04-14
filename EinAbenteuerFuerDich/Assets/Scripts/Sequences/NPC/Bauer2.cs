using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static CameraScript;
using static BubbleScript;

public class Bauer2 : TalkScript
{
    protected override IEnumerator PlaySequence()
    {
        StartCoroutine(Setup());
        StartCoroutine(cScript.SetZoom(6));
        yield return new WaitForSeconds(1);

        anim_char.Play("lookPuzzled");
        speachBubble.Say("Hm, die Hühner\nsind wieder weg", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookSad");
        player.GetComponent<VoiceScript>().PlayQuestion();
        yield return new WaitForSeconds(1);

        anim_char.Play("lookNormal");
        speachBubble.Say("Ob ich mir\nSorgen mache?", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_char.Play("Squeesh");
        speachBubble.Say("Ach Iwo,\ndas machen die\nHühner ständig!", Bubbles.Normal, target: gameObject);
        speachBubble.Say("Abends kommen sie\ndann immer mit\nentspannden Gesichtern\nzurück in ihr Stall", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("smallJump");
        yield return new WaitForSeconds(.5f);
        anim_comp.Play("lookSad");
        speachBubble.Say("Aber dann ist unser\nGeburtstag schon\nlängst vorbei!", Bubbles.Normal, target: companion);
        speachBubble.Say("Und wir brauchen\nunbedingt noch\nHühnerbrühe und Eier!", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_char.Play("lookNormal");
        speachBubble.Say("Hm,\nDann müsst ihr\nsie wohl suchen", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_char.Play("lookHappy");
        speachBubble.Say("Viel Erfolg!", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_char.Play("lookNormal");
        StartCoroutine(cScript.SetZoom(8));
        yield return new WaitForSeconds(1);
        EndSequence();
        yield break;
    }
}
