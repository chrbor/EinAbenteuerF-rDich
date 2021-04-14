using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static CameraScript;
using static BubbleScript;

public class Party_Dad : TalkScript
{
    protected override IEnumerator PlaySequence()
    {
        StartCoroutine(Setup());
        StartCoroutine(cScript.SetZoom(6));
        yield return new WaitForSeconds(1);

        speachBubble.Say("Gefällt euch die Feier?", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("Squeesh");
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(1);

        StartCoroutine(cScript.SetZoom(8));
        yield return new WaitForSeconds(1);
        EndSequence();
        yield break;
    }
}