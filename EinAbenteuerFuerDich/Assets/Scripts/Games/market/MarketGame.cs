using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using UnityEngine.UI;
using static CameraScript;


/// <summary>
/// Spielablauf:
/// Spiel startet, indem der Einkaufswagen angetippt wird.
/// 
/// Ziel ist es 15 Sachen zu fangen.
/// Es dürfen nicht mehr als drei Sachen auf den Boden kommen.
/// Der Einkaufswagen kann bewegt werden, indem man das Smartphone nach links/rechts schiebt
/// </summary>
public class MarketGame : MonoBehaviour
{
    public Text text;

    public static int lives = 3;
    public static int pointCount = 0;
    public int goalCount = 15;
    public AudioClip gameMusic;
    public AudioClip winClip;
    public AudioClip loseClip;
    private AudioSource aSrc;


    public static float maxRange;

    [Header("Wichtige Scripte zur Kommunikation:")]
    public CartScript cart;
    public ThrowerScript thrower;
    public ParticleSystem clouds;
    private ParticleSystem.EmissionModule cloudsEmission;

    public static bool cartTouched;

    // Start is called before the first frame update
    void Start()
    {
        //Lege die maximale Reichweite fest, in der die Waren fallen:
        maxRange = Camera.main.aspect * Camera.main.orthographicSize - 2.5f;
        cloudsEmission = clouds.emission;

        thrower.SetParams(1.75f, new Vector2(3, 5), cart.transform.position.y);
        aSrc = GetComponent<AudioSource>();
        aSrc.loop = true;
        StartCoroutine(StartPreScene());
    }

    IEnumerator StartPreScene()
    {
        //Hier Scene vor dem ersten Spiel animieren:

        //Starte Hauptspiel:
        StartCoroutine(MainGame());
        yield break;
    }

    IEnumerator MainGame()
    {
        if (!aSrc.isPlaying) { aSrc.volume = 0.25f; aSrc.Play(); }

        cloudsEmission.enabled = false;
        yield return new WaitUntil(() => Input.touchCount == 0);
        //Warte, bis der Wagen berührt wird:
        text.text = "warte auf touch des EKs";
        cartTouched = false;
        yield return new WaitUntil(() => !pauseGame && cartTouched);

        text.text = "run game";

        //run game:
        bool gameWon = false;
        runGame = true;
        StartCoroutine(ListenForPosReset());
        StartCoroutine(thrower.ThrowObjects());
        aSrc.Stop();
        cScript.aSrc.loop = true;
        cScript.aSrc.clip = gameMusic;
        cScript.aSrc.Play();
        while (runGame)
        {
            cloudsEmission.enabled = true;
            cart.SetActive(true);
            while(!pauseGame && runGame)
            {
                yield return new WaitForFixedUpdate();
                if (lives <= 0) runGame = false;
                if(pointCount >= goalCount) { runGame = false; gameWon = true; }
            }
            cart.SetActive(false);
            cloudsEmission.enabled = false;
        }

        aSrc.volume = 0;
        aSrc.Play();
        for(float volume = 1; volume > 0; volume -= 0.02f) { cScript.aSrc.volume = volume; aSrc.volume = (1 - volume)*.25f; yield return new WaitForFixedUpdate(); }
        cScript.aSrc.Stop();
        cScript.aSrc.loop = false;
        cScript.aSrc.volume = 1;
        if (gameWon)
        {
            cScript.aSrc.clip = winClip;
            cScript.aSrc.Play();
            text.text = "Yay, du hast gewonnen!";
        }
        else
        {
            //Hier Sequenz einfügen, die dem Spieler sagt, dass man verloren hat
            cScript.aSrc.clip = loseClip;
            cScript.aSrc.Play();
            text.text = "you lose";
            yield return new WaitWhile(() => Input.touchCount > 0);
            yield return new WaitWhile(() => Input.touchCount == 0);
            cart.ResetPosition();
            for(int i = 0; i < cart.transform.GetChild(0).childCount; i++) Destroy(cart.transform.GetChild(0).GetChild(i).gameObject);
            lives = 3;
            pointCount = 0;
            StartCoroutine(MainGame());
        }

        yield break;
    }

    IEnumerator ListenForPosReset()
    {
        float clickIntervall = 0.3f;
        while (runGame)
        {
            if(Input.touchCount > 0)
            {
                float count = 0;
                for (; Input.touchCount != 0 && count < clickIntervall; count += Time.deltaTime) yield return new WaitForEndOfFrame();
                for (; Input.touchCount == 0 && count < clickIntervall; count += Time.deltaTime) yield return new WaitForEndOfFrame();
                if (count < clickIntervall) StartCoroutine(cart.ResetPosition());
            }
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1);
        StartCoroutine(cart.ResetPosition());
        yield break;
    }
}
