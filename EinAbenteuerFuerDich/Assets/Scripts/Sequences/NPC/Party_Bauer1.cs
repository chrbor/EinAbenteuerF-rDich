using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static CameraScript;
using static BubbleScript;

public class Party_Bauer1 : TalkScript
{
    protected override IEnumerator PlaySequence()
    {
        StartCoroutine(Setup());
        StartCoroutine(cScript.SetZoom(6));
        yield return new WaitForSeconds(.1f);
        cScript.offset = Vector2.up * 2;
        yield return new WaitForSeconds(1);

        StartCoroutine(cScript.SetZoom(2, .5f));
        StartCoroutine(cScript.SetRotation(-10, .5f));
        anim_char.Play("lookFurious");
        yield return new WaitForSeconds(.5f);
        anim_player.Play("lookInPain");
        anim_comp.Play("lookInPain");
        speachBubble.Say("Meine\nMühle!", Bubbles.Shouting, fontSize: 35, speed: .2f, autoPlay: true);
        StartCoroutine(cScript.SetZoom(3, .5f));
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(cScript.SetZoom(6));
        StartCoroutine(cScript.SetRotation());

        anim_char.Play("lookNormal");
        speachBubble.Say("Ah, sorry!\nMacht der Gewohnheit!", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_char.Play("lookHappy");
        speachBubble.Say("Alles gute\nzum Geburtstag!", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        speachBubble.Say("Ahahah...ha...\n...Danke...", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookSad");
        yield return new WaitForSeconds(.1f);
        anim_comp.Play("lookSad");

        anim_char.Play("lookNormal");
        speachBubble.Say("Tut uns leid,\ndass wir ihr Mühle\nzerstört haben", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        speachBubble.Say("Macht euch da\nkeine Sorgen", Bubbles.Normal, target: gameObject);
        speachBubble.Say("Die Mühle war\nsehr alt und\nbaufällig", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_char.Play("lookHappy");
        speachBubble.Say("Um ehrlich zu sein\nhätte ich sie sowieso\ndemnächst neu aufgebaut", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        anim_comp.Play("JumpScare");
        yield return new WaitForSeconds(.1f);
        anim_player.Play("JumpScare");
        yield return new WaitUntil(() => speachBubble.finished);

        anim_char.Play("lookNormal");
        StartCoroutine(cScript.SetZoom(8));
        yield return new WaitForSeconds(1);
        EndSequence();
        yield break;
    }
}
