using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggScript : MonoBehaviour
{
    public AnimationCurve groundBounce;
    public AnimationCurve airCurve;
    [Min(.1f)]
    public float change;
    public float maxRotation;
    public float velocity;
    private float realVel;
    private float realChange;
    private float rotation;
    private float counter;
    private bool inAir;

    private void Start()
    {
        Debug.Log("Egg layed");
        inAir = Random.Range(0, 2) == 0;
        realChange = change * Time.fixedDeltaTime;
        rotation = Random.Range(-maxRotation, maxRotation);
        realVel = velocity * Time.fixedDeltaTime;
        StartCoroutine(Move());
    }

    private void FixedUpdate()
    {
        transform.Rotate(Vector3.forward, rotation);
        transform.localPosition += Vector3.left * realVel;
    }

    IEnumerator Move()
    {
        //Gehe in Startposition:
        float startPos_y = inAir ? 30 : 25;
        while(inAir? transform.localPosition.y < startPos_y : transform.localPosition.y > startPos_y)
        {
            transform.localPosition += Vector3.up * (inAir ? realChange : -realChange);
            yield return new WaitForFixedUpdate();
        }

        //Spiele Animation ab
        counter = 0;
        float factor = Random.Range(.5f, 2.5f);
        while (true)
        {
            counter += Time.deltaTime * 2;
            if (counter > 1) { counter--; factor = Random.Range(.5f, 3f); }
            transform.localPosition = new Vector3(transform.localPosition.x , startPos_y + factor * (inAir ? airCurve.Evaluate(counter) : groundBounce.Evaluate(counter))); 
            yield return new WaitForEndOfFrame();
        }
    }
}
