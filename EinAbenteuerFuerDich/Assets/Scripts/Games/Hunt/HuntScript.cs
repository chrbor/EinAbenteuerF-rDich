using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static GameManager;
using static CameraScript;
using static BubbleScript;
using static MenuScript;

public class HuntScript : MonoBehaviour
{
    public float runVelocity;
    [Min(5)]
    public float minDist = 5;
    [Min(2)]
    public float maxView = 30;
    [Min(5)]
    public float startDist = 30;
    [HideInInspector]
    public float preyDist;
    public float eggDistance;
    [Min(0.1f)]
    public float eggTime;
    [HideInInspector]
    public float eggCount;
    public float eggGoal = 6;
    public GameObject egg;
    public HunterScript hunter;
    public AnimationCurve jumpCurve;
    private bool isJumping;

    public AudioClip[] chickenClips;
    private AudioSource aSrc;
    private AudioClip raceMusic;

    //follower:
    private List<Vector3> prevPos = new List<Vector3>();

    //HuhnSteuerung:
    int mask = (1 << 10);//Obstacle

    public GameObject ground;
    public GameObject[] obstacles;
    public GameObject[] decoration;
    public List<GameObject> activeGrounds;
    public GameObject spa;

    private float eggTimer;
    private float realVel;
    private Animator anim;

    private void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
        aSrc = GetComponent<AudioSource>();
        raceMusic = cScript.aSrc.clip;
        realVel = (runVelocity + difficulty * 5) * Time.fixedDeltaTime;
        player_startPos = new Vector2(270, -2);

