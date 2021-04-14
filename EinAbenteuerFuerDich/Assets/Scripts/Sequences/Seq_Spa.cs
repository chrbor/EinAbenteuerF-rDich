using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameManager;
using static CameraScript;
using static BubbleScript;
using static MenuScript;

public class Seq_Spa : MonoBehaviour
{
    public Material transition;

    [HideInInspector]
    public bool glassReceived;
    bool brewReceived;

    bool talking;

    private Animator anim;
    private Animator anim_comp;
    private Animator anim_player;

    private GameObject camFocus;
    private GameObject bulb;
    private GameObject glass;

    public void GetGlass(GameObject _glass) => StartCoroutine(GlassSeq(_glass));
    public void GetBrew(GameObject _spa) => StartCoroutine(BrewSeq(_spa));


    private void Start()
    {
        anim = GetComponent<Animator>();
        anim_comp = companion.transform.GetChild(0).GetComponent<Animator>();
        anim_player = player.transform.GetChild(0).GetComponent<Animator>();
        camFocus = transform.GetChild(0).gameObject;
        bulb = transform.GetChild(1).gameObject;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (talking || runGame) return;
        runGame = true;
        StartCoroutine(GoBackDialog());
    }

    IEnumerator GlassSeq(GameObject _glass)
    {
        glass = _glass;
        glass.GetComponent<Collider2D>().enabled = false;
        pauseMove = true;
        staticCam = true;

        glass.transform.parent = player.transform;
        Vector2 diff = glass.transform.localPosition;
        AnimationCurve x_curve = AnimationCurve.EaseInOut(0, diff.x, 1, 0);
        AnimationCurve y_curve = AnimationCurve.EaseInOut(0, diff.y, 1, 5);

        StartCoroutine(cScript.SetZoom(5));
        StartCoroutine(cScript.SetRotation());
        for (float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            glass.transform.localPosition = new Vector3(x_curve.Evaluate(count), y_curve.Evaluate(count));
            glass.transform.localScale += Vector3.one * 0.01f;
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(1);
        StartCoroutine(cScript.SetZoom(8));
        yield return new WaitForSeconds(1);
        pauseMove = false;
        staticCam = false;

        SpriteRenderer glassSprite = glass.transform.GetChild(0).GetComponent<SpriteRenderer>();
        Color step = Color.black * Time.fixedDeltaTime;
        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            glassSprite.color -= step;
            glass.transform.localPosition += Vector3.down * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        glass.SetActive(false);
        glassReceived = true;
        yield break;
    }

    IEnumerator BrewSeq(GameObject _spa)
    {
        pauseMove = true;
        staticCam = true;

        glass.SetActive(true);
        SpriteRenderer glassSprite = glass.transform.GetChild(0).GetComponent<SpriteRenderer>();
        SpriteRenderer stickerSprite = glassSprite.transform.GetChild(0).GetComponent<SpriteRenderer>();
        Color step = Color.black * Time.fixedDeltaTime;
        for (float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            glassSprite.color += step;
            glass.transform.localPosition += Vector3.up * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        cScript.target = _spa;
        StartCoroutine(cScript.SetZoom(5));
        StartCoroutine(cScript.SetRotation());

        glassSprite.sortingLayerName = "Default";
        glass.transform.parent = _spa.transform;
        Vector2 diff = glass.transform.localPosition;
        AnimationCurve x_curve = AnimationCurve.EaseInOut(0, diff.x, 1, 0);
        AnimationCurve y_curve = AnimationCurve.EaseInOut(0, diff.y, 1, 3);

        for (float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            glass.transform.localPosition = new Vector3(x_curve.Evaluate(count), y_curve.Evaluate(count));
            yield return new WaitForFixedUpdate();
        }

        glass.GetComponent<Animator>().Play("GetBrew");
        yield return new WaitForSeconds(2);

        cScript.target = player;
        StartCoroutine(cScript.SetZoom(8));

        glass.transform.parent = player.transform;
        diff = glass.transform.localPosition;
        x_curve = AnimationCurve.EaseInOut(0, diff.x, 1, 0);
        y_curve = AnimationCurve.EaseInOut(0, diff.y, 1, 5);

        for (float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            glass.transform.localPosition = new Vector3(x_curve.Evaluate(count), y_curve.Evaluate(count));
            yield return new WaitForFixedUpdate();
        }

        glassSprite.sortingLayerName = "Forground";
        for (float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            glassSprite.color -= step;
            stickerSprite.color -= step;
            glass.transform.localPosition += Vector3.down * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        staticCam = false;
        pauseMove = false;
        brewReceived = true;
        yield break;
    }

    IEnumerator GoBackDialog()
    {
        pauseMove = true;
        staticCam = true;

        StartCoroutine(cScript.SetRotation(0, 2));
        StartCoroutine(cScript.SetZoom(6, 2));
        cScript.offset = Vector2.up * 2;

        companion.GetComponent<FollowScript>().enabled = false;
        yield return new WaitForSeconds(.1f);
        anim_comp.SetBool("moving", true);
        anim_comp.SetBool("inAir", false);
        anim_comp.SetTrigger("startMove");
        float comp_vel = 5 * Time.fixedDeltaTime;
        while (companion.transform.localPosition.x - player.transform.localPosition.x > -5)
        {
            companion.transform.position += Vector3.left * comp_vel;
            yield return new WaitForFixedUpdate();
        }
        anim_comp.SetBool("moving", false);
        anim_comp.Play("Squeesh");

        if (brewReceived)
        {
            speachBubble.Say("Wir haben die Eier\nund die Brühe", Bubbles.Normal, target: companion);
            speachBubble.Say("Lass uns zurückgehen.", Bubbles.Normal, target: companion);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);

            StartCoroutine(GoBack());
            yield break;
        }

        anim_comp.Play("lookSad");
        speachBubble.Say("Wir haben noch nicht\ndie Brühe!", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        anim_comp.Play("lookNormal");
        speachBubble.Say("Sollen wir trotzdem\nzurückgehen?", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        //Entscheidung hier einfügen
        menu.SetDecisionActive(true);
        yield return new WaitUntil(() => menu.decisionMade);
        yield return new WaitForSeconds(1);

        if (menu.useLeftDecision) { StartCoroutine(GoBack()); yield break; }


        anim_player.Play("Squeesh");
        player.GetComponent<VoiceScript>().PlayExplain();
        yield return new WaitForSeconds(1);
        StartCoroutine(Tipp());

        yield break;
    }

    IEnumerator GoBack()
    {
        anim_player.Play("Squeesh");
        player.GetComponent<VoiceScript>().PlayShout();
        yield return new WaitForSeconds(1);

        //Blende:
        GameObject transition = canvas.transform.GetChild(canvas.transform.childCount - 1).gameObject;
        Material trans_mat = transition.GetComponent<Image>().material;
        Mat_Intro = trans_mat;
        transition.SetActive(true);
        for (float count = -1.1f; count < 1.1f; count += Time.deltaTime)
        {
            trans_mat.SetFloat("_end", -count);
            trans_mat.SetFloat("_start", -(count + 0.05f));

            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene("World");
        yield break;
    }

    public IEnumerator Tipp()
    {
        Debug.Log("playing Tipp");

        yield return new WaitForSeconds(.1f);
        companion.GetComponent<FollowScript>().enabled = false;
        pauseMove = true;
        runGame = true;

        if (!glassReceived)
        {
            anim_comp.Play("lookFocused");
            speachBubble.Say("Hhhhmmmm....\nWo kriegen wir die\nBrühe her?", Bubbles.Normal, target: companion);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);


            anim_comp.Play("Squeesh");
            bulb.transform.position = companion.transform.position;
            bulb.SetActive(true);
            bulb.GetComponent<Animator>().Play("Idea");
            yield return new WaitForSeconds(1.5f);
            bulb.SetActive(false);

            speachBubble.Say("Ich habe eine Idee!\nAber dafür müssen wir\nzuerst einen Behälter\nfinden.", Bubbles.Normal, target: companion);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);
            anim_comp.Play("lookPuzzled");
            speachBubble.Say("Am besten wäre ein Glas\noder ein Becher", Bubbles.Normal, target: companion);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);
            anim_comp.SetTrigger("startMove");
            speachBubble.Say("Wenn du einen siehst dann\ntippe ihn an, um ihn\nmitzunehmen", Bubbles.Normal, target: companion);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);

        }
        else
        {
            anim_comp.Play("lookNormal");
            speachBubble.Say("Das Glas haben wir!\nJetzt müssen wir nur noch\ndie kochende Brühe holen", Bubbles.Normal, target: companion);
            yield return new WaitUntil(() => !speachBubble.finished);
            yield return new WaitUntil(() => speachBubble.finished);

            anim_player.Play("lookSad");
            player.GetComponent<VoiceScript>().PlayQuestion();
            yield return new WaitForSeconds(1);
            anim_comp.SetTrigger("startMove");
        }

        player.GetComponent<PlayerScript>().enabled = false;
        companion.GetComponent<FollowScript>().enabled = true;
        yield return new WaitForSeconds(1f);
        anim_player.SetBool("inAir", false);
        anim_player.SetBool("moving", true);
        anim_player.SetTrigger("startMove");
        while(player.transform.localPosition.x < -100)
        {
            player.transform.position += Vector3.right * .1f;
            yield return new WaitForFixedUpdate();
        }
        anim_player.SetBool("moving", false);

        anim_comp.SetBool("inAir", false);
        anim_comp.SetTrigger("startMove");
        player.GetComponent<PlayerScript>().enabled = true;

        GetComponent<Collider2D>().enabled = true;

        staticCam = false;
        pauseMove = false;
        runGame = false;
        yield break;
    }
}
