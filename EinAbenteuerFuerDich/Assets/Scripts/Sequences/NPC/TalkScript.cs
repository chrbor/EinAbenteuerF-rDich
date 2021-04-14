using UnityEngine;
using static GameManager;
using static CameraScript;
using System.Collections;

public class TalkScript : Sequence
{
    protected Animator anim_char;
    protected bool ready;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (runGame || pauseGame || other.gameObject.layer != 14/*Touch*/) return;
        if (!other.GetComponent<TouchSensor>().tipped) return;

        anim_char = transform.GetChild(0).GetComponent<Animator>();
        anim_char.Play("Squeesh");
        //setze order:
        if(anim_char.transform.GetChild(0).GetComponent<SpriteRenderer>()) anim_char.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder += 3;
        else anim_char.transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sortingOrder += 3;
        anim_char.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sortingOrder += 3;
        Transform tfm_acc = anim_char.transform.GetChild(0).GetChild(0);
        for (int i = 0; i < tfm_acc.childCount; i++) tfm_acc.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder += 3;

        if (gameObject.GetComponent<NPCScript>()) gameObject.GetComponent<NPCScript>().enabled = false;
        cScript.target = gameObject;
        cScript.offset = new Vector2((player.transform.position.x - gameObject.transform.position.x)/2 , 2);
        StartCoroutine(PlaySequence());
        StartCoroutine(SetCompPos());
    }

    protected IEnumerator SetCompPos()
    {
        ready = false;
        yield return new WaitForSeconds(.1f);

        anim_comp.SetBool("moving", true);
        anim_comp.SetTrigger("startMove");
        Vector3 step = Vector3.right * .1f * Mathf.Sign(player.transform.position.x - companion.transform.position.x) * (Mathf.Abs(player.transform.position.x - companion.transform.position.x) > 4 ? 1 : -1);
        while(Mathf.Abs(Mathf.Abs(player.transform.position.x - companion.transform.position.x) - 4) > .1f)
        {
            companion.transform.position += step;
            yield return new WaitForFixedUpdate();
        }
        anim_comp.SetBool("moving", false);
        ready = true;
        yield break;
    }

    protected override void EndSequence(bool getDestroyed = false)
    {
        anim_char.SetTrigger("startMove");
        //setze order:
        if (anim_char.transform.GetChild(0).GetComponent<SpriteRenderer>()) anim_char.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder -= 3;
        else anim_char.transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sortingOrder -= 3;
        anim_char.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sortingOrder -= 3;
        Transform tfm_acc = anim_char.transform.GetChild(0).GetChild(0);
        for (int i = 0; i < tfm_acc.childCount; i++) tfm_acc.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder += 3;
        if (gameObject.GetComponent<NPCScript>()) gameObject.GetComponent<NPCScript>().enabled = true;
        base.EndSequence(getDestroyed);
    }
}