        StartCoroutine(GetIntoStartPosition());
    }

    IEnumerator GetIntoStartPosition()
    {
        yield return new WaitForFixedUpdate();
        companion.GetComponent<FollowScript>().enabled = false;
        yield return new WaitForSeconds(.1f);
        Animator anim_comp = companion.transform.GetChild(0).GetComponent<Animator>();
        anim_comp.SetBool("moving", false);
        anim_comp.SetBool("inAir", false);

        transform.position = new Vector3(4.9f, 1.7f);
        transform.localScale = new Vector3(-.4f, .4f, 1);
        cScript.strength = .03f;
        cScript.target = gameObject;
        StartCoroutine(cScript.SetZoom(6, .5f));
        StartCoroutine(cScript.SetRotation(.5f));
        yield return new WaitForSeconds(1);
        anim.Play("Idle");
        aSrc.clip = chickenClips[0];
        aSrc.Play();

        yield return new WaitForSeconds(1);

        cScript.target = player;
        cScript.offset = new Vector2(2, 3);
        cScript.strength = .1f;
        yield return new WaitForSeconds(.75f);
        companion.transform.GetChild(0).GetComponent<Animator>().Play("JumpScare");
        speachBubble.Say("DA IST EIN HUHN!\nSCHNAPPEN WIR ES UNS!!!", Bubbles.Shouting, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);
        cScript.target = gameObject;
        cScript.strength = .2f;
        StartCoroutine(cScript.SetRotation(10, .5f));
        StartCoroutine(cScript.SetZoom(.5f, .5f));
        cScript.offset = new Vector2(-1f, .75f);
        anim.Play("lookNormal");
        yield return new WaitForSeconds(1f);
        anim.Play("lookSad");

        aSrc.Play();
        yield return new WaitForSeconds(1f);
        aSrc.clip = chickenClips[1];
        aSrc.Play();
        yield return new WaitForSeconds(.5f);
        StartCoroutine(cScript.SetZoom(6, .5f));
        StartCoroutine(cScript.SetRotation(0, .5f));
        cScript.strength = .05f;
        anim.Play("Jump");
        transform.localScale = Vector3.one * .4f;
        Vector3 startPos = transform.position;
        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            transform.position = startPos + Vector3.up * jumpCurve.Evaluate(count);
            yield return new WaitForFixedUpdate();
        }

        cScript.strength = .05f;
        StartCoroutine(RunHuntGame());

        //yield return new WaitForSeconds(2);
        //eggCount = 6;

        yield break;
    }

    IEnumerator LoseSequence()
    {
        //Setze Boden:
        Vector3 chickenSpeed = Vector3.right * .1f;
        for(float count = 0; count < 2; count += Time.fixedDeltaTime)
        {
            SetGround();
            transform.position += chickenSpeed;

            prevPos.Add(hunter.transform.position);
            companion.transform.position = prevPos[0] + Vector3.left * 5;
            if (prevPos.Count > 10) prevPos.RemoveAt(0);

            yield return new WaitForFixedUpdate();
        }
        runGame = false;
        cScript.target = player;
        cScript.strength = .03f;
        cScript.offset = Vector2.up * 2;
        StartCoroutine(cScript.SetZoom(8, 2));
        hunter.anim.SetBool("moving", false);

        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            companion.transform.position += Vector3.right * realVel / 2;
            yield return new WaitForFixedUpdate();
        }

        Animator anim_comp = companion.transform.GetChild(0).GetComponent<Animator>();
        anim_comp.SetBool("moving", false);
        yield return new WaitForSeconds(.5f);
        anim_comp.Play("Squeesh");
        yield return new WaitForSeconds(1);

        anim_comp.Play("lookSad");

        speachBubble.Say("Oh nein!\nDas Huhn ist uns\nentkommen.", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        hunter.GetComponent<VoiceScript>().PlayMumble();
        hunter.anim.Play("lookSad");
        yield return new WaitForSeconds(1);

        anim_comp.Play("lookAngry");
        yield return new WaitForSeconds(.5f);
 
        speachBubble.Say("Wir dürfen nicht aufgeben.\nEs steht ein\nKartoffelkuchen auf\ndem Spiel", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        anim_comp.Play("lookNormal");

        speachBubble.Say("...Wollen wir weitersuchen?", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        yield return new WaitForSeconds(.1f);
        hunter.anim.Play("lookNormal");
        yield return new WaitForSeconds(1);

        //Entscheidung hier einfügen
        menu.SetDecisionActive(true);
        yield return new WaitForSeconds(.1f);
        yield return new WaitUntil(() => menu.decisionMade);

        if(!menu.useLeftDecision)
        {
            StartCoroutine(GetIntoStartPosition());
            yield break;
        }
        
        //Transition zu der HauptWelt:
        GameObject transition = canvas.transform.GetChild(canvas.transform.childCount - 1).gameObject;
        Material trans_mat = transition.GetComponent<Image>().material;
        transition.SetActive(true);
        for (float count = -1.1f; count < 1.1f; count += Time.deltaTime)
        {
            trans_mat.SetFloat("_end", count);
            trans_mat.SetFloat("_start", (count + 0.05f));

            yield return new WaitForEndOfFrame();
        }
        //DontDestroyOnLoad(Instantiate(Seq_afterHunt));
        SceneManager.LoadScene("World");
        
        yield break;
    }

    IEnumerator WinSequence()
    {
        runGame = false;

        //Renne bis zum nächsten ground:
        while (activeGrounds[0].transform.position.x >= -maxView * Camera.main.aspect - 75)
        {
            foreach (var ground in activeGrounds) ground.transform.position += Vector3.left * realVel;

            prevPos.Add(hunter.transform.position);
            companion.transform.position = prevPos[0] + Vector3.left * 5;
            if (prevPos.Count > 10) prevPos.RemoveAt(0);

            yield return new WaitForFixedUpdate();
        }

        //Setze Spa als nächste Deco:
        GameObject tmp = activeGrounds[1];

        GameObject mySpa = Instantiate(spa, tmp.transform.position + new Vector3(250.5f, 39.18f), Quaternion.identity, tmp.transform);

        activeGrounds[0].transform.position += Vector3.right * 400;
        for (int i = activeGrounds[0].transform.childCount; i > 0; i--) Destroy(activeGrounds[0].transform.GetChild(i - 1).gameObject);

        cScript.strength = 0.01f;
        cScript.target = gameObject;
        cScript.offset = Vector2.up * 2;

        //Renne bis zum Pool:
        transform.parent = mySpa.transform;
        while(transform.localPosition.x < -62)
        {
            prevPos.Add(hunter.transform.position);
            companion.transform.position = prevPos[0] + Vector3.left * 5;
            foreach (var ground in activeGrounds) ground.transform.position += Vector3.left * realVel;
            transform.localPosition += Vector3.right * realVel;

            yield return new WaitForFixedUpdate();
        }

        Animator anim_comp = companion.transform.GetChild(0).GetComponent<Animator>();
        hunter.anim.SetBool("moving", false);
        anim_comp.SetBool("moving", false);

        //Setze Huhn in den Hintergrund und lasse es in den Pool springen:
        transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        Transform chBody = transform.GetChild(0).GetChild(0).GetChild(0);
        chBody.GetChild(1).GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        chBody.GetChild(2).GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        chBody.GetChild(3).GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        chBody.GetChild(4).GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        chBody.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName = "Default";

        anim.SetTrigger("Jump");
        float x_pos = transform.position.x;
        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            x_pos += .3f;
            transform.position = new Vector3(x_pos,(count > .5f ? 4.6f + jumpCurve.Evaluate(count) * .4f : 1.6f + jumpCurve.Evaluate(count)));

            yield return new WaitForFixedUpdate();
        }
        anim.Play("sleep");
        yield return new WaitForSeconds(2);

        cScript.target = player;
        cScript.strength = .05f;
        cScript.offset = Vector2.up * 2;
        Destroy(hunter);

        yield return new WaitForSeconds(2);
        companion.transform.GetChild(0).GetComponent<Animator>().Play("JumpScare");
        speachBubble.Say("Hier sind die ganzen\nHühner also!", Bubbles.Normal, target: companion);
        speachBubble.Say("Die Eier haben wir schon,\nes fehlt uns nur\nnoch die Brühe", Bubbles.Normal, target: companion);
        yield return new WaitUntil(() => !speachBubble.finished);
        yield return new WaitUntil(() => speachBubble.finished);

        progress.huntDone = true;
        LoadSave.SaveProgress();

        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        player.transform.parent = mySpa.transform;
        companion.transform.parent = mySpa.transform;
        player.GetComponent<Rigidbody2D>().gravityScale = 2;
        StartCoroutine(mySpa.GetComponent<Seq_Spa>().Tipp());
        yield break;
    }

    IEnumerator RunHuntGame()
    {
        //Set camera:
        cScript.target = hunter.gameObject;

        cScript.aSrc.loop = true;
        cScript.aSrc.clip = raceMusic;
        cScript.aSrc.Play();
        anim.Play("PanikRun");
        hunter.anim.SetBool("moving", true);
        hunter.anim.SetTrigger("startMove");
        companion.transform.GetChild(0).GetComponent<Animator>().SetBool("moving", true);
        companion.transform.GetChild(0).GetComponent<Animator>().SetTrigger("startMove");
        prevPos.Clear();
        preyDist = startDist;
        eggCount = 0;

        StartCoroutine(SetGameInfo(true));

        runGame = true;
        while (runGame && eggCount < eggGoal)
        {
            if (pauseGame)
            {
                //Pause game:
                player.GetComponent<Rigidbody2D>().simulated = false;
                yield return new WaitForSeconds(.1f);
                player.transform.GetChild(0).GetComponent<Animator>().SetBool("inAir", false);
                CanvasGroup infoGroup = canvas.transform.GetChild(2).GetComponent<CanvasGroup>();
                for(float count = 1; count > 0; count -= Time.fixedDeltaTime)
                {
                    infoGroup.alpha = count;
                    yield return new WaitForFixedUpdate();
                }
                infoGroup.gameObject.SetActive(false);
                yield return new WaitWhile(()=>pauseGame);
                infoGroup.gameObject.SetActive(true);
                for (float count = 0; count < 1; count += Time.fixedDeltaTime)
                {
                    infoGroup.alpha = count;
                    yield return new WaitForFixedUpdate();
                }
                player.GetComponent<Rigidbody2D>().simulated = true;
            }

            //Steuerung des Huhns:
            RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.down, Vector2.right, 10, mask);
            if(hit.collider != null)
                StartCoroutine(Jump());

            if (eggTimer > 0) eggTimer -= Time.fixedDeltaTime;
            else if(preyDist < eggDistance && Random.Range(0,100) == 0)
            {
                StartCoroutine(LayEgg());
                aSrc.clip = chickenClips[1];
                aSrc.Play();
            }
            //*/
            //Setze Boden:
            SetGround();

            //Update der Positionen:
            if (preyDist > maxView * Camera.main.aspect) { Debug.Log("game over"); runGame = false; break; }
            hunter.transform.position = new Vector3(transform.position.x -preyDist, hunter.rb.position.y);
            cScript.offset = new Vector3(preyDist/2, preyDist/8);
            Camera.main.orthographicSize = (preyDist + 20) / (Camera.main.aspect * 2);

            prevPos.Add(hunter.transform.position);
            companion.transform.position = prevPos[0] + Vector3.left * 5;
            if (prevPos.Count > 10) prevPos.RemoveAt(0);

            yield return new WaitForFixedUpdate();
        }
        StartCoroutine(cScript.SetBGM(manager.normalMusic, 4));
        StartCoroutine(SetGameInfo(false));

        if (eggCount == eggGoal) StartCoroutine(WinSequence());
        else StartCoroutine(LoseSequence());
        yield break;
    }

    private void SetGround()
    {
        foreach (var ground in activeGrounds) ground.transform.position += Vector3.left * realVel;
        if(activeGrounds[0].transform.position.x < -maxView * Camera.main.aspect - 75)
        {
            GameObject tmp = activeGrounds[0];
            activeGrounds.RemoveAt(0);

            for (int i = tmp.transform.childCount; i > 0; i--) Destroy(tmp.transform.GetChild(i-1).gameObject);
            tmp.transform.position = activeGrounds[1].transform.position + Vector3.right * 100;

            //Add bckgrnd:
            float pos = -50;
            int obj;
            do
            {
                obj = Random.Range(0, decoration.Length);

                pos += (obj < 4 ? 2 : 10) + Random.Range(0f, 10f);
                if (pos > 50) pos = 50;
                Instantiate(decoration[obj], new Vector3(pos, 24.85f) + tmp.transform.position, Quaternion.identity, tmp.transform);
            } while (pos < 30);

            //Add obstacles:
            pos = -50;
            do
            {
                pos += 20 + Random.Range(0f, 20f);
                if (pos > 50) pos = 50;
                obj = Random.Range(0, obstacles.Length);
                Instantiate(obstacles[obj], new Vector3(pos, obj < 2? 30 : 25) + tmp.transform.position, Quaternion.identity, tmp.transform);
            } while (pos < 30);

            activeGrounds.Add(tmp);
        }
    }

    IEnumerator LayEgg()
    {
        eggTimer = eggTime;

        StartCoroutine(Jump());
        yield return new WaitForSeconds(0.1f);
        GameObject obj = Instantiate(egg, transform.position, Quaternion.identity);
        obj.transform.parent = activeGrounds[2].transform;
        yield break;
    }

    IEnumerator Jump()
    {
        if (isJumping) yield break;
        isJumping = true;
        anim.SetTrigger("Jump");
        float ptr = 0;
        for(int i = 0; i < 50; i++)
        {
            ptr += 0.02f;
            transform.position = new Vector3(transform.position.x, 1.6f + jumpCurve.Evaluate(ptr));
            yield return new WaitForFixedUpdate();
        }
        anim.SetTrigger("Run");
        isJumping = false;
        yield break;
    }

    IEnumerator SetGameInfo(bool active)
    {
        CanvasGroup gameInfo = canvas.transform.GetChild(2).GetComponent<CanvasGroup>();
        gameInfo.gameObject.SetActive(true);
        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            gameInfo.alpha = active ? count : 1 - count;
            yield return new WaitForFixedUpdate();
        }
        gameInfo.gameObject.SetActive(active);
        yield break;
    }
}
