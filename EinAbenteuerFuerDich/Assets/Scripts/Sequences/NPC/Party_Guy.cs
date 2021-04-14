using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;
using static CameraScript;
using static BubbleScript;

public class Party_Guy : TalkScript
{
    public Material transition;
    public GameObject package;

    protected override IEnumerator PlaySequence()
    {
        StartCoroutine(Setup());
        StartCoroutine(cScript.SetZoom(6));
        yield return new WaitForSeconds(1);

        anim_comp.Play("lookScared");
        speachBubble.Say("Hey, hey du!", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        speachBubble.Say("Tut mir leid,\ndass die Kiste so\nviel Ärger gemacht\nhat", Bubbles.Normal, target: gameObject);
        speachBubble.Say("Ich mein: Wer hätte\ngedacht das da ein\nJetpack verpackt\nist?", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookAngry");
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(1);

        anim_comp.Play("lookNormal");
        anim_char.Play("lookAnnoyed");
        speachBubble.Say("Jaja,\nich habe es\nverstanden!", Bubbles.Normal, target: gameObject);
        speachBubble.Say("Ich mache keine\nzwielichtigen Geschäfte\nmehr", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_player.Play("lookNormal");
        anim_char.Play("smallJump");
        speachBubble.Say("Als Wiedergutmachung\nHabe ich euch sogar\nnoch ein weiteres\nGeschenk mitgebracht!", Bubbles.Normal, target: gameObject);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        //Guy wirft Paket:
        GameObject myBox = Instantiate(package, gameObject.transform.position + Vector3.up * 5, Quaternion.identity);
        float x_diff = player.transform.position.x - transform.position.x;
        myBox.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Abs(x_diff) > 3 ? (x_diff > 0 ? 3 : -3) : (x_diff > 0 ? -3 : 3), 10);

        SpriteRenderer sprite_myBox = myBox.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
        sprite_myBox.color = new Color(1, 1, 1, 0);
        for (float count = 0; count < 1; count += Time.fixedDeltaTime) { sprite_myBox.color = new Color(1, 1, 1, count); yield return new WaitForFixedUpdate(); }

        cScript.target = myBox;
        cScript.offset = Vector2.up * 2;

        yield return new WaitForSeconds(1);
        anim_comp.Play("lookPuzzled");
        speachBubble.Say("Hhmmm, ich frage mich,\nwas in dem Paket\ndrinnen ist", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_comp.Play("lookHappy");
        speachBubble.Say("Vielleicht ist dies\nder Start unseres\nnächsten Abenteuers!", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);


        anim_player.Play("lookSad");
        player.GetComponent<VoiceScript>().PlayQuestion();
        yield return new WaitForSeconds(1);

        StartCoroutine(DoTransition());

        StartCoroutine(cScript.SetZoom(8, .5f));
        StartCoroutine(cScript.SetRotation(10, .5f));
        yield return new WaitForSeconds(.75f);
        StartCoroutine(cScript.SetZoom(1, .75f));
        StartCoroutine(cScript.SetRotation(30, .75f));
        yield return new WaitForSeconds(.75f);

        SceneManager.LoadScene("Credits");
        yield break;
    }

    IEnumerator DoTransition()
    {
        float timeStep = Time.fixedDeltaTime / 1.5f;
        Image image = canvas.transform.GetChild(canvas.transform.childCount - 1).GetComponent<Image>();
        image.material = transition;
        Material mat = image.material;
        image.gameObject.SetActive(true);
        mat.SetFloat("_start", 0);
        for(float count = 1; count > 0; count -= timeStep)
        {
            mat.SetFloat("_end", count * 10);
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }
}
