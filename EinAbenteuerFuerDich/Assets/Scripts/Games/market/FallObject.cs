using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MarketGame;
using static CameraScript;

public class FallObject : MonoBehaviour
{
    public AnimationCurve FallTraj;
    public float maxRotation;

    float rotation;
    private bool followCurve, inCart;
    private AudioSource aSrc;
    public AudioClip inCartClip, splashClip;

    Vibration vib;

    private void Start()
    {
        aSrc = GetComponent<AudioSource>();

        //Setup der Vibration:
        vib = new Vibration();
        vib.SetVibrationEffect(new long[54] { 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 5, 1, 5, 1, 5, 1, 5, 1, 5, 1, 5, 1, 10, 1, 10, 1, 10, 1, 5, 1, 5, 1, 5, 1, 5, 1, 5, 1, 5, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2 });
    }

    /// <summary>
    /// Lässt das Object entsprechend der Animationskurve fallen
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="falltime">Die Zeit in sek, die das Objekt braucht, bis es den Boden erreicht</param>
    public IEnumerator StartFall(Vector2 targetPosition, float falltime)
    {
        rotation = Random.Range(-maxRotation, maxRotation);

        //Bereite FallTraj vor:
        int steps = (int)(falltime / Time.fixedDeltaTime);
        float diff = FallTraj.Evaluate(1) - FallTraj.Evaluate(0);
        if (diff >= 0) { Destroy(gameObject); yield break; }
        float x_factor = targetPosition.x - transform.position.x;
        float y_factor = (targetPosition.y - transform.position.y) / diff;
        Vector3 start = transform.position;

        //Laufe den Graphen ab:
        followCurve = true;
        inCart = false;
        float process;
        for (float i = 0; i < falltime && followCurve; i += Time.fixedDeltaTime)
        {
            process = i / falltime;
            transform.position = start + new Vector3(x_factor * process, y_factor * FallTraj.Evaluate(process));
            transform.localScale = Vector3.one * (0.2f - 0.3f * FallTraj.Evaluate(process));
            yield return new WaitForFixedUpdate();
        }
        if (!followCurve) yield break;
        //FeedBack:
        lives--;
        aSrc.panStereo = transform.position.x / maxRange;
        aSrc.clip = splashClip;
        vib.Vibrate(0);
        aSrc.Play();
        StartCoroutine(cScript.Shake());
        inCart = true;
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitWhile(()=>aSrc.isPlaying);
        vib.DestroyVibration();
        yield return new WaitUntil(() => reset);
        Destroy(gameObject);

        yield break;
    }

    private void Update()
    {
        if (!inCart) transform.Rotate(Vector3.forward, rotation);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        StartCoroutine(FallIntoCart(other.transform));
    }

    IEnumerator FallIntoCart(Transform cart)
    {
        pointCount++;

        //Feedback:
        aSrc.panStereo = cart.position.x / maxRange;
        aSrc.clip = inCartClip;
        aSrc.Play();

        followCurve = false;
        transform.parent = cart.GetChild(0).transform;
        Vector3 step = (transform.localPosition + (Vector3)Random.insideUnitCircle)/25;
        for (int i = 0; i < 25; i++) { transform.localPosition -= step; yield return new WaitForFixedUpdate(); }
        inCart = true;
        //yield return new WaitWhile(()=>aSrc.isPlaying);
        yield return new WaitUntil(() => reset);
        vib.DestroyVibration();
        Destroy(gameObject);
        yield break;
    }
}
