using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using static MenuScript;
using static CameraScript;
using static LoadSave;
using static ppAccess;

public class GameManager : MonoBehaviour
{
    public AudioClip normalMusic;
    public GameObject currentCanvas;

    public static bool runGame;
    public static bool pauseGame;
    public static bool staticCam;
    public static bool pauseMove;

    public static int difficulty;

    public static GameObject player;
    public static GameObject companion;
    public static AccessoirSet playerAccessoir;
    public static AccessoirSet companionAccessoir;

    private static bool setPlayerOnLoad;
    private static Vector2 _player_startPos { get; set; }//backing field
    public static Vector2 player_startPos
    {
        get { return _player_startPos; }
        set { _player_startPos = value; setPlayerOnLoad = true; }
    }
    private static bool setTransitionOnLoad;
    private static Material mat_Intro { get; set; }
    public static Material Mat_Intro
    {
        get { return mat_Intro; }
        set { mat_Intro = value; setTransitionOnLoad = mat_Intro != null; }
    }
    public static bool mat_Intro_reversed;

    public static GameObject canvas;

    public static GameManager manager;

    //Saved Info:
    public static Progress progress;
    public static CollectedHats hatProgress;
    public static Soundfile playerCry;

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

        canvas = currentCanvas;

        TouchScript.portraitMode = false;

        //Load Savefile:
        //DeleteSaveFile();
        //LoadProgress();
        //difficulty = progress.level;
        //LoadHats();

    }

    private void Start()
    {
        if (setPlayerOnLoad)
        {
            player.transform.position = player_startPos;
            companion.transform.position = player_startPos + Vector2.right * 5;
            Camera.main.transform.position = new Vector3(player_startPos.x, player_startPos.y, -10);
            setPlayerOnLoad = false;
        }
        if (setTransitionOnLoad) { menu.SetTransition(mat_Intro);
            if (mat_Intro_reversed) menu.DoTransition(-1, 1, true); else menu.DoTransition(1, -1, true);
            mat_Intro_reversed = false;
            setTransitionOnLoad = false;
        }
        
        postprocess.bloom.threshold.value = .95f;
        postprocess.doF.focalLength.value = 1;
        
    }

    public void ChangeClothes()=> StartCoroutine(ChangeClothes(playerAccessoir, companionAccessoir, true));
    public void ChangeClothes(AccessoirSet _playerSet, AccessoirSet _compSet)=> StartCoroutine(ChangeClothes(_playerSet, _compSet, true));
    IEnumerator ChangeClothes(AccessoirSet playerSet, AccessoirSet compSet, bool isActive = true)
    {
        //Kleide den Spieler und dessen Companion zurück:
        Transform parent_playerAcc = player.transform.GetChild(0).GetChild(0).GetChild(0);
        Transform parent_companionAcc = companion.transform.GetChild(0).GetChild(0).GetChild(0);

        StartCoroutine(clothObj.DestroyCloths(parent_playerAcc));
        yield return new WaitForSeconds(.2f);
        StartCoroutine(clothObj.DestroyCloths(parent_companionAcc));
        yield return new WaitForSeconds(.9f);
        //Füge Dektektivkleidung hinzu:
        foreach (var cloth in playerSet.accessoirs) { StartCoroutine(cloth.CreateCloth(parent_playerAcc)); yield return new WaitForFixedUpdate(); }
        yield return new WaitForSeconds(.2f);
        foreach (var cloth in compSet.accessoirs) StartCoroutine(cloth.CreateCloth(parent_companionAcc));
        yield break;
    }

    public void PlayNormal()
    {
        cScript.aSrc.Stop();
        cScript.aSrc.loop = true;
        cScript.aSrc.clip = normalMusic;
        cScript.aSrc.Play();
    }
}
