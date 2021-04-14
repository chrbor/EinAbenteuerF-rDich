using System.Collections;
using UnityEngine;
using static GameManager;

public class FlourScript : MonoBehaviour
{
    bool active;
    private void Start()
    {
        StartCoroutine(WaitforOneSec());
    }

    IEnumerator WaitforOneSec()
    {
        yield return new WaitForSeconds(1);
        active = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(active)
            StartCoroutine(GetCollected());
    }

    IEnumerator GetCollected()
    {
        GetComponent<Collider2D>().enabled = false;
        transform.parent = player.transform.GetChild(0).GetChild(0).GetChild(0);
        Vector3 startPos = transform.localPosition;
        Vector3 diff = Vector3.up * 2 - startPos;
        for (float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            transform.localPosition = startPos + diff * count;
            yield return new WaitForFixedUpdate();
        }

        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            transform.localPosition += Vector3.up * .05f;
            yield return new WaitForFixedUpdate();
        }

        MillScript.flourTaken = true;

        Color colorStep = Color.black * Time.fixedDeltaTime;
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        for (float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            sprite.color -= colorStep;
            transform.localPosition += Vector3.down * .05f;
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
        yield break;
    }
}
