using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;
using static CameraScript;
using static MenuScript;
using static BubbleScript;

public class BoxScript : MonoBehaviour
{
    public int shakeGoal = 10;

    public float shakeThresh = 1.3f;
    public int maxPulsCount = 1;
    public float velStrength;

    public GameObject[] waste;
    public GameObject jetpack;

    public Material transition;

    // Start is called before the first frame update
    void Start()
    {
        Input.gyro.enabled = true;
        StartCoroutine(BoxGame());
    }

    IEnumerator BoxGame()
    {
        //*
        //Warte darauf in die richtige position gedreht zu werden
        float rot;
        do
        {
            rot = Mathf.Atan2(Input.gyro.gravity.y, Input.gyro.gravity.x);
            yield return new WaitForFixedUpdate();
        } while (Mathf.Abs(rot) < 1.3f || Input.gyro.gravity.x > -.4f);

        //Hebe Kiste hoch:
        Vector3 start = transform.position;
        AnimationCurve curve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        cScript.offset = Vector3.down * 4;
        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            transform.position = start * curve.Evaluate(count);
            yield return new WaitForFixedUpdate();
        }

        //Starte Spiel:
        transform.GetChild(1).GetComponent<Rigidbody2D>().simulated = true;
        transform.GetChild(2).GetComponent<Rigidbody2D>().simulated = true;
        runGame = true;
        int shakeCount = 0;
        GameObject content;
        Rigidbody2D rb_content;

        float timer = 10;
        while (runGame)
        {
            //Rotiere die Kiste entsprechend der Ausrichtung des Geräts:
            transform.rotation = Quaternion.Euler(0,0,Mathf.Atan2(-Input.gyro.gravity.y, -Input.gyro.gravity.x) * Mathf.Rad2Deg);

            //Checke, ob geschüttelt wurde:
            if(Input.gyro.userAcceleration.x < -shakeThresh && Mathf.Abs(transform.eulerAngles.z) > 150)
            {
                timer = 10;
                if (++shakeCount > shakeGoal) { runGame = false; break; }
                int pulsCount = Random.Range(1, maxPulsCount);
                for(int i = 0; i < pulsCount; i++)
                {
                    content = Instantiate(waste[Random.Range(0, waste.Length)], transform.position + new Vector3(), Quaternion.Euler(0,0,Random.Range(-180, 180)));
                    rb_content = content.GetComponent<Rigidbody2D>();
                    rb_content.angularVelocity = Random.Range(-5, 5);
                    rb_content.velocity = Random.insideUnitCircle * velStrength;
                }
                yield return new WaitUntil(()=>Input.gyro.userAcceleration.x > -1);
            }

            timer -= Time.fixedDeltaTime;
            if(timer < 0)
            {
                //Hinweise:
                if(Mathf.Abs(transform.eulerAngles.z) < 150)
                    speachBubble.Say("Hey,\nDu hältst die\nKiste falsch rum!", Bubbles.Normal, speed: 2f, autoPlay: true);
                else
                    speachBubble.Say("Hm, vielleicht musst\ndu die Kiste\netwas schütteln", Bubbles.Normal, speed: 2f, autoPlay: true);

                timer = 10;
            }
            yield return new WaitForFixedUpdate();
        }

        content = Instantiate(jetpack, transform.position, Quaternion.identity);
        start = content.transform.position;
        for(float count = 1; count > 0; count -= Time.fixedDeltaTime)
        {
            content.transform.position = start + Vector3.down * 5 * curve.Evaluate(count);
            yield return new WaitForFixedUpdate();
        }
        cScript.target = content;

        //Hebe die Kiste weg:
        start = transform.position;
        for (float count = 1; count > 0; count -= Time.fixedDeltaTime)
        {
            transform.position = start + Vector3.up * 20 * curve.Evaluate(count);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitUntil(()=>Input.touchCount != 0);
        yield return new WaitUntil(()=>Input.touchCount == 0);
        //*/
        //Lade nächste Scene:
        menu.SetTransition(transition);
        menu.DoTransition(-1, 1, false);
        yield return new WaitForSeconds(2.4f);
        Mat_Intro = transition;
        SceneManager.LoadScene("MillDestruction");

        yield break;
    }
}
