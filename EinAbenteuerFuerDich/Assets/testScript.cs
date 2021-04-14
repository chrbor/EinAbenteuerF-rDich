using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TouchAndScreen;

public class testScript : MonoBehaviour
{
    public GameObject target;
    RectTransform tfm;

    // Start is called before the first frame update
    void Start()
    {
        tfm = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        tfm.anchoredPosition = WorldToPixel(target.transform.position, tfm.anchorMin);

    }
}
