using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static CameraScript;
using UnityEngine.UI;
using static MenuScript;


public class BusScript : MonoBehaviour
{
    public int direction;
    public GameObject busPrefab;
    public AnimationCurve busPos;
    public GameObject endStation;
    public Material mat;

    private GameObject bus;
    private int ready;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.GetComponent<TouchSensor>().tipped || runGame || other.gameObject.layer != 14/*Touch*/) return;
        runGame = true;
        StartCoroutine(DriveWithBus());
    }

    IEnumerator DriveWithBus()
    {
        yield return new WaitForEndOfFrame();

        cScript.target = gameObject;
        cScript.offset = Vector2.zero;
        StartCoroutine(cScript.SetRotation());
        staticCam = true;
        pauseMove = true;

        companion.transform.GetChild(0).GetComponent<Animator>().SetBool("moving", false);

        //hole Bus:
        bus = Instantiate(busPrefab, transform.position + new Vector3(direction * -50, -4), Quaternion.identity);
        bus.GetComponent<AudioSource>().Play();
        ParticleSystem.MainModule pSystem = bus.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>().main;
        pSystem.startSpeed = new ParticleSystem.MinMaxCurve(-.5f * direction, -direction);
        bus.transform.localScale = new Vector3(-direction, 1);
        Vector3 start = transform.position - Vector3.up * 4;
        for(float count = 0; count < 3; count += Time.fixedDeltaTime)
        {
            bus.transform.position = start - busPos.Evaluate(count) * Vector3.right * direction;
            yield return new WaitForFixedUpdate();
        }

        companion.GetComponent<FollowScript>().enabled = false;
        player.GetComponent<PlayerScript>().enabled = false;
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        yield return new WaitForSeconds(.1f);
        player.transform.GetChild(0).GetComponent<Animator>().SetBool("inAir", false);
        yield return new WaitForSeconds(.25f);

        //Steige ein:
        ready = 2;
        StartCoroutine(EnterBus(new Vector2(4f, 1.75f), player));
        yield return new WaitForSeconds(.5f);
        StartCoroutine(EnterBus(new Vector2(7f, 1.75f), companion));
        yield return new WaitUntil(() => ready <= 0);
        yield return new WaitForSeconds(1f);

        //Fahre weg:
        start = bus.transform.position;
        for (float count = 3; count > 0; count -= Time.fixedDeltaTime)
        {
            bus.transform.position = start + busPos.Evaluate(count) * Vector3.right * direction;
            yield return new WaitForFixedUpdate();
        }

        //Blende zur anderen Station:
        menu.SetTransition(mat);
        menu.DoTransition(direction, 1, false);
        yield return new WaitForSeconds(2.4f);
        cScript.target = endStation;
        cScript.transform.position = endStation.transform.position + Vector3.back * 10;
        menu.DoTransition(direction, -1, true);
        yield return new WaitForSeconds(2f);

        //Komme an:
        start = new Vector3(endStation.transform.position.x, transform.position.y - 4);
        for (float count = 0; count < 3; count += Time.fixedDeltaTime)
        {
            bus.transform.position = start - busPos.Evaluate(count) * Vector3.right * direction;
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(.25f);

        //Steige aus:
        ready = 2;
        StartCoroutine(ExitBus(-6, player));
        yield return new WaitForSeconds(.5f);
        StartCoroutine(ExitBus(-4, companion));
        yield return new WaitUntil(() => ready <= 0);
        yield return new WaitForSeconds(1f);

        //Fahre weg:
        start = bus.transform.position;
        for (float count = 3; count > 0; count -= Time.fixedDeltaTime)
        {
            bus.transform.position = start + busPos.Evaluate(count) * Vector3.right * direction;
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(.25f);



        //Räume Szene auf:
        Destroy(bus);
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        player.GetComponent<PlayerScript>().enabled = true;
        companion.GetComponent<FollowScript>().enabled = true;
        cScript.target = player;
        staticCam = false;
        pauseMove = false;
        runGame = false;
        yield break;
    }

    IEnumerator EnterBus(Vector2 sitPos, GameObject character)
    {
        Animator anim = character.transform.GetChild(0).GetComponent<Animator>();
        anim.SetBool("moving", true);
        anim.SetTrigger("startMove");

        Vector3 goalPos = bus.transform.position - Vector3.right * 4.8f * bus.transform.localScale.x;
        Vector3 step = Vector3.right * 0.1f * Mathf.Sign(goalPos.x - character.transform.position.x);

        while (Mathf.Abs(character.transform.position.x - goalPos.x) > .1f)
        {
            character.transform.position += step;
            yield return new WaitForFixedUpdate();
        }

        character.transform.parent = bus.transform;
        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            character.transform.localPosition = new Vector3(-4.5f, sitPos.y * count);
            yield return new WaitForFixedUpdate();
        }
        for (float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            character.transform.localPosition = new Vector3(-4.5f * (1 - count) + sitPos.x * count, sitPos.y);
            yield return new WaitForFixedUpdate();
        }

        anim.SetBool("moving", false);
        ready--;
        yield break;
    }

    IEnumerator ExitBus(float endPos_x, GameObject character)
    {
        Animator anim = character.transform.GetChild(0).GetComponent<Animator>();
        anim.SetBool("moving", true);
        anim.SetTrigger("startmove");

        float startPos_x = character.transform.localPosition.x;
        for (float counter = 0; counter < 1; counter += Time.fixedDeltaTime)
        {
            character.transform.localPosition = new Vector3(-4.5f * counter + startPos_x * (1 - counter), character.transform.localPosition.y);
            yield return new WaitForFixedUpdate();
        }
        for (float count = 1; count > 0; count -= Time.fixedDeltaTime)
        {
            character.transform.localPosition = new Vector3(-4.5f, character.transform.localPosition.y * count);
            yield return new WaitForFixedUpdate();
        }
        character.transform.parent = null;

        Vector3 step = Vector3.right * endPos_x * direction * Time.fixedDeltaTime / 2;

        for (float counter = 0; counter < 2; counter += Time.fixedDeltaTime)
        {
            character.transform.localPosition += step;
            yield return new WaitForFixedUpdate();
        }

        character.transform.parent = null;
        anim.SetBool("moving", false);
        ready--;
        yield break;
    }
}
