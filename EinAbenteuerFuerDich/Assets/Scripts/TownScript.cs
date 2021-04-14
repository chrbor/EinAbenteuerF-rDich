using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownScript : MonoBehaviour
{
    AudioSource aSrc;
    public float maxDist;
    float dist;

    // Start is called before the first frame update
    void Start()
    {
        aSrc = GetComponent<AudioSource>();
        aSrc.Play();
    }

    // Update is called once per frame
    void Update()
    {
        dist = Camera.main.transform.position.x - transform.position.x;
        if (maxDist < Mathf.Abs(dist)) { aSrc.volume = 0; return; }
        dist /= maxDist;
        aSrc.volume = (1 - Mathf.Abs(dist)) * .2f;
        aSrc.panStereo = dist;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, maxDist);
    }
}
