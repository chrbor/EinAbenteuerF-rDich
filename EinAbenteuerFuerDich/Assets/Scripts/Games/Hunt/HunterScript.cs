using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    private bool eggBlock, lossBlock;
    public HuntScript prey;
    private float gainPerStep;

    private Text scoreText;

    private Vibration vib;

    private void Awake()
    {
        player = gameObject;
        scoreText = canvas.transform.GetChild(2).GetChild(1).GetComponent<Text>();
        scoreText.text = "0/" + prey.eggGoal;
        rb = GetComponent<Rigidbody2D>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        anim.SetBool("moving", false);

        //Setup der Vibration:
        vib = new Vibration();
        vib.SetVibrationEffect(new long[30] { 1, 10, 1, 10, 1, 10, 1, 5, 1, 5, 1, 5, 1, 5, 1, 5, 1, 5, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2 });
    }

    void Start()
    {

        Input.gyro.enabled = true;

        jumpReady = true;
        gainPerStep = gainPerSec * Time.deltaTime;
    }

    private void OnDestroy()
    {
        vib.DestroyVibration();
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
        if (!runGame) return;

        // Falle zurück
        switch (other.tag)
        {
            case "obstacle":
                vib.Vibrate(0);
                StartCoroutine(AddLoss());
                break;
            case "collect":
                StartCoroutine(AddEgg(other.gameObject));
                break;
            default:
                //Reach goal:
                break;
        }         
    }

    IEnumerator AddEgg(GameObject egg)
    {
        if (eggBlock) yield break;
        eggBlock = true;
        prey.eggCount++;
        scoreText.text = prey.eggCount + "/" + prey.eggGoal;
        Destroy(egg);
        yield return new WaitForSeconds(.2f);
        eggBlock = false;
    }

    IEnumerator AddLoss()
    {
        if (lossBlock) yield break;
        lossBlock = true;
        StartCoroutine(cScript.Shake());
        float stepLoss = lossPerObstacle / 50;
        for(int i = 0; i < 50; i++) { prey.preyDist += stepLoss; yield return new WaitForFixedUpdate(); }
        lossBlock = false;
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
