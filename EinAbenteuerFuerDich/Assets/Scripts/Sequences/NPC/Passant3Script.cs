using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static CameraScript;
using static BubbleScript;

public class Passant3Script : TalkScript
{
    protected override IEnumerator PlaySequence()
    {
        StartCoroutine(Setup());
        StartCoroutine(cScript.SetZoom(6));
        yield return new WaitForSeconds(1);

        anim_char.Play("lookHappy");
        speachBubble.Say("Hach ja,\nwas kaufe ich\nmir heute?", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookSad");
        yield return new WaitForSeconds(.1f);
        anim_comp.Play("lookSad");

        speachBubble.Say("(Oh weh!)", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("Squeesh");
        yield return new WaitForSeconds(.5f);
        anim_player.Play("lookSad");
        player.GetComponent<VoiceScript>().PlayMumble();
        yield return new WaitForSeconds(1);


        anim_char.Play("lookNormal");
        StartCoroutine(cScript.SetZoom(8));
        yield return new WaitForSeconds(1);
        EndSequence();
        yield break;
    }
}
