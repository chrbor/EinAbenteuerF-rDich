using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static CameraScript;

public class HuntScript : MonoBehaviour
{
    public float runVelocity;
    [Min(5)]
    public float minDist = 5;
    [Min(2)]
    public float maxView = 30;
    [Min(5)]
    public float preyDist = 10;
    public float eggDistance;
    [Min(0.1f)]
    public float eggTime;
    [HideInInspector]
    public float eggCount;
    public float eggGoal = 6;
    public GameObject egg;
    public HunterScript hunter;
    public AnimationCurve jumpCurve;
    private bool isJumping;

    //HuhnSteuerung:
    int mask = (1 << 10);//Obstacle

    public GameObject ground;
    public GameObject[] obstacles;
    public GameObject[] decoration;
    public List<GameObject> activeGrounds;

    private float eggTimer;
    private float realVel;
    private Animator anim;

    private void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
        realVel = runVelocity * Time.fixedDeltaTime;

        cScript.target = hunter.gameObject;

        //Test:
        StartCoroutine(RunBackground());
    }

    IEnumerator RunBackground()
    {
        //Set camera:
        cScript.target = hunter.gameObject;

        cScript.aSrc.loop = true;
        cScript.aSrc.Play();
        anim.Play("PanikRun");
        hunter.anim.SetBool("moving", true);
        eggCount = 0;

        runGame = true;
        while (runGame)
        {
            //Steuerung des Huhns:
            RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.down, Vector2.right, 10, mask);
            if(hit.collider != null)
                StartCoroutine(Jump());

            if (eggTimer > 0) eggTimer -= Time.fixedDeltaTime;
            else if(preyDist < eggDistance && Random.Range(0,100) == 0)
                StartCoroutine(LayEgg());


            //Setze Boden:
            foreach (var ground in activeGrounds) ground.transform.position += Vector3.left * realVel;
            if(activeGrounds[0].transform.position.x < -maxView * Camera.main.aspect - 75)
            {
                GameObject tmp = activeGrounds[0];
                activeGrounds.RemoveAt(0);

                for (int i = tmp.transform.childCount; i > 0; i--) Destroy(tmp.transform.GetChild(i-1).gameObject);
                tmp.transform.position = activeGrounds[1].transform.position + Vector3.right * 100;
                //Add bckgrnd:
                float pos = -50;
                int obj;
                do
                {
                    obj = Random.Range(0, decoration.Length);

                    pos += (obj < 4 ? 2 : 10) + Random.Range(0f, 10f);
                    if (pos > 50) pos = 50;
                    Instantiate(decoration[obj], new Vector3(pos, 24.85f) + tmp.transform.position, Quaternion.identity, tmp.transform);
                } while (pos < 30);

                //Add obstacles:
                pos = -50;
                do
                {
                    pos += 20 + Random.Range(0f, 20f);
                    if (pos > 50) pos = 50;
                    obj = Random.Range(0, obstacles.Length);
                    Instantiate(obstacles[obj], new Vector3(pos, obj < 2? 30 : 25) + tmp.transform.position, Quaternion.identity, tmp.transform);
                } while (pos < 30);

                activeGrounds.Add(tmp);
            }
            
            //Update der Positionen:
            if (preyDist > maxView * Camera.main.aspect) { Debug.Log("game over"); runGame = false; yield break; }
            hunter.transform.position = new Vector3(transform.position.x -preyDist, hunter.rb.position.y);
            cScript.offset = new Vector3(preyDist/2, preyDist/8);
            Camera.main.orthographicSize = (preyDist + 5) / Camera.main.aspect;

            yield return new WaitForFixedUpdate();
        }
        for (float volume = 1; volume > 0; volume -= 0.02f) { cScript.aSrc.volume = volume; yield return new WaitForFixedUpdate(); }
        cScript.aSrc.Stop();
        yield break;
    }

    IEnumerator LayEgg()
    {
        eggTimer = eggTime;

        StartCoroutine(Jump());
        yield return new WaitForSeconds(0.1f);
        GameObject obj = Instantiate(egg, transform.position, Quaternion.identity);
        obj.transform.parent = activeGrounds[2].transform;
        yield break;
    }

    IEnumerator Jump()
    {
        if (isJumping) yield break;
        isJumping = true;
        anim.SetTrigger("Jump");
        float ptr = 0;
        for(int i = 0; i < 50; i++)
        {
            ptr += 0.02f;
            transform.position = new Vector3(transform.position.x, 1.6f + jumpCurve.Evaluate(ptr));
            yield return new WaitForFixedUpdate();
        }
        anim.SetTrigger("Run");
        isJumping = false;
        yield break;
    }
}
