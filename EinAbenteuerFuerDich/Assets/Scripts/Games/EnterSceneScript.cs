using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static MenuScript;
using static CameraScript;
using UnityEngine.SceneManagement;

public class EnterSceneScript : MonoBehaviour
{
    public string sceneToOpen;
    public Material transition_material;
    public Material transition_next;
    public float direction = 1;
    public float closing = 1;

    /*
    private void Start()
    {
        StartCoroutine(ChangeScene());
    }
    //*/
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.GetComponent<TouchSensor>().tipped || runGame || pauseGame || other.gameObject.layer != 14/*Touch*/) return;
        if (SceneManager.GetSceneByName(sceneToOpen) == null) return;

        StartCoroutine(ChangeScene());
    }

    IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(.1f);
        if (pauseGame) yield break;

        pauseMove = true;
        staticCam = true;
        runGame = true;
        StartCoroutine(cScript.SetRotation());
        menu.SetTransition(transition_material);
        menu.DoTransition(direction, closing, false);
        Mat_Intro = transition_next;
        StartCoroutine(cScript.SetBGM());
        yield return new WaitForSeconds(2.4f);
        SceneManager.LoadScene(sceneToOpen);
        yield break;
    }
}
