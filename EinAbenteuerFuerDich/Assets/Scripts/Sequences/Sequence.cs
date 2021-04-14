using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;
using static CameraScript;

public class Sequence : MonoBehaviour
{
    protected Animator anim_player, anim_comp;
    /*
    private void Awake()=> SceneManager.sceneLoaded += OnLevelLoaded;
    private void OnDestroy() => SceneManager.sceneLoaded -= OnLevelLoaded;
    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)=> StartCoroutine(PlaySequence());
    //wenn player mit animiert wird, dann am ende:
    SceneManager.MoveGameObjectToScene(player, SceneManager.GetActiveScene());//cuz I am stupid!
    SceneManager.MoveGameObjectToScene(companion, SceneManager.GetActiveScene());
    */

    protected virtual IEnumerator PlaySequence()
    {
        Debug.Log("Sequence not impmeneted");
        yield break;
    }

    protected IEnumerator Setup()
    {
        runGame = true;
        pauseMove = true;
        staticCam = true;
        StartCoroutine(TasklistScript.SetTaskList(false));
        StartCoroutine(cScript.SetRotation());

        player.GetComponent<PlayerScript>().enabled = false;
        companion.GetComponent<FollowScript>().enabled = false;
        player.GetComponent<Collider2D>().enabled = false;
        player.GetComponent<Rigidbody2D>().simulated = false;

        anim_player = player.transform.GetChild(0).GetComponent<Animator>();
        anim_comp = companion.transform.GetChild(0).GetComponent<Animator>();
        yield return new WaitForSeconds(.1f);
        anim_player.SetBool("inAir", false);
        anim_comp.SetBool("inAir", false);
        anim_player.SetBool("moving", false);
        anim_comp.SetBool("moving", false);
        yield break;
    }

    protected virtual void EndSequence(bool getDestroyed = true)
    {
        runGame = false;
        staticCam = false;
        pauseMove = false;
        StartCoroutine(TasklistScript.SetTaskList(true));
        companion.GetComponent<FollowScript>().enabled = true;
        player.GetComponent<PlayerScript>().enabled = true;
        player.GetComponent<Collider2D>().enabled = true;
        player.GetComponent<Rigidbody2D>().simulated = true;
        anim_player.SetTrigger("startMove");
        anim_comp.SetTrigger("startMove");
        cScript.target = player;
        cScript.offset = Vector2.up * 2;
        if(getDestroyed) Destroy(gameObject);
    }
}
