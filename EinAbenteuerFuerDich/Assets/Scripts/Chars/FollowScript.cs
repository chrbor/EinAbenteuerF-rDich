using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class FollowScript : MonoBehaviour
{
    public int arrayLength;
    public float thresh_stop, thresh_start;
    private List<Vector3> prePos = new List<Vector3>();
    private bool isMoving;
    private Vector3 diff, start;

    Animator anim;
    PlayerScript pScript;
    bool preOnGround = true;
    Collider2D col;

    private void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
        pScript = player.GetComponent<PlayerScript>();
        col = GetComponent<Collider2D>();
    }

    void FixedUpdate()
    {

        if(prePos.Count < arrayLength)
        {
            if(!isMoving && (transform.position - player.transform.position).sqrMagnitude > thresh_start)
            {
                diff =  (player.transform.position - transform.position) / arrayLength;
                anim.SetBool("moving", true);
                isMoving = true;
            }
            if (!isMoving) return;

            transform.position += diff;
            prePos.Add(player.transform.position);
        }
        else if(!col.IsTouchingLayers() || (prePos[0] - player.transform.position).sqrMagnitude > thresh_stop)
        {
            transform.position = prePos[0];
            prePos.RemoveAt(0);
            prePos.Add(player.transform.position);
        }
        else { prePos.Clear(); isMoving = false; anim.SetBool("moving", false); }



        /*
        diff = player.transform.position - transform.position;
        if (diff.sqrMagnitude < thresh)
        {
            if(!col.IsTouchingLayers()) transform.position += (Vector3)(Vector2.up * strength * diff);
            anim.SetBool("moving", false);
        }
        else
        {
            transform.position += (Vector3)(strength * diff);
            anim.SetBool("moving", true);
        }*/

        if (!col.IsTouchingLayers() && preOnGround) { anim.SetTrigger("jump"); preOnGround = false; }
        anim.SetBool("inAir", !col.IsTouchingLayers());
        preOnGround |= col.IsTouchingLayers();
    }
}
