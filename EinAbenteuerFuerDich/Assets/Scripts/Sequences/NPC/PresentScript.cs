using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static CameraScript;
using static BubbleScript;

public class PresentScript : TalkScript
{
    protected override IEnumerator PlaySequence()
    {
        StartCoroutine(Setup());
        StartCoroutine(cScript.SetZoom(6));
        yield return new WaitForSeconds(1);

        anim_char.Play("pMonJump");
        speachBubble.Say("Hey, sehe ich aus\nwie ein Geschenk\noder was?", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("JumpScare");
        yield return new WaitForSeconds(.1f);
        anim_comp.Play("JumpScare");

        speachBubble.Say("Ehrlich gesagt,\nirgendwie schon!\n", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_char.Play("pMonTalk");
        speachBubble.Say("Mmph,\ndas höre ich häufig", Bubbles.Normal, target: gameObject);
        speachBubble.Say("Trotzdem kein\nGrund mir gleich die\nKleider vom Leib\nzu reißen!", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_char.Play("pMonIdle");

        anim_player.Play("lookSad");
        yield return new WaitForSeconds(.1f);
        anim_comp.Play("lookSad");

        speachBubble.Say("Tut uns leid!\nDas wird nicht\nwieder vorkommen", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_char.Play("pMonTalk");
        speachBubble.Say("...Grummel...!\nDas will ich aber\nauch hoffen!", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_char.Play("pMonIdle");

        cScript.target = player;
        cScript.offset = new Vector2((companion.transform.position.x - player.transform.position.x)/2 , 2);
        StartCoroutine(cScript.SetZoom(3, 2));
        anim_player.Play("lookAnnoyed");
        anim_comp.Play("lookAnnoyed");
        yield return new WaitForSeconds(2);

        speachBubble.Say("(Na toll!\nWie sollen wir jetzt\nan unsere Geschenke\nrankommen)", Bubbles.Normal, target: companion);
        speachBubble.Say("(Wenn wir nicht\nWissen, ob es sich\num ein Geschenk oder\nein Monster handelt?)", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("Squeesh");
        player.GetComponent<VoiceScript>().PlayMumble();
        yield return new WaitForSeconds(.5f);
        anim_player.Play("lookSad");
        yield return new WaitForSeconds(1);

        StartCoroutine(cScript.SetZoom(8));
        yield return new WaitForSeconds(1);
        EndSequence();
        yield break;
    }
}
