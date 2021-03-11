using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moveleft : MonoBehaviour
{
    public float moveSpeed;

    void Update()
    {
        transform.localPosition += Vector3.left * moveSpeed;
    }
}
