using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MarketGame;
using static GameManager;

public class CartScript : MonoBehaviour
{
    public float change_thresh;
    public float vel_thresh = 0.05f;
    public float damp_thresh = 0.1f;
    public float damping = 0.01f;
    public float sensitivity = 1;

    private float accel;
    private float velocity;
    private float moveDir;
    private float waitForPermCount;


    private bool rolling = false;
    private bool reseting = false;

    public Animator charAnim;
    private bool moving;

    public void SetActive(bool active) { rolling = active; moving = false; }

    private void Awake()
    {
        Input.gyro.enabled = true;
        velocity = 0;
        moveDir = 0;
    }

    void FixedUpdate()
    {
        if (!rolling || !runGame) return;

        //Einkaufswagen- movement:
        accel = sensitivity * Input.gyro.userAcceleration.x;

        if (waitForPermCount == 0) velocity += accel;
        Debug.Log("dir: " + moveDir);
        if(waitForPermCount > 0)
        {
            if (Mathf.Abs(accel) < 0.01f) { if (--waitForPermCount == 0) moveDir = 0; }
            else  waitForPermCount = 2;

            return;
        }

        if (moveDir == 0 ) { if (Mathf.Abs(velocity) > 0.05f && waitForPermCount == 0) moveDir = Mathf.Sign(velocity); }
        else if (moveDir * velocity < 0.02f) { velocity = 0; waitForPermCount = 2; }

        Debug.Log("vel: " + velocity);
        Debug.Log("accel: " + accel);

        //Transmit movement:
        if (Mathf.Abs(velocity) < damp_thresh) velocity = velocity * (1 - damping);
        if (Mathf.Abs(velocity) < vel_thresh) { if (moving) charAnim.SetBool("moving", false); moving = false; return; }
        transform.position = new Vector3(Mathf.Abs(transform.position.x + velocity) > maxRange ? Mathf.Sign(velocity) * maxRange : transform.position.x + velocity, transform.position.y);

        if (!moving) charAnim.SetBool("moving", true);
        moving = true;
    }

    public IEnumerator ResetPosition()
    {
        if (reseting) yield break;
        reseting = true;

        bool wasRolling = rolling;
        rolling = false;
        float thresh = 0.5f;
        Vector3 step = -thresh * Mathf.Sign(transform.position.x) * Vector3.right;
        charAnim.SetBool("moving", true);
        while (Mathf.Abs(transform.position.x) > thresh) { transform.position += step; yield return new WaitForFixedUpdate(); }
        charAnim.SetBool("moving", false);
        velocity = 0;
        rolling = true;

        reseting = false;
        yield break;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TouchSensor touch = other.GetComponent<TouchSensor>();
        if (touch != null/*Touch*/ && other.GetComponent<TouchSensor>().tipped)
            cartTouched = true;
    }
}
