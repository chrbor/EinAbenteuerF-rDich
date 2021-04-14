using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static BubbleScript;
using static CameraScript;

public class BuildedHouseScript : MonoBehaviour
{
    bool talking;
    ParticleSystem.EmissionModule steam;

    private void Start()
    {
        steam = transform.GetChild(2).GetComponent<ParticleSystem>().emission;

        if(progress.millState % 3 == 1)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            GetComponent<Collider2D>().enabled = false;
            steam.enabled = false;
        }
        else steam.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.GetComponent<TouchSensor>().tipped || runGame || talking || other.gameObject.layer != 14/*Touch*/) return;
        runGame = true;
        pauseMove = true;
        talking = true;
        staticCam = true;
        StartCoroutine(Talk());
    }

    IEnumerator Talk()
    {
        TasklistScript.SetTaskList(false);

        yield return new WaitForSeconds(1);

        StartCoroutine(cScript.SetRotation());
        cScript.offset = Vector2.up * 2;
        cScript.target = transform.GetChild(1).gameObject;
        player.transform.GetChild(0).GetComponent<Animator>().Play("Squeesh");
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(2);
        player.transform.GetChild(0).GetComponent<Animator>().Play("Idle");

        Animator anim = GetComponent<Animator>();
        anim.SetBool("right", player.transform.position.x > transform.GetChild(1).position.x);
        anim.SetTrigger("stopBuild");
        transform.GetChild(0).GetComponent<Collider2D>().enabled = false;

        steam.enabled = false;
        GameObject hammer = transform.GetChild(1).GetChild(0).gameObject;
        Animator anim_ham = transform.GetChild(1).GetChild(0).GetComponent<Animator>();
        anim_ham.Play("Squeesh");
        yield return new WaitForSeconds(1);
        speachBubble.Say("Oh, Hallo!\nDas Haus ist leider\nnoch nicht fertig.", Bubbles.Normal, target: hammer);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_ham.Play("lookHappy");
        speachBubble.Say("Falls ihr auch ein Haus\ngebaut haben wollt, dann\nbesucht doch die geheime\nGilde der Handwerker!", Bubbles.Normal, target: hammer);
        speachBubble.Say("Ihr findet sie direkt\nam anderen Ende\nder Stadt.", Bubbles.Normal, target: hammer);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_ham.Play("lookFocused");
        speachBubble.Say("Aber sagt das keinem weiter.\nSie ist schließlich geheim!", Bubbles.Normal, target: hammer);
        speachBubble.Say("Niemand, der das\nKlopfpasswort nicht kennt,\nkommt da rein.", Bubbles.Normal, target: hammer);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_ham.Play("lookHappy");
        speachBubble.Say("So, jetzt muss ich aber\nweiterarbeiten. Ein Haus\nbaut sich ja nicht\nvon allein!!", Bubbles.Normal, target: hammer);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_ham.Play("lookNormal");
        yield return new WaitForSeconds(.3f);

        transform.GetChild(0).GetComponent<Collider2D>().enabled = true;
        anim.Play("BuildHouse");
        steam.enabled = true;

        cScript.target = companion;
        yield return new WaitForSeconds(1.5f);
        companion.transform.GetChild(0).GetComponent<Animator>().Play("lookFocused");

        speachBubble.Say("Hhhhhmmmmm...", Bubbles.Normal, target: companion);
        speachBubble.Say("Das Hämmern hört sich\nirgendwie schon fast wie\nein Klopfzeichen an... ", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        player.transform.GetChild(0).GetComponent<Animator>().Play("lookHappy");
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(2);

        player.transform.GetChild(0).GetComponent<Animator>().Play("lookNormal");
        yield return new WaitForSeconds(.1f);
        companion.transform.GetChild(0).GetComponent<Animator>().Play("lookNormal");
        yield return new WaitForSeconds(.3f);

        talking = false;
        pauseMove = false;
        runGame = false;
        staticCam = false;
        cScript.target = player;
        companion.transform.GetChild(0).GetComponent<Animator>().SetTrigger("startMove");
        player.transform.GetChild(0).GetComponent<Animator>().SetTrigger("startMove");
        TasklistScript.SetTaskList(true);
        yield break;
    }
}
