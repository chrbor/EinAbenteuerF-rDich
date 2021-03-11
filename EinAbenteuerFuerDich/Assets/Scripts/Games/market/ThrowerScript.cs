using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MarketGame;
using static GameManager;

public class ThrowerScript : MonoBehaviour
{
    public float movingSpeed;
    private float stepSpeed;

    public GameObject[] throwables;

    private bool throwing;
    private float maxRunRange;
    private float y_end;
    private Vector2 falltimeRange;

    private void Start()
    {
        stepSpeed = movingSpeed * Time.fixedDeltaTime;
    }
    public void SetParams(float _maxRunRange, Vector2 _falltimeRange, float _y_end)
    {
        maxRunRange = _maxRunRange;
        falltimeRange = _falltimeRange;
        y_end = _y_end;
    }
    public void StopThrowing() => throwing = false;

    public IEnumerator ThrowObjects()
    {
        float x_throwPos;
        FallObject throwable;
        Animator anim = transform.GetChild(0).GetComponent<Animator>();

        throwing = true;
        while (throwing && runGame)
        {
            //Move to Random Position
            x_throwPos = Random.Range(-maxRunRange, maxRunRange);
            float sign = Mathf.Sign(x_throwPos - transform.position.x);
            anim.Play("Move", 0);
            while(Mathf.Abs(x_throwPos - transform.position.x) > stepSpeed)
            {
                transform.position += Vector3.right * sign * stepSpeed;
                yield return new WaitForFixedUpdate();
            }
            anim.Play("Idle", 0);

            yield return new WaitForSeconds(0.1f);
            //Spiele Animation:
            anim.Play("squeesh", 0);
            //Spawne Objekt:
            throwable = Instantiate(throwables[Random.Range(0, throwables.Length)], transform.position, Quaternion.identity).GetComponent<FallObject>();
            StartCoroutine(throwable.StartFall(new Vector2(Random.Range(-maxRange, maxRange), y_end), Random.Range(falltimeRange.x, falltimeRange.y)));
            yield return new WaitForSeconds(0.75f);
        }
        yield break;
    }
}
