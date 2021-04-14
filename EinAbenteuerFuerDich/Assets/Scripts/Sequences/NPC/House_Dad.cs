using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static CameraScript;
using static BubbleScript;

public class House_Dad : TalkScript
{
    protected override IEnumerator PlaySequence()
    {
        StartCoroutine(Setup());
        StartCoroutine(cScript.SetZoom(6));
        yield return new WaitForSeconds(1);

        anim_char.Play("lookFocused");
        speachBubble.Say("Warum braucht\nihr eigentlich so lange\nfür den Einkauf?", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookInPain");
        yield return new WaitForSeconds(.1f);
        anim_comp.Play("lookInPain");
        speachBubble.Say("Es ist gerade\netwas schwer alle\nSachen zu bekommen...", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_char.Play("lookAnnoyed");
        speachBubble.Say("Ist also wieder\ndie SuperRabatt-Woche\nim Supermarkt?", Bubbles.Normal, target: gameObject);
        speachBubble.Say("Da sind die Regale\nimmer im Nu leer!", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("Squeesh");
        yield return new WaitForSeconds(.5f);
        anim_player.Play("lookInPain");
        player.GetComponent<VoiceScript>().PlayExplain();
        yield return new WaitForSeconds(1);

        anim_char.Play("lookFocused");
        speachBubble.Say("Soso...", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_char.Play("lookNormal");
        StartCoroutine(cScript.SetZoom(8));
        yield return new WaitForSeconds(1);
        EndSequence();
        yield break;
    }
}