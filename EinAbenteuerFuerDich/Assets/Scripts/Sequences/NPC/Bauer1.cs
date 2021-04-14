using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static CameraScript;
using static BubbleScript;

public class Bauer1 : TalkScript
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
        anim_char.Play(progress.millState % 3 == 2 ? "lookHappy" : "lookFurious");
        yield return new WaitForSeconds(.5f);
        speachBubble.Say("Meine\nMühle!", Bubbles.Shouting, fontSize: 35);
        StartCoroutine(cScript.SetZoom(3, .5f));
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        yield return new WaitForSeconds(.5f);
        StartCoroutine(cScript.SetZoom(8));
        StartCoroutine(cScript.SetRotation());
        yield return new WaitForSeconds(1);

        EndSequence();
        yield break;
    }
}
