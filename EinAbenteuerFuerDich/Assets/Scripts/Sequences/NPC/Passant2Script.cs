using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static CameraScript;
using static BubbleScript;

public class Passant2Script : TalkScript
{
    protected override IEnumerator PlaySequence()
    {
        StartCoroutine(Setup());
        StartCoroutine(cScript.SetZoom(6));
        yield return new WaitForSeconds(1);

        speachBubble.Say("Hallo, wie geht's?", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_char.Play("lookNormal");

        anim_player.Play("lookHappy");
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(1);

        anim_char.Play("Squeesh");
        speachBubble.Say("Gibt es was neues?", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("smallJump");
        player.GetComponent<VoiceScript>().PlayName();
        yield return new WaitForSeconds(1);

        anim_comp.Play("lookSad");
        speachBubble.Say("Oh-oh!", Bubbles.Normal, target: companion);
        speachBubble.Say("Hoffentlich endet\ndas nicht wieder in\neinem ellenlangen\nGespräch!", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("Squeesh");
        player.GetComponent<VoiceScript>().PlayExplain();
        yield return new WaitForSeconds(2);

        anim_player.Play("lookNormal");
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(1);
        player.GetComponent<VoiceScript>().PlayName();
        yield return new WaitForSeconds(1);

        anim_player.Play("smallJump");
        player.GetComponent<VoiceScript>().PlayExplain();
        yield return new WaitForSeconds(1);

        anim_char.Play("SmallJump");
        speachBubble.Say("Interessant!!\nErzähl weiter!", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_comp.Play("smallJump");
        yield return new WaitForSeconds(.5f);
        anim_comp.Play("lookSad");
        speachBubble.Say("(Nein, hör auf!)", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookHappy");
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(1);
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(1);
        player.GetComponent<VoiceScript>().PlayName();
        anim_comp.Play("lookAnnoyed");
        yield return new WaitForSeconds(2);

        anim_player.Play("lookFocused");
        player.GetComponent<VoiceScript>().PlayMumble();
        yield return new WaitForSeconds(1);
        player.GetComponent<VoiceScript>().PlayName();
        yield return new WaitForSeconds(2);

        anim_player.Play("lookAngry");
        player.GetComponent<VoiceScript>().PlayQuestion();
        yield return new WaitForSeconds(1);

        anim_char.Play("JumpScare");
        speachBubble.Say("Oh nein, wirklich?\nUnd was ist dann\npassiert?", Bubbles.Normal, target: gameObject);
        anim_comp.Play("lookFocused");
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookNormal");
        player.GetComponent<VoiceScript>().PlayName();
        yield return new WaitForSeconds(1);
        player.GetComponent<VoiceScript>().PlayExplain();
        yield return new WaitForSeconds(1.5f);

        anim_player.Play("smallJump");
        player.GetComponent<VoiceScript>().PlayName();
        yield return new WaitForSeconds(1);

        anim_comp.Play("Squeesh");
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(.5f);
        anim_comp.Play("LookInPain");
        anim_player.Play("lookAnnoyed");
        speachBubble.Say("Wir würden uns ja gerne\nnoch weiter mit dir \nunterhalten,", Bubbles.Normal, target: companion);
        speachBubble.Say("Aber wir haben noch\nfürchterlich viel\nzu tun", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_char.Play("lookSad");
        speachBubble.Say("Oh, tut mir leid!\nIch wollte euch nicht\naufhalten", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_char.Play("lookHappy");
        anim_player.Play("lookNormal");
        speachBubble.Say("Du musst mir unbedingt\nspäter noch den Rest\nder Geschichte\nerzählen", Bubbles.Normal, target: gameObject);
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
