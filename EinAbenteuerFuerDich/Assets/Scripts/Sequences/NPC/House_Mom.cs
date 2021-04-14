using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;
using static CameraScript;
using static BubbleScript;
using static MenuScript;

public class House_Mom : TalkScript
{
    public AnimationCurve flightVector;
    public Material transition;
    public GameObject pot;

    protected override IEnumerator PlaySequence()
    {
        StartCoroutine(Setup());
        StartCoroutine(cScript.SetZoom(6));
        yield return new WaitForSeconds(1);

        anim_char.Play("smallJump");
        speachBubble.Say("Ah, da seit ihr wieder\nHabt ihr alle Zutaten\nmitgebracht?", Bubbles.Normal, target: gameObject);
        menu.SetDecisionActive(true);
        yield return new WaitUntil(() => menu.decisionMade);
        menu.SetDecisionActive(false);
        yield return new WaitForSeconds(1);


        bool gotAll = progress.brewDone && progress.huntDone && progress.marketDone && progress.potatoDone && progress.millState > 2;
        if (menu.useLeftDecision)//Abbruch
        {
            anim_comp.Play("lookInPain");
            speachBubble.Say("Ähm,...\nda fällt mir ein,\ndass wir noch etwas\nvergessen haben", Bubbles.Normal, target: companion);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);

            anim_char.Play("lookHappy");
            speachBubble.Say("Gut,\naber lasst euch nicht\nzu viel Zeit!", Bubbles.Normal, target: gameObject);
            speachBubble.Say("Der Kuchen muss\nschließlich noch\ngebacken werden!", Bubbles.Normal, target: gameObject);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);

