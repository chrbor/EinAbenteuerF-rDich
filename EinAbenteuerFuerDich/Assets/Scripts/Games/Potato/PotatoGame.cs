using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static CameraScript;
using UnityEngine.UI;

public class PotatoGame : MonoBehaviour
{
    public Text text;

    public AudioClip gameMusic;
    public AccessoirSet[] accessoirSets;
    public Vector2[] potatoPos;
    public GameObject potatoPrefab;

    [Header("MoveVar:")]
    public float thresh_maxAngle;
    public float thresh_run;
    public float velocity;
    private float realVel;

    private List<GameObject> potatos = new List<GameObject>();
    [HideInInspector]
    public int potatoCount;
    public int potatoGoal;
    [HideInInspector]
    public bool digging;
    private bool isMoving;

    public static PotatoGame potatoGame;
    private bool running_game;
    public Vibration vib;
    [HideInInspector]
    public float minDist;
    int vibID, vibID_current;

    void Start()
    {
        potatoGame = this;
        realVel = velocity * Time.fixedDeltaTime;

        //Setup der Vibration:
        vib = new Vibration();
        vib.SetVibrationEffect(new long[2] { 1, 10 }, 0);
        vib.SetVibrationEffect(new long[2] { 1, 5 }, 0);
        vib.SetVibrationEffect(new long[2] { 1, 2 }, 0);
        vib.SetVibrationEffect(new long[2] { 1, 1 }, 0);
        vib.SetVibrationEffect(new long[2] { 5, 1 }, 0);
        vib.SetVibrationEffect(new long[2] { 10, 1 }, 0);
    }

    IEnumerator UpdateVibrator()
    {
        while (running_game)
        {
            if (minDist == 99) { vib.Cancel(); vibID_current = 44; }
            else
            {
                vibID = minDist >= 10 ? 5 : (int)(minDist / 2);

                if (vibID != vibID_current) { vib.Cancel(); vib.Vibrate(vibID); }
                vibID_current = vibID;
                minDist = 99;
            }

            yield return new WaitForSeconds(.1f);
        }
        yield break;
    }

    IEnumerator RunIntroSequence()
    {
        text.text = "spiele Sequenz ab";

        //Verkleide den Spieler und dessen Companion zu Detektive:
        Transform parent_playerAcc = player.transform.GetChild(0).GetChild(0).GetChild(0);
        //Transform parent_companionAcc = companion.transform.GetChild(0).GetChild(0).GetChild(0);

        StartCoroutine(clothObj.DestroyCloths(parent_playerAcc));
        yield return new WaitForSeconds(.2f);
        //StartCoroutine(clothObj.DestroyCloths(parent_companionAcc));
        yield return new WaitForSeconds(.9f);
        //Füge Dektektivkleidung hinzu:
        foreach (var cloth in accessoirSets[0].accessoirs) { StartCoroutine(cloth.CreateCloth(parent_playerAcc)); yield return new WaitForFixedUpdate(); }
        yield return new WaitForSeconds(.2f);
        //foreach (var cloth in companionAccessoir.accessoirs) StartCoroutine(cloth.CreateCloth(parent_companionAcc));
        yield return new WaitForSeconds(.9f);

        StartCoroutine(RunPotatoGame());
        yield break;
    }

