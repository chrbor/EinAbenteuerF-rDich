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

    [Header("Kleidung:")]
    public AccessoirSet accessoir;

    Animator anim;
    PlayerScript pScript;
    bool preOnGround = true;
    bool onGround;

    private void Awake()
    {
        companion = gameObject;
        companionAccessoir = accessoir;   
        anim = transform.GetChild(0).GetComponent<Animator>();
    }

    private void Start()
    {
        pScript = player.GetComponent<PlayerScript>();
    }

    private void OnEnable()
    {
        isMoving = false;
        prePos.Clear();
        diff = Vector3.zero;        
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
        else if(!onGround || (prePos[0] - player.transform.position).sqrMagnitude > thresh_stop)
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

        if (!onGround && preOnGround) { anim.SetTrigger("jump"); preOnGround = false; }
        anim.SetBool("inAir", !onGround);
        preOnGround |= onGround;
    }

    private void OnTriggerStay2D(Collider2D other) => onGround |= other.gameObject.layer == 12;
    private void OnTriggerExit2D(Collider2D other) => onGround &= other.gameObject.layer != 12;
    
}
