using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class BuildedHouseScript : MonoBehaviour
{
    bool talking;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.GetComponent<TouchSensor>().tipped || runGame || talking || other.gameObject.layer != 14/*Touch*/) return;
        pauseMove = true;
        talking = true;
        StartCoroutine(Talk());
    }

    IEnumerator Talk()
    {
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("stopBuild");
        transform.GetChild(0).GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(.1f);
        transform.GetChild(1).localScale = new Vector3(player.transform.position.x > transform.GetChild(1).position.x ? -2 / 3 : 2 / 3, 2 / 3);
        //Talk:
        yield return new WaitUntil(() => Input.touchCount == 0);
        yield return new WaitUntil(() => Input.touchCount != 0);
        yield return new WaitUntil(() => Input.touchCount == 0);

        transform.GetChild(0).GetComponent<Collider2D>().enabled = true;
        anim.Play("BuildHouse");
        talking = false;
        pauseMove = false;
        yield break;
    }
}
