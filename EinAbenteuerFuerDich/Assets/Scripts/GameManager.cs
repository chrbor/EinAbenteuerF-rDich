using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CameraScript;

public class GameManager : MonoBehaviour
{
    public AudioClip normalMusic;

    public static bool runGame;
    public static bool pauseGame;
    public static bool staticCam;
    public static bool pauseMove;

    public static int difficulty;

    public static GameObject player;
    public static GameObject companion;
    public static AccessoirSet playerAccessoir;
    public static AccessoirSet companionAccessoir;

    public static GameManager manager;

    [System.Serializable]
    public class AccessoirSet
    {
        //public Transform parent;
        public clothObj[] accessoirs;

        //Destroy Accessoir:
        //for(int i = parent.childCount -1; i >= 0 ; i--) StartCoroutine(clothObj.DestroyCloths(parent));
        
        //Setting Accessoir:
        //foreach(var cloth in accessoirs) StartCoroutine(cloth.CreateCloth(parent));
    }

    private void Awake()
    {
        manager = this;

        runGame = false;
        pauseGame = false;
        pauseMove = false;
        staticCam = false;
        difficulty = 0;

        TouchScript.portraitMode = false;
    }

    private void Start()
    {
        PlayNormal();
    }

    public void PlayNormal()
    {
        cScript.aSrc.Stop();
        cScript.aSrc.loop = true;
        cScript.aSrc.clip = normalMusic;
        cScript.aSrc.Play();
    }
}
