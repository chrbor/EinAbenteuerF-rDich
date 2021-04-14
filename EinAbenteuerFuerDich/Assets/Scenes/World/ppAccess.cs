using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ppAccess : MonoBehaviour
{
    public static ppAccess postprocess;

    Volume volume;
    [HideInInspector]
    public Bloom bloom;
    [HideInInspector]
    public DepthOfField doF;

    // Start is called before the first frame update
    void Start()
    {
        postprocess = this;
        volume = GetComponent<Volume>();

        volume.sharedProfile.TryGet(out bloom);
        volume.sharedProfile.TryGet(out doF);
    }
}
