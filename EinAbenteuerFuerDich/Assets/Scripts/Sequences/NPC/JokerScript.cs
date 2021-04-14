using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;
using static CameraScript;
using static BubbleScript;
using static MenuScript;

public class JokerScript : TalkScript
{
    public Material transition;

    protected override IEnumerator PlaySequence()
    {
        StartCoroutine(Setup());
        StartCoroutine(cScript.SetZoom(6));
        yield return new WaitForSeconds(.1f);
        yield return new WaitForSeconds(1);

        switch (progress.jokeCount)
        {
            case 0:
                cScript.offset = Vector2.up * 2;
                yield return new WaitForSeconds(1);
                speachBubble.Say("Ich bin gestern zu\nspät zur Arbeit gekommen,\nweil der Zug ausgefallen\nist", Bubbles.Normal, target: gameObject);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);
                StartCoroutine(cScript.SetZoom(2, .5f));
                StartCoroutine(cScript.SetRotation(10, .5f));
                anim_char.Play("lookPuzzled");
                yield return new WaitForSeconds(.5f);
                speachBubble.Say("Anscheinend war die\nLok down!", Bubbles.Normal, target: gameObject);
                StartCoroutine(cScript.SetZoom(3, .5f));
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);
                yield return new WaitForSeconds(.5f);
                StartCoroutine(cScript.SetZoom(6));
                StartCoroutine(cScript.SetRotation());
                cScript.offset = new Vector2((player.transform.position.x - gameObject.transform.position.x) / 2, 2);
                yield return new WaitForSeconds(1);
                anim_char.Play("lookHappy");
                speachBubble.Say("Aber als ich das\nmeinem Chef bei einem\nkühlen Corona erklärt\nhabe,", Bubbles.Normal, target: gameObject);
                speachBubble.Say("Hat er es mir\nverziehen", Bubbles.Normal, target: gameObject);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);
                break;
            case 1:
                anim_char.Play("smallJump");
                speachBubble.Say("Hey,\nwillst du einen\nBaustellenwitz hören?", Bubbles.Normal, target: gameObject);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);
                anim_comp.Play("lookPuzzled");
                speachBubble.Say("...Ja?", Bubbles.Normal, target: companion);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);
                anim_comp.Play("lookAnnoyed");
                anim_player.Play("lookAnnoyed");
                speachBubble.Say("...", Bubbles.Normal, target: gameObject);
                speachBubble.Say("...ich arbeite dran!", Bubbles.Normal, target: gameObject);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                break;
            case 2:
                anim_char.Play("smallJump");
                speachBubble.Say("Ein Rätsel für dich:", Bubbles.Normal, target: gameObject);
                speachBubble.Say("Einem Mann\nfällt das Handy runter.\nDem Gerät passiert aber\nnichts!\nWarum?", Bubbles.Normal, target: gameObject);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);
                anim_comp.Play("lookPuzzled");
                speachBubble.Say("...Äh...?", Bubbles.Normal, target: companion);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);
                cScript.offset = Vector2.up * 2;
                yield return new WaitForSeconds(1);
                anim_comp.Play("lookAnnoyed");
                anim_player.Play("lookAnnoyed");
                StartCoroutine(cScript.SetZoom(2, .5f));
                StartCoroutine(cScript.SetRotation(10, .5f));
                anim_char.Play("jumpScare");
                yield return new WaitForSeconds(.5f);
                speachBubble.Say("Es war im\nFlugmodus!", Bubbles.Normal, target: gameObject);
                StartCoroutine(cScript.SetZoom(3, .5f));
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);
                yield return new WaitForSeconds(.5f);
                StartCoroutine(cScript.SetZoom(6));
                StartCoroutine(cScript.SetRotation());
                cScript.offset = new Vector2((player.transform.position.x - gameObject.transform.position.x) / 2, 2);
                yield return new WaitForSeconds(1);
                speachBubble.Say("...wow", Bubbles.Normal, target: companion);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);
                break;
            case 3:
                anim_char.Play("smallJump");
                speachBubble.Say("Was ist grün und\nhockt auf dem Klo", Bubbles.Normal, target: gameObject);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);
                anim_comp.Play("lookPuzzled");
                speachBubble.Say("...Jemand mit\nVerstopfungen?", Bubbles.Normal, target: companion);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);
                anim_comp.Play("lookAnnoyed");
                anim_player.Play("lookAnnoyed");
                anim_char.Play("lookHappy");
                speachBubble.Say("...ein Kaktus", Bubbles.Normal, target: gameObject);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                anim_comp.Play("lookAngry");
                speachBubble.Say("Deine Witze sind\nein Verbrechen gegen\ndie Menschheit!", Bubbles.Normal, target: companion);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);
                break;
            case 4:
                yield return new WaitForSeconds(.1f);
                anim_comp.Play("lookSad");
                speachBubble.Say("...Bitte nicht!", Bubbles.Normal, target: companion);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                cScript.offset = Vector2.up * 2;
                yield return new WaitForSeconds(1);
                anim_char.Play("smallJump");
                speachBubble.Say("Wo findet man\nviele Kühe?", Bubbles.Normal, target: gameObject);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);
                StartCoroutine(cScript.SetZoom(2, .5f));
                StartCoroutine(cScript.SetRotation(10, .5f));
                anim_char.Play("lookPuzzled");
                yield return new WaitForSeconds(.5f);
                speachBubble.Say("Im Muuuuseum!", Bubbles.Normal, target: gameObject);
                StartCoroutine(cScript.SetZoom(3, .5f));
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);
                yield return new WaitForSeconds(.5f);
                StartCoroutine(cScript.SetZoom(6));
                StartCoroutine(cScript.SetRotation());

                menu.SetTransition(transition);
                menu.DoTransition(1, 1, false);
                yield return new WaitForSeconds(2.4f);
                SceneManager.LoadScene("Death");
                yield break;
                break;
        }
        progress.jokeCount++;
        LoadSave.SaveProgress();

        anim_char.Play("lookNormal");
        StartCoroutine(cScript.SetZoom(8));
        EndSequence();
        yield return new WaitForSeconds(1);
        yield break;
    }
}
