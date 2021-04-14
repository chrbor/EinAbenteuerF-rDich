using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class EtageScript : MonoBehaviour
{
    [System.Serializable]
    public class EtageClass
    {
        public float activationHeight;
        public GameObject activationObject;
    }

    public EtageClass[] etages;

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach(EtageClass etage in etages)
            etage.activationObject.SetActive(etage.activationHeight < player.transform.position.y);
    }
}
