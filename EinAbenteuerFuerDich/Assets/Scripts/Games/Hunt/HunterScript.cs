using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static CameraScript;

public class HunterScript : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody2D rb;
    [HideInInspector]
    public Animator anim;

    [Header("JumpVal:")]
    public float thresh_jump;
    public float thresh_jump_max;
    public float jumpStrength;
    private bool jumpReady;

    [Header("Huntval:")]
    public float gainPerSec;
    public float lossPerObstacle;
    private bool addingLoss;
    public HuntScript prey;
    private float gainPerStep;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        anim.SetBool("moving", false);
        Input.gyro.enabled = true;

        jumpReady = true;
        gainPerStep = gainPerSec * Time.deltaTime;
    }

    private void Update()
    {
        //Springen:
        if (Input.gyro.userAcceleration.z > thresh_jump && jumpReady && runGame)
        {
            jumpReady = false;
            rb.AddForce(Vector2.up * (Input.gyro.userAcceleration.z > thresh_jump_max ? thresh_jump_max : Input.gyro.userAcceleration.z) * jumpStrength);
            anim.SetTrigger("jump");
        }

        if(prey.preyDist > prey.minDist) prey.preyDist -= gainPerStep;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Falle zurück
        switch (other.tag)
        {
            case "obstacle":
                StartCoroutine(AddLoss());
                break;
            case "collect":
                prey.eggCount++;
                Debug.Log("egg collected");
                Destroy(other.gameObject);
                break;
            default:
                //Reach goal:
                break;
        }         
    }

    IEnumerator AddLoss()
    {
        if (addingLoss) yield break;
        addingLoss = true;
        StartCoroutine(cScript.Shake());
        float stepLoss = lossPerObstacle / 50;
        for(int i = 0; i < 50; i++) { prey.preyDist += stepLoss; yield return new WaitForFixedUpdate(); }
        addingLoss = false;
        yield break;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        jumpReady = true;
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        anim.SetBool("inAir", false);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        anim.SetBool("inAir", true);
    }
}