    IEnumerator RunPotatoGame()
    {
        //Rotiere die Camera um 90°:
        for (float count = 0; count < 1; count += Time.fixedDeltaTime) { cScript.transform.rotation = Quaternion.Euler(0, 0, count * 90); yield return new WaitForFixedUpdate(); }
        
        yield return new WaitUntil(() => Input.touchCount == 0);
        while(Input.touchCount == 0)
        {
            text.text = Input.gyro.gravity.ToString();
            yield return new WaitForEndOfFrame();
        }
        //yield return new WaitUntil(() => Input.touchCount != 0);

        //Verteile die Kartoffeln:
        foreach(var pos in potatoPos)
            potatos.Add(Instantiate(potatoPrefab, (Vector2)transform.position + pos, Quaternion.identity));

        transform.GetChild(0).gameObject.SetActive(true);

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        Animator anim = player.transform.GetChild(0).GetComponent<Animator>();
        anim.Rebind();
        cScript.target = player;
        Camera.main.orthographicSize = 8;
        Input.gyro.enabled = true;

        cScript.aSrc.clip = gameMusic;
        cScript.aSrc.loop = true;
        cScript.aSrc.Play();

        runGame = true;
        running_game = true;
        potatoCount = 0;
        StartCoroutine(UpdateVibrator());
        while (runGame && potatoCount < potatoGoal)
        {
            text.text = "kartoffeln: " + potatoCount + " von " + potatoGoal;

            //Laufen:
            if (Input.gyro.gravity.x > -0.2f)
            {
                Camera.main.transform.rotation = Quaternion.Euler(0, 0, 90);
                anim.SetBool("moving", false);

                //Grab-Geste:
                yield return new WaitUntil(() => !runGame || Input.gyro.gravity.x < -.2f);

                digging = true;
                anim.Play("dig");
                yield return new WaitForSeconds(1f);
                anim.SetTrigger("startMove");
                digging = false;
            }
            else
            {
                float angle = Mathf.Atan(Input.gyro.gravity.y / Input.gyro.gravity.x) * Mathf.Rad2Deg;
                if (Mathf.Abs(angle) > thresh_maxAngle) angle = Mathf.Sign(angle) * thresh_maxAngle;

                float moveStrength = angle / 90;

                cScript.offset = new Vector2(moveStrength * 10, 1);
                Camera.main.transform.eulerAngles = new Vector3(0, 0, 90 - angle);

                if (Mathf.Abs(angle) > thresh_run)
                {
                    rb.position += Vector2.right * realVel * moveStrength;
                    anim.SetBool("moving", true);
                }
                else anim.SetBool("moving", false);
            }

            yield return new WaitForFixedUpdate();
        }

        if(potatoCount >= potatoGoal)
        {
            text.text = "You win";
        }
        else
        {
            text.text = "You lose";
        }
        cScript.aSrc.Stop();

        //Clear Game:
        vib.Cancel();
        yield return new WaitUntil(() => Input.touchCount == 0);
        yield return new WaitUntil(() => Input.touchCount > 0);
        //Rotiere die Camera um 90°:
        for (float count = 1; count > 0; count -= Time.fixedDeltaTime) { cScript.transform.rotation = Quaternion.Euler(0, 0, count * 90); yield return new WaitForFixedUpdate(); }
        
        //Kleide den Spieler und dessen Companion zurück:
        Transform parent_playerAcc = player.transform.GetChild(0).GetChild(0).GetChild(0);
        //Transform parent_companionAcc = companion.transform.GetChild(0).GetChild(0).GetChild(0);

        StartCoroutine(clothObj.DestroyCloths(parent_playerAcc));
        yield return new WaitForSeconds(.2f);
        //StartCoroutine(clothObj.DestroyCloths(parent_companionAcc));
        yield return new WaitForSeconds(.9f);
        //Füge Dektektivkleidung hinzu:
        foreach (var cloth in playerAccessoir.accessoirs) { StartCoroutine(cloth.CreateCloth(parent_playerAcc)); yield return new WaitForFixedUpdate(); }
        yield return new WaitForSeconds(.2f);
        //foreach (var cloth in companionAccessoir.accessoirs) StartCoroutine(cloth.CreateCloth(parent_companionAcc));
        yield return new WaitForSeconds(.9f);

        yield return new WaitForEndOfFrame();
        transform.GetChild(0).gameObject.SetActive(false);
        runGame = false;
        running_game = false;
        staticCam = false;
        pauseMove = false;
        cScript.target = player;
        yield break;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.GetComponent<TouchSensor>().tipped || runGame || other.gameObject.layer != 14/*Touch*/) return;
        staticCam = true;
        pauseMove = true;
        cScript.target = gameObject;
        cScript.offset = Vector2.zero;
        cScript.transform.rotation = Quaternion.identity;
        StartCoroutine(RunIntroSequence());
    }

    private void OnDrawGizmosSelected()
    {
        foreach (Vector2 pos in potatoPos)
            Gizmos.DrawWireSphere(transform.position + (Vector3)pos, 10);
    }
}
