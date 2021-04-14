using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class NPCScript : MonoBehaviour
{
    [Header("MovingArea")]
    public bool centerAsOffset;
    public float x_center;
    public float radius;

    [Header("Behaviour")]
    public float moveSpeed;
    public float minWaitTime, maxWaitTime;


    private float count = 0;
    private float x_goal;
    private float realVel;
    private Animator anim;

    private void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
        anim.Play("Idle");

        if (centerAsOffset) x_center += transform.localPosition.x;
        x_goal = transform.localPosition.x;
        realVel = moveSpeed * Time.fixedDeltaTime;
        minWaitTime = minWaitTime > maxWaitTime ? maxWaitTime : minWaitTime;
    }

    private void FixedUpdate()
    {
        if (pauseGame) return;

        if (count > 0)
        {
            count -= Time.fixedDeltaTime;
            if (count < 0)
            {
                transform.localScale = new Vector3(Mathf.Sign(x_goal - transform.localPosition.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
                anim.SetBool("moving", true);
            }
            return;
        }

        if (Mathf.Abs(x_goal - transform.localPosition.x) > realVel)
        {
            transform.localPosition += Vector3.right * realVel * Mathf.Sign(x_goal - transform.localPosition.x);
            return;
        }

        //set next cycle
        count = Random.Range(minWaitTime, maxWaitTime);
        x_goal = x_center + Random.Range(-radius, radius);
        anim.SetBool("moving", false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        float x_real = centerAsOffset ? transform.position.x + x_center : x_center;
        Gizmos.DrawWireSphere(new Vector3(x_real, transform.position.y), 1);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(x_real - radius, transform.position.y), new Vector3(x_real + radius, transform.position.y));
    }
}
