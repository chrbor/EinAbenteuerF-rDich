using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static CameraScript;
using static BubbleScript;
using static MenuScript;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Seq_Jetpack : MonoBehaviour
{
    public GameObject nextScenePrefab;
    public Material transition;
    public AudioClip music;

    GameObject camFocus;

    // Start is called before the first frame update
    void Start()
    {
        runGame = true;
        pauseMove = true;
        staticCam = true;
        camFocus = transform.GetChild(0).gameObject;
        cScript.target = camFocus;
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        //Warte auf die vorgeladene Transition
        yield return new WaitForSeconds(2.4f);

        GameObject brother = transform.GetChild(1).gameObject;
        Animator anim = GetComponent<Animator>();
        yield return new WaitForSeconds(1);
        brother.transform.GetChild(0).GetComponent<Animator>().Play("lookHappy");

        speachBubble.Say("Wow, ein Jetpack!\nSo etwas habe ich mir\nschon immer gewünscht!", Bubbles.Normal, target: brother);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        yield return new WaitForSeconds(1);
        brother.transform.GetChild(0).GetComponent<Animator>().Play("lookNormal");


        //Bruder zieht sich den Jetpack an:
        anim.SetTrigger("next");//setze jetpack auf

        speachBubble.Say("Damit kann man\nüberall hinfliegen!", Bubbles.Normal, target: brother);
        speachBubble.Say("Ob es funktioniert?", Bubbles.Normal, target: brother);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        yield return new WaitForSeconds(1f);
        speachBubble.Say("3...2...1...\nSTART!!!", Bubbles.Normal, target: brother, autoPlay: true);
        cScript.aSrc.Stop();
        cScript.aSrc.clip = music;
        cScript.aSrc.volume = .7f;
        cScript.aSrc.Play();
        yield return new WaitForSeconds(1);

        anim.SetTrigger("next");//starte flug
        yield return new WaitForSeconds(.5f);

        //Starte Jetpack-animation:
        GameObject jetpack = transform.GetChild(1).GetChild(1).gameObject;
        jetpack.transform.GetChild(0).GetComponent<Animator>().Play("Thrust");
        ParticleSystem.EmissionModule emission = jetpack.transform.GetChild(1).GetComponent<ParticleSystem>().emission;
        emission.enabled = true;
        emission = jetpack.transform.GetChild(2).GetComponent<ParticleSystem>().emission;
        emission.enabled = true;

        yield return new WaitForSeconds(1.75f);
        StartCoroutine(cScript.SetZoom(20));
        cScript.offset = new Vector2(0, 9.5f);
        yield return new WaitForSeconds(2);
        player.transform.GetChild(0).GetComponent<Animator>().Play("Squeesh");
        player.GetComponent<VoiceScript>().PlayShout();

        yield return new WaitForSeconds(1);
        transform.GetChild(2).GetChild(0).GetComponent<Animator>().Play("lookHappy");
        player.transform.GetChild(0).GetComponent<Animator>().SetTrigger("startMove");


        //Laufe zum ersten Loop-Punkt:
        pauseMove = false;
        while(player.transform.position.x < -20)
        {
            camFocus.transform.position = player.transform.position;
            cScript.transform.rotation = Quaternion.Euler(0, 0, -player.GetComponent<PlayerScript>().angle);
            yield return new WaitForFixedUpdate();
        }
        pauseMove = true;

        StartCoroutine(cScript.SetRotation());
        camFocus.transform.localPosition = new Vector3(85, 5);
        yield return new WaitForSeconds(1);
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        yield return new WaitForSeconds(.1f);
        player.transform.GetChild(0).GetComponent<Animator>().SetBool("inAir", false);


        speachBubble.Say("Hhhhhiiillllllffffeee!!!", Bubbles.Shouting, fontSize: 30, target: brother);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        yield return new WaitForSeconds(2);
        anim.SetTrigger("next");
        yield return new WaitForSeconds(1);

        //Laufe zum zweiten Loop:
        pauseMove = false;
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        while (player.transform.position.x < 53)
        {
            camFocus.transform.position = player.transform.position;
            cScript.transform.rotation = Quaternion.Euler(0, 0, -player.GetComponent<PlayerScript>().angle);
            yield return new WaitForFixedUpdate();
        }
        pauseMove = true;

        StartCoroutine(cScript.SetRotation());
        camFocus.transform.localPosition = new Vector3(167, 7);
        yield return new WaitForSeconds(1);
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        yield return new WaitForSeconds(.1f);
        player.transform.GetChild(0).GetComponent<Animator>().SetBool("inAir", false);
        yield return new WaitForSeconds(1);

        speachBubble.Say("AaaAahhHhHh!!!", Bubbles.Shouting, target: brother);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        yield return new WaitForSeconds(1);
        anim.SetTrigger("next");
        yield return new WaitForSeconds((1 - anim.GetCurrentAnimatorStateInfo(0).normalizedTime) * 2.35f);
        yield return new WaitForSeconds(2.5f);

        GameObject transition_obj = menu.transform.parent.GetChild(transform.childCount - 1).gameObject;
        transition_obj.GetComponent<Image>().color = Color.white;
        transition_obj.GetComponent<Image>().material = transition;
        Material trans_mat = transition_obj.GetComponent<Image>().material;
        transition_obj.SetActive(true);

        for (float count = -.2f; count < 2.2f; count += Time.deltaTime * 4)
        {
            trans_mat.SetFloat("_end", count);
            trans_mat.SetFloat("_start", (count + 0.05f));

            yield return new WaitForEndOfFrame();
        }

        GameObject nextSeq = Instantiate(nextScenePrefab, new Vector3(69.3f, 6.3f), Quaternion.identity);
        DontDestroyOnLoad(nextSeq);
        yield return new WaitForFixedUpdate();
        SceneManager.LoadScene("World");

        yield break;
    }
}
