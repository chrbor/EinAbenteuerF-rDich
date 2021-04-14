using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDoorScript : MonoBehaviour
{
    public float minAngle;
    public float maxAngle;
    Rigidbody2D rb;
    private float angle;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.centerOfMass = Vector2.zero;
    }

    void FixedUpdate()
    {
        if (!rb.simulated) return;

        //Debug.Log((Mathf.Asin(rb.rotation * Mathf.Deg2Rad) * Mathf.Sign(rb.rotation)));
        rb.angularVelocity += Mathf.Sin(rb.rotation * Mathf.Deg2Rad) * rb.gravityScale;

        angle = transform.localRotation.eulerAngles.z;
        while (Mathf.Abs(angle) > 180) angle -= 360 * Mathf.Sign(angle);

        //Checke perimeter:
        if (angle < minAngle) { rb.rotation = minAngle; rb.angularVelocity = Mathf.Abs(rb.angularVelocity) * (1 - rb.angularDrag); return; }
        if(angle > maxAngle) { rb.rotation = maxAngle; rb.angularVelocity = Mathf.Abs(rb.angularVelocity) * (rb.angularDrag - 1); return; }
    }
}
