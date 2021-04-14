using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameManager;
using static CameraScript;
using static BubbleScript;

public class Seq_DestroyedMill : MonoBehaviour
{
    public Material mat_explsn;
    private Material mat_busTrans;

    public AudioClip tumbleClip;

    GameObject camFocus;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode) => StartCoroutine(StartSequence());
    private void OnDestroy() => SceneManager.sceneLoaded -= OnLevelLoaded;

    IEnumerator StartSequence()
    {
        Animator anim = GetComponent<Animator>();
        AudioSource aSrc_player = player.GetComponent<AudioSource>();
        cScript.aSrc.Stop();

        runGame = true;
        pauseMove = true;
        staticCam = true;
        camFocus = transform.GetChild(0).gameObject;
        cScript.target = MillScript.millSeq_Object;
        cScript.offset = Vector3.up * 5;
        cScript.transform.position = cScript.target.transform.position + Vector3.back * 10;
        camFocus.transform.localPosition = Vector3.zero;

        MillScript.millSeq_Object.transform.GetChild(1).gameObject.SetActive(true);

        //blende:
        GameObject transition = canvas.transform.GetChild(canvas.transform.childCount - 1).gameObject;
        transition.SetActive(true);
        mat_busTrans = transition.GetComponent<Image>().material;
        transition.GetComponent<Image>().material = mat_explsn;
        transition.GetComponent<Image>().color = Color.white;
        Material mat_trans = transition.GetComponent<Image>().material;
        mat_trans.SetFloat("_start", 1);
        mat_trans.SetFloat("_end", 0.95f);
        yield return new WaitForFixedUpdate();

        StartCoroutine(TasklistScript.SetTaskList(false));


        //Bereite Player und Companion für die Sequenz vor:
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        companion.GetComponent<FollowScript>().enabled = false;
        yield return new WaitForSeconds(.1f);

        //Positioniere Companion und Spieler:
        player.transform.position = new Vector3(51, -2);
        companion.transform.position = new Vector3(63, 4.5f);
        companion.transform.eulerAngles = new Vector3(0, 0, 122);
        companion.transform.GetChild(0).GetComponent<Animator>().SetBool("inAir", false);
        player.transform.GetChild(0).GetComponent<Animator>().SetBool("inAir", false);
        player.transform.parent = transform;
        companion.transform.parent = transform;
        anim.Rebind();

        cScript.aSrc.clip = tumbleClip;
        cScript.aSrc.loop = false;
        cScript.aSrc.Play();

        //blende:
        mat_trans.SetFloat("_end", 1);
        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            mat_trans.SetFloat("_start", count * 5 - 4);
            yield return new WaitForFixedUpdate();
        }
        transition.SetActive(false);
        transition.GetComponent<Image>().color = Color.black;
        transition.GetComponent<Image>().material = mat_busTrans;
        StartCoroutine(cScript.SetZoom(12, 2));

        yield return new WaitForSeconds(3);

        manager.PlayNormal();

        cScript.strength = 0.2f;
        StartCoroutine(cScript.SetZoom(3, 0.5f));
        StartCoroutine(cScript.SetRotation(-30, .5f));
        cScript.target = MillScript.millSeq_Object.transform.GetChild(1).gameObject;
        cScript.offset = Vector2.up * 3;
        cScript.target.transform.GetChild(0).GetComponent<Animator>().Play("lookFurious");

        speachBubble.Say("MEINE MÜHLE!\nArrrrgh!!!", Bubbles.Shouting, fontSize: 35);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        cScript.strength = 0.05f;
        StartCoroutine(cScript.SetZoom(8, 2));
        StartCoroutine(cScript.SetRotation(0, 2));
        yield return new WaitForSeconds(1);
        camFocus.transform.localPosition = new Vector3(-13.5f, -8f);
        cScript.target = camFocus;

        yield return new WaitForSeconds(2);
        companion.transform.GetChild(0).GetComponent<Animator>().Play("lookInPain");

        speachBubble.Say("Aua!", Bubbles.Normal, fontSize: 20, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        companion.transform.GetChild(0).GetComponent<Animator>().Play("lookNormal");
        anim.Play("StandUp");
        yield return new WaitForSeconds(4.5f);
        companion.transform.GetChild(0).GetComponent<Animator>().Play("lookSad");

        speachBubble.Say("Vielleicht sollten wir\nlieber verschwinden!", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        player.transform.GetChild(0).GetComponent<Animator>().Play("lookAnnoyed");
        player.GetComponent<VoiceScript>().PlayMumble();
        yield return new WaitForSeconds(1);

        StartCoroutine(TasklistScript.SetTaskList(true));
        yield return new WaitForSeconds(.8f);

        companion.transform.GetChild(0).GetComponent<Animator>().Play("lookNormal");
        yield return new WaitForSeconds(.1f);
        player.transform.GetChild(0).GetComponent<Animator>().Play("lookNormal");


        yield return new WaitForSeconds(.1f);
        player.transform.parent = null;
        companion.transform.parent = null;
        SceneManager.MoveGameObjectToScene(player, SceneManager.GetActiveScene());//cuz I am stupid!
        SceneManager.MoveGameObjectToScene(companion, SceneManager.GetActiveScene());

        runGame = false;
        staticCam = false;
        pauseMove = false;
        companion.GetComponent<FollowScript>().enabled = true;

        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        player.transform.GetChild(0).GetComponent<Animator>().SetTrigger("startMove");
        companion.transform.GetChild(0).GetComponent<Animator>().SetTrigger("startMove");
        cScript.target = player;
        progress.startSeqDone = true;
        LoadSave.SaveProgress();
        Destroy(gameObject);
        yield break;
    }
}
