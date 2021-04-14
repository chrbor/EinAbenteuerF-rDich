using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using static PotatoGame;
using static GameManager;

public class PotstoScript : MonoBehaviour
{
    public AnimationCurve FlightVec;
    public Sprite[] sprites;
    float dist;
    bool falling, vibrating;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (falling || vibrating) return;
        StartCoroutine(Vibrate(other.transform));
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        vibrating = false;
    }

    private void Update()
    {
        if (!runGame) Destroy(gameObject);
    }

    IEnumerator Vibrate(Transform other)
    {
        vibrating = true;
        while (vibrating)
        {
            dist = Mathf.Abs(other.position.x - transform.position.x);

            if (potatoGame.digging && dist < 2)
            {
                //falle in den Korb:
                falling = true;
                vibrating = false;
                StartCoroutine(FallIntoBasket());
            }
            if (dist < potatoGame.minDist) potatoGame.minDist = dist;

            yield return new WaitForFixedUpdate();
        }
        yield break;
    }

    IEnumerator FallIntoBasket()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
        transform.parent = companion.transform.GetChild(0).GetChild(0).GetChild(0);
        Vector3 startPos = transform.localPosition;
        float diff = -1.75f + Random.Range(-.1f, .1f) - startPos.x;
        float rotation = Random.Range(-10, 10);
        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            transform.localPosition = startPos + new Vector3(diff * count, FlightVec.Evaluate(count));
            transform.Rotate(Vector3.forward, rotation);
            yield return new WaitForFixedUpdate();
        }

        transform.parent = transform.parent.GetChild(2);
        canvas.transform.GetChild(canvas.transform.childCount - 3).GetChild(1).GetChild(potatoGame.potatoCount++).GetComponent<Image>().color = Color.white;
        yield break;
    }
}
