using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InGameButton : MonoBehaviour
{
    private UnityAction ButtonPressed;
    private UnityAction ButtonReleased;
    private int fingerID = -1;
    [HideInInspector]
    public bool active;
    private bool released;

    private Animator anim;
    private AudioSource aSrc;
    public AudioClip error;

    private void Start()
    {
        aSrc = GetComponent<AudioSource>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        anim.Play("BluePulse", 0);
        active = true;
    }

    public void SetCallback(UnityAction _ButtonPressed, UnityAction _ButtonReleased)
    {
        ButtonPressed = _ButtonPressed;
        ButtonReleased = _ButtonReleased;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (fingerID != -1 || !active) return;
        fingerID = other.GetComponent<TouchSensor>().fingerID;

        if (!released)
        {
            ButtonPressed.Invoke();
            anim.Play("GreenPulse", 0);
            StartCoroutine(PlayShock());
        }
        released = false;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (fingerID != other.GetComponent<TouchSensor>().fingerID || !active) return;
        fingerID = -1;
        StartCoroutine(ConfirmRelease());
    }

    IEnumerator ConfirmRelease()
    {
        released = true;
        yield return new WaitForSeconds(.1f);
        if (!released) yield break;

        ButtonReleased.Invoke();
        aSrc.pitch = 1;
        aSrc.clip = error;
        aSrc.Play();
        anim.Play("RedShake", 0);
        yield break;
    }

    IEnumerator PlayShock()
    {
        aSrc.pitch = 1 + 0.1f * fingerID;
        aSrc.Play();
        Transform shock = transform.GetChild(1);
        SpriteRenderer shockSprite = shock.GetComponent<SpriteRenderer>();
        shock.gameObject.SetActive(true);
        for (float count = 0; count < 1; count += 2 * Time.deltaTime) { yield return new WaitForEndOfFrame(); shock.localScale += Vector3.one * count; shockSprite.color -= Color.black * Time.deltaTime; }
        shock.gameObject.SetActive(false);
        yield break;
    }
}
