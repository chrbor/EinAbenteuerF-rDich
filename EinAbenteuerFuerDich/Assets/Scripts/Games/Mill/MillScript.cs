using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static CameraScript;
using UnityEngine.UI;
using UnityEngine.Events;

public class MillScript : MonoBehaviour
{
    public Text text;

    public AudioClip gameMusic;
    public UnityAction ButtonPressed, ButtonReleased;

    [Header("Construct-Game:")]
    public GameObject touchpoint;
    private List<GameObject> activeTouchPoints = new List<GameObject>();
    public Vector2[] touchPointPos;
    public float waitTime = 2;

    private Vector2[] partPos;
    private Transform partHolder;

    [Header("BlowingGame:")]
    public float goal;
    public float gain;
    public float loss;
    public float thresh_sound;
    private float loss_real;
    private float goal_real;


    private bool buttonHold, buttonReleased, game_running;
    private bool phase2;

    private void Start()
    {
        partHolder = transform.GetChild(1);
        partPos = new Vector2[partHolder.childCount];
        for (int i = 0; i < partPos.Length; i++) partPos[i] = partHolder.GetChild(i).localPosition;
        partHolder.gameObject.SetActive(false);

        ButtonPressed += OnButtonPressed;
        ButtonReleased += OnButtonReleased;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.GetComponent<TouchSensor>().tipped || runGame || other.gameObject.layer != 14/*Touch*/) return;
        runGame = true;
        staticCam = true;
        pauseMove = true;
        cScript.target = gameObject;
        cScript.offset = Vector2.up * 8;
        cScript.transform.rotation = Quaternion.identity;
        if (phase2) StartCoroutine(BlowMill());
        else
        {
            StartCoroutine(GetDestroyed());
            StartCoroutine(MillGame());
        }
    }

    IEnumerator GetDestroyed()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        partHolder.gameObject.SetActive(true);
        Rigidbody2D rb_part;
        for(int i = 1; i < partHolder.childCount; i++) {
            rb_part = partHolder.GetChild(i).GetComponent<Rigidbody2D>();
            rb_part.GetComponent<Collider2D>().enabled = true;
            rb_part.bodyType = RigidbodyType2D.Dynamic;
            rb_part.velocity = new Vector2(rb_part.transform.localPosition.y, rb_part.transform.localPosition.x);
            rb_part.angularVelocity = Random.Range(-10f, 10f);
        }
        yield break;
    }

    IEnumerator BuildPart(int partNumber)
    {
        Transform part = transform.GetChild(1).GetChild(partNumber);
        part.GetComponent<Collider2D>().enabled = false;
        part.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        Vector2 diff_pos = part.localPosition;
        float diff_rot = part.localRotation.eulerAngles.z;

        for(float count = 1; count > 0; count -= Time.fixedDeltaTime)
        {
            part.localPosition = partPos[partNumber]  + count * diff_pos;
            part.localRotation = Quaternion.Euler(0,0,diff_rot * count);
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }

    IEnumerator MillGame()
    {
        //Rotiere die Camera um 90°:
        for (float count = 0; count < 1; count += Time.fixedDeltaTime) { cScript.transform.rotation = Quaternion.Euler(0, 0, count * 90); Camera.main.orthographicSize = 8 - count;  yield return new WaitForFixedUpdate(); }

        text.text = "Wait for release";

        activeTouchPoints.Clear();
        yield return new WaitUntil(() => Input.touchCount == 0);
        text.text = "starting game";
        cScript.aSrc.Stop();
        cScript.aSrc.loop = true;
        cScript.aSrc.clip = gameMusic;
        cScript.aSrc.Play();
        yield return new WaitForSeconds(2f);

        float timeCount;
        buttonReleased = false;
        float touchStepNumber = touchPointPos.Length + 2 * (difficulty - 1);
        int oldBuildPart = 0, newBuildPart;
        for(int i = 0; i < touchStepNumber && !buttonReleased ; i++)
        {
            //Bauanimation:
            newBuildPart = Mathf.CeilToInt((i + 1) * (10f / touchStepNumber));
            for(int j = oldBuildPart; j < newBuildPart; j++) StartCoroutine(BuildPart(j));
            oldBuildPart = newBuildPart;

            //Update die touchpoints:
            activeTouchPoints.Insert(0, Instantiate(touchpoint, touchPointPos[i] + (Vector2)transform.position, Quaternion.identity));
            activeTouchPoints[0].GetComponent<InGameButton>().SetCallback(ButtonPressed, ButtonReleased);
            buttonHold = false;
            text.text = "touchpoint " + i + " created";
            yield return new WaitUntil(() => buttonHold || buttonReleased);
            timeCount = 0;
            while(timeCount < waitTime && !buttonReleased) { yield return new WaitForEndOfFrame(); timeCount += Time.deltaTime; }
        }

        foreach (var obj in activeTouchPoints) obj.GetComponent<InGameButton>().active = false;
        manager.PlayNormal();

        //Wenn einer der Buttons released wurde, dann hat der Spieler das Spiel verloren:
        if (buttonReleased)
            text.text = "you lose!";
        else
        {
            text.text = "you win!";
            GetComponent<SpriteRenderer>().enabled = true;
            transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
            transform.GetChild(1).gameObject.SetActive(false);
            phase2 = true;
        }

        //Clear Game:
        yield return new WaitUntil(() => Input.touchCount == 0);
        yield return new WaitUntil(() => Input.touchCount > 0);
        //Rotiere die Camera um 90°:
        for (float count = 1; count > 0; count -= Time.fixedDeltaTime) { cScript.transform.rotation = Quaternion.Euler(0, 0, count * 90); Camera.main.orthographicSize = 8 - count; yield return new WaitForFixedUpdate(); }

        yield return new WaitUntil(() => Input.touchCount == 0);
        if (buttonReleased) StartCoroutine(GetDestroyed());
        foreach (GameObject obj in activeTouchPoints) Destroy(obj);
        activeTouchPoints.Clear();
        yield return new WaitForEndOfFrame();
        runGame = false;
        staticCam = false;
        pauseMove = false;
        cScript.target = player;
        yield break;
    }

    IEnumerator BlowMill()
    {

        loss_real = loss * Time.fixedDeltaTime;
        goal_real = goal * Time.fixedDeltaTime;
        StartCoroutine(VoiceScript.ReportFrequencies());

        game_running = true;
        float rotation = 0;
        bool win = false;
        Transform wings = transform.GetChild(0);
        while (game_running)
        {

            text.text = "pow: " + VoiceScript.Signal.power;
            rotation += VoiceScript.Signal.power > thresh_sound ? gain * VoiceScript.Signal.power : -loss_real;
            if (rotation < 0) rotation = 0;
            wings.Rotate(Vector3.forward, rotation);
            if (win = rotation > goal_real) game_running = false;
            yield return new WaitForFixedUpdate();
        }
        VoiceScript.runFetch = false;

        //Aktion, die bei Sieg/ Niederlage ausgeführt wird:
        if (win)
        {
            text.text = "you win!";
        }
        else
        {
            text.text = "you lose!";
        }

        //Clear Game:
        yield return new WaitUntil(() => Input.touchCount == 0);
        yield return new WaitUntil(() => Input.touchCount > 0);
        runGame = false;
        staticCam = false;
        pauseMove = false;
        cScript.target = player;
        yield break;
    }

    public void OnButtonPressed() => buttonHold = true;
    public void OnButtonReleased() => buttonReleased = true;

    public void SetButtonHold() => buttonHold = true;

    private void OnDrawGizmosSelected()
    {
        foreach(Vector2 pos in touchPointPos)
            Gizmos.DrawWireSphere(transform.position + (Vector3)pos, 1.5f);
    }
}
