using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class TasklistScript : MonoBehaviour
{

    public static GameObject taskList;
    private void OnEnable()
    {
        taskList = gameObject;
        StartCoroutine(UpdateList());
    }

    IEnumerator UpdateList()
    {
        yield return new WaitForSeconds(1);
        Transform tasklist = transform.GetChild(1);

        if (progress.potatoDone && !tasklist.GetChild(0).GetChild(0).gameObject.activeSelf)
            StartCoroutine(UpdateTask(tasklist.transform.GetChild(0).GetChild(0).gameObject));

        if (progress.brewDone && !tasklist.GetChild(1).GetChild(0).gameObject.activeSelf)
            StartCoroutine(UpdateTask(tasklist.transform.GetChild(1).GetChild(0).gameObject));

        if (progress.huntDone && !tasklist.GetChild(2).GetChild(0).gameObject.activeSelf)
            StartCoroutine(UpdateTask(tasklist.transform.GetChild(2).GetChild(0).gameObject));

        if (progress.millState > 2 && !tasklist.GetChild(3).GetChild(0).gameObject.activeSelf)
            StartCoroutine(UpdateTask(tasklist.transform.GetChild(3).GetChild(0).gameObject));

        if (progress.marketDone && !tasklist.GetChild(4).GetChild(0).gameObject.activeSelf)
            StartCoroutine(UpdateTask(tasklist.transform.GetChild(4).GetChild(0).gameObject));
        yield break;
    }

    IEnumerator UpdateTask(GameObject taskLine)
    {
        Rect line = taskLine.GetComponent<RectTransform>().rect;
        float endSize = line.width;
        line.width = 20;
        float step = (endSize - line.width) * Time.fixedDeltaTime / .5f;
        taskLine.SetActive(true);

        for(float count = 0; count < .5f; count += Time.fixedDeltaTime)
        {
            line.width += step;
            yield return new WaitForFixedUpdate();
        }

        yield break;
    }

    public static IEnumerator SetTaskList(bool active)
    {
        //Nehme Liste raus:
        if (!taskList) yield break;
        CanvasGroup taskListSprite = canvas.transform.GetChild(canvas.transform.childCount - 3).GetChild(0).GetComponent<CanvasGroup>();
        taskListSprite.gameObject.SetActive(true);
        for (float count = 1; count > 0; count -= Time.fixedDeltaTime)
        {
            taskListSprite.alpha = active ? 1 - count : count;
            yield return new WaitForFixedUpdate();
        }
        taskListSprite.gameObject.SetActive(active);
        yield break;
    }
}