            anim_char.Play("lookNormal");
            StartCoroutine(cScript.SetZoom(8));
            yield return new WaitForSeconds(1);
            EndSequence();
            yield break;
        }
        else
        {
            anim_comp.Play("lookHappy");
            speachBubble.Say("Yup, wir haben\nalles", Bubbles.Normal, target: companion);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);

            anim_char.Play("lookHappy");
            speachBubble.Say("Toll!\nDann werft mal alles\nbitte in den Topf", Bubbles.Normal, target: gameObject);
            speachBubble.Say("Damit ich den\nKuchen backen kann", Bubbles.Normal, target: gameObject);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);
            anim_char.Play("lookNormal");

            Animator anim_pot = pot.GetComponent<Animator>();

            anim_pot.Play("showPot");
            yield return new WaitForSeconds(1);


            GameObject stuff = GameObject.Find("Stuff");
            AudioSource aSrc = GetComponent<AudioSource>();
            aSrc.pitch = 1;
            if(progress.potatoDone) { StartCoroutine(ThrowIntoPot(stuff.transform.GetChild(0).gameObject, pot)); yield return new WaitForSeconds(1); aSrc.pitch += .1f; aSrc.Play(); }
            if(progress.millState > 2) { StartCoroutine(ThrowIntoPot(stuff.transform.GetChild(1).gameObject, pot)); yield return new WaitForSeconds(1); aSrc.pitch += .1f; aSrc.Play(); }
            if(progress.huntDone) { StartCoroutine(ThrowIntoPot(stuff.transform.GetChild(2).gameObject, pot)); yield return new WaitForSeconds(1); aSrc.pitch += .1f; aSrc.Play(); }
            if(progress.brewDone) { StartCoroutine(ThrowIntoPot(stuff.transform.GetChild(3).gameObject, pot)); yield return new WaitForSeconds(1); aSrc.pitch += .1f; aSrc.Play(); }
            if(progress.marketDone) { StartCoroutine(ThrowIntoPot(stuff.transform.GetChild(4).gameObject, pot)); yield return new WaitForSeconds(1); aSrc.pitch += .1f; aSrc.Play(); }
            yield return new WaitForSeconds(1);

            if (!gotAll)
            {
                string missing = "";
                if (!progress.potatoDone) missing = "die Kartoffeln";
                if (progress.millState < 3) missing = "das Mehl";
                if (!progress.huntDone) missing = "die Eier";
                if (!progress.brewDone) missing = "die Hühnerbrühe";
                if (!progress.marketDone) missing = "den anderen Kram";

                anim_char.Play("JumpScare");
                speachBubble.Say("Moment mal,\nda fehlt doch was!", Bubbles.Normal, target: gameObject);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);
                anim_player.Play("lookSad");
                anim_comp.Play("lookSad");
                anim_char.Play("smallJump");
                speachBubble.Say("Ihr habt\n" +  missing + "\nvergessen!", Bubbles.Normal, target: gameObject);
                speachBubble.Say("Schnell wieder alles\nraus damit", Bubbles.Normal, target: gameObject);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                if (progress.potatoDone) { StartCoroutine(ThrowOutOfPot(stuff.transform.GetChild(0).gameObject)); yield return new WaitForSeconds(.5f); aSrc.pitch -= .1f; aSrc.Play(); }
                if (progress.millState > 2) { StartCoroutine(ThrowOutOfPot(stuff.transform.GetChild(1).gameObject)); yield return new WaitForSeconds(.5f); aSrc.pitch -= .1f; aSrc.Play(); }
                if (progress.huntDone) { StartCoroutine(ThrowOutOfPot(stuff.transform.GetChild(2).gameObject)); yield return new WaitForSeconds(.5f); aSrc.pitch -= .1f; aSrc.Play(); }
                if (progress.brewDone) { StartCoroutine(ThrowOutOfPot(stuff.transform.GetChild(3).gameObject)); yield return new WaitForSeconds(.5f); aSrc.pitch -= .1f; aSrc.Play(); }
                if (progress.marketDone) { StartCoroutine(ThrowOutOfPot(stuff.transform.GetChild(4).gameObject)); yield return new WaitForSeconds(.5f); aSrc.pitch -= .1f; aSrc.Play(); }
                yield return new WaitForSeconds(.5f);

                anim_char.Play("lookSad");
                speachBubble.Say("Das war knapp!!!", Bubbles.Normal, target: gameObject);
                speachBubble.Say("Ohne\n" + missing + "\nkann ich den Kartoffel-\nkuchen nicht backen", Bubbles.Normal, target: gameObject);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                anim_char.Play("JumpScare");
                speachBubble.Say("Das ist die\nwichtigste Zutat\nvon allen!!!", Bubbles.Normal, target: gameObject);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                anim_pot.SetTrigger("putBack");
            }
            else
            {
                anim_pot.SetTrigger("putBack");
                yield return new WaitForSeconds(1);
                anim_pot.Play("cook");

                anim_char.Play("lookHappy");
                speachBubble.Say("Perfekt!\njetzt muss der Kuchen\nnoch für die nächsten\n3 Stunden backen", Bubbles.Normal, target: gameObject);
                speachBubble.Say("Und dann kann\ndie Feier beginnen!", Bubbles.Normal, target: gameObject);
                yield return new WaitUntil(() => !speachBubble.finished);
                yield return new WaitUntil(() => speachBubble.finished);

                anim_player.Play("lookHappy");
                yield return new WaitForSeconds(.1f);
                anim_comp.Play("lookHappy");

                menu.SetTransition(transition);
                menu.DoTransition(1, 1, false);
                Mat_Intro = transition;

                yield return new WaitForSeconds(2.4f);
                SceneManager.LoadScene("PreParty");
            }

        }

        anim_char.Play("lookNormal");
        StartCoroutine(cScript.SetZoom(8));
        yield return new WaitForSeconds(1);
        EndSequence();
        yield break;
    }

    IEnumerator ThrowIntoPot(GameObject ingredient, GameObject pot)
    {
        ingredient.transform.position = player.transform.position + Vector3.up * 3;

        //Zeige dich:
        Vector3 step_pos = Vector3.up * 4 * Time.fixedDeltaTime;
        ingredient.SetActive(true);
        for(float count = 0; count < .5f; count += Time.fixedDeltaTime)
        {
            ingredient.transform.position += step_pos;
            for (int i = 0; i < ingredient.transform.childCount; i++) ingredient.transform.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, count);
            yield return new WaitForFixedUpdate();
        }
        step_pos /= 2;
        for (float count = 0.5f; count < 1; count += Time.fixedDeltaTime)
        {
            ingredient.transform.position -= step_pos;
            for (int i = 0; i < ingredient.transform.childCount; i++) ingredient.transform.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, count);
            yield return new WaitForFixedUpdate();
        }

        //Fliege in den Eimer
        step_pos = (pot.transform.position - ingredient.transform.position) * Time.fixedDeltaTime;
        Vector3 virtPos = ingredient.transform.position;
        for(float count = 0; count < 1; count+=Time.fixedDeltaTime)
        {
            virtPos += step_pos;
            ingredient.transform.position = virtPos + Vector3.up * flightVector.Evaluate(count);
            yield return new WaitForFixedUpdate();
        }
        ingredient.SetActive(false);
        yield break;
    }

    IEnumerator ThrowOutOfPot(GameObject ingredient)
    {
        ingredient.SetActive(true);

        //Fliege in den Eimer
        Vector3 step_pos = (player.transform.position + Vector3.up * 2 - ingredient.transform.position) * Time.fixedDeltaTime;
        Vector3 virtPos = ingredient.transform.position;
        for (float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            virtPos += step_pos;
            ingredient.transform.position = virtPos + Vector3.up * flightVector.Evaluate(count);
            for (int i = 0; i < ingredient.transform.childCount; i++) ingredient.transform.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1 - count);
            yield return new WaitForFixedUpdate();
        }
        ingredient.SetActive(false);
        yield break;
    }
}