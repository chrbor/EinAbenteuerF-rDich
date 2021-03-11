using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public static CameraScript cScript;

    public GameObject target;
    [Range(0, 0.1f)]
    public float strength;
    public Vector2 offset;
    [HideInInspector]
    public AudioSource aSrc;

    // Start is called before the first frame update
    void Awake()
    {
        cScript = this;
        aSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target == null) return;

        transform.position += (Vector3)(strength * ((Vector2)(target.transform.position - transform.position) + offset)); 
    }

    public IEnumerator Shake()
    {
        for (int i = 40; i > 0; i--)
        {
            transform.position += (Vector3)((Vector2)Random.insideUnitSphere * i * Camera.main.orthographicSize / 1000);
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }
}
