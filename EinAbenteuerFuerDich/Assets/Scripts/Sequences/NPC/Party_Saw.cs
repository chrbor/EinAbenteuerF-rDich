using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static CameraScript;
using static BubbleScript;

public class Party_Saw : TalkScript
{
    protected override IEnumerator PlaySequence()
    {
        StartCoroutine(Setup());
        StartCoroutine(cScript.SetZoom(6));
        yield return new WaitForSeconds(1);

        anim_comp.Play("lookAnnoyed");
        speachBubble.Say("(Oh nein,\nwer hat denn die\nNervensäge\neingeladen?)", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_char.Play("smallJump");
        speachBubble.Say("Alles gute zum\nGeburtstag", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        speachBubble.Say("Ich muss schon sagen:\nEs sind eine Menge\nGäste hier", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        speachBubble.Say("Ich habe mich\nzum Beispiel gerade mit dem\nBesitzer der Mühle\nunterhalten", Bubbles.Normal, target: gameObject);
        speachBubble.Say("Wie man die Mühle\nnoch weiter ausbauen\nkann", Bubbles.Normal, target: gameObject);
        speachBubble.Say("Und welche Formulare\nund Anträge benötigt\nwerden", Bubbles.Normal, target: gameObject);
        speachBubble.Say("Um den Ausbau\nzu bewilligen. Zum einen\nist da zum\nBeispiel...", Bubbles.Normal, target: gameObject, speed: .3f, autoPlay: true);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitForSeconds(1);
        anim_player.Play("lookAnnoyed");
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("Squeesh");
        yield return new WaitForSeconds(.5f);
        anim_comp.Play("lookInPain");
        speachBubble.Say("Alles gut!\nWir wollten euch\nnicht stören", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        speachBubble.Say("Das ist kein Problem!\nIch bin gespannt, was\nihr über die Pläne zum\nAusbau denkt", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookInPain");
        player.GetComponent<VoiceScript>().PlayExplain();
        yield return new WaitForSeconds(1);

        anim_char.Play("lookSad");
        speachBubble.Say("Ihr werdet\nanderswo gebraucht?", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_char.Play("lookHappy");
        speachBubble.Say("Dann reden wir\nhalt später darüber", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);


        anim_char.Play("lookNormal");
        StartCoroutine(cScript.SetZoom(8));
        yield return new WaitForSeconds(1);
        EndSequence();
        yield break;
    }
}
