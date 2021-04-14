using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonScript : MonoBehaviour
{
    [Range(0, 1)]
    public float startPoint;
    void Start() => transform.GetChild(0).GetComponent<Animator>().Play("Hover", 0, startPoint);
}
