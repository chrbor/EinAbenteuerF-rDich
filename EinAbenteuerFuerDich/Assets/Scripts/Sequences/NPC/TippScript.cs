using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static CameraScript;
using static BubbleScript;

public class TippScript : TalkScript
{

    TippScript tipp;
    private void Start()
    {
        tipp = this;
    }

    public void PlayTipp() => StartCoroutine(PlaySequence());

    protected override IEnumerator PlaySequence()
    {
        StartCoroutine(Setup());
        StartCoroutine(cScript.SetZoom(3));
        yield return new WaitForSeconds(1);

        anim_comp.Play("lookFocused");
        speachBubble.Say("Hm, was brauchen\nwir noch für\nden Kartoffelkuchen?", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        if (progress.millState < 3) { StartCoroutine(TippMill()); yield break; }
        if (!progress.potatoDone) { StartCoroutine(TippPotato()); yield break; }
        if (!progress.brewDone || !progress.huntDone) { StartCoroutine(TippHunt()); yield break; }
        if (!progress.marketDone) { StartCoroutine(TippMarket()); yield break; }
        
        //Ansonsten: Alles fertig:

        anim_comp.Play("smallJump");
        speachBubble.Say("Ich glaube wir\nhaben alles!", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("lookHappy");
        speachBubble.Say("Jetzt müssen wir\nnur noch die Sachen\nnach Hause bringen", Bubbles.Normal, target: gameObject);
        speachBubble.Say("Damit Mama\nden Kuchen backen\nkann", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookHappy");
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(1);

        StartCoroutine(cScript.SetZoom(8));
        yield return new WaitForSeconds(1);
        EndSequence();
        yield break;
    }

    IEnumerator TippMill()
    {
        anim_comp.Play("smallJump");
        speachBubble.Say("Ich glaube wir\nbrauchen noch Mehl!", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        switch (progress.millState)
        {
            case 0:
                anim_comp.Play("lookSad");
                speachBubble.Say("Aber wo bekommen\nwir bloß Mehl her", Bubbles.Normal, target: gameObject);
                speachBubble.Say("Wenn doch\ndie Mühle zerstört ist?", Bubbles.Normal, target: gameObject);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                anim_player.Play("Squeesh");
                yield return new WaitForSeconds(.5f);
                anim_player.Play("lookSad");
                player.GetComponent<VoiceScript>().PlayMumble();

                anim_comp.Play("smallJump");
                speachBubble.Say("Dann müssen wir\ndie Mühle halt selbst\nwieder reparieren", Bubbles.Normal, target: gameObject);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                anim_player.Play("smallJump");
                player.GetComponent<VoiceScript>().PlayExplain();
                yield return new WaitForSeconds(.5f);
                anim_player.Play("lookAnnoyed");

                anim_comp.Play("lookFocused");
                speachBubble.Say("Stimmt, alleine\nschaffen wir\ndas nie!", Bubbles.Normal, target: gameObject);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                anim_comp.Play("smallJump");
                speachBubble.Say("Vielleicht finden\nwir jemanden in\nder Stadt, der uns\nhelfen kann", Bubbles.Normal, target: gameObject);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);
                break;
            case 1:
                anim_comp.Play("lookHappy");
                speachBubble.Say("Glücklicherweise\nhilft uns die Guilde\nder Handwerker", Bubbles.Normal, target: gameObject);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                anim_comp.Play("lookFocused");
                speachBubble.Say("Hoffentlich schaffen\nwir es zusammen die\nMühle wieder\naufzubauen", Bubbles.Normal, target: gameObject);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                anim_comp.Play("smallJump");
                speachBubble.Say("Lass uns am\nbesten gleich zur\nMühle gehen", Bubbles.Normal, target: gameObject);
                speachBubble.Say("Die Handwerker\nsollten schon dort\nsein", Bubbles.Normal, target: gameObject);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);
                break;
            case 2:
                anim_comp.Play("lookHappy");
                speachBubble.Say("Zum Glück ist\ndie Mühle jetzt wieder\nrepariert", Bubbles.Normal, target: gameObject);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                anim_comp.Play("smallJump");
                speachBubble.Say("Lass uns am\nbesten gleich zur\nMühle gehen", Bubbles.Normal, target: gameObject);
                speachBubble.Say("Und schauen, ob\nschon genug Mehl\n für uns gemahlen\nwurde", Bubbles.Normal, target: gameObject);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);
                break;
        }

        anim_player.Play("lookHappy");
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(1);


        StartCoroutine(cScript.SetZoom(8));
        yield return new WaitForSeconds(1);
        EndSequence();
        yield break;
    }
    IEnumerator TippPotato()
    {
        anim_comp.Play("smallJump");
        speachBubble.Say("Wenn ich  mich\nnicht irre, dann\nbrauchen wir noch\nKartoffeln!", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("lookPuzzled");
        speachBubble.Say("Ist nicht hinter\nder mühle ein\nrießiges Kartoffelfeld?", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("lookHappy");
        speachBubble.Say("In dessen Lagerhaus\nsollten genug Kartoffeln\nfür 100 Kartoffelkuchen\nsein!", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookHappy");
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(1);

        StartCoroutine(cScript.SetZoom(8));
        yield return new WaitForSeconds(1);
        EndSequence();
        yield break;
    }
    IEnumerator TippMarket()
    {
        anim_comp.Play("smallJump");
        speachBubble.Say("Als letztes brauchen\nwir noch den ganzen\nanderen Kram", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("lookFocused");
        speachBubble.Say("Ich bin mir zwar\nnicht ganz sicher,\nwas das alles sein\nsoll...", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("Squeesh");
        speachBubble.Say("Aber ein bisschen\nGeld haben wir\nnoch", Bubbles.Normal, target: gameObject);
        speachBubble.Say("Damit können wir\nim Supermarkt\nden Rest holen!", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookHappy");
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(1);

        StartCoroutine(cScript.SetZoom(8));
        yield return new WaitForSeconds(1);
        EndSequence();
        yield break;
    }
    IEnumerator TippHunt()
    {
        anim_comp.Play("smallJump");
        speachBubble.Say("Als nächstes brauchen\nwir noch Eier\nund Hühnerbrühe", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("Squeesh");
        speachBubble.Say("Ich glaube ich habe\neinen Hühnerstall hinter\nden Kartoffelfeldern\ngesehen", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookAngry");
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(1);

        anim_comp.Play("smallJump");
        yield return new WaitForSeconds(.5f);
        anim_comp.Play("lookAnnoyed");

        speachBubble.Say("Irgendwoher müssen\nwir die Brühe bekommen!", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        StartCoroutine(cScript.SetZoom(8));
        yield return new WaitForSeconds(1);
        EndSequence();
        yield break;
    }
}