using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static MenuScript;
using static GameManager;

public class CreditScript : MonoBehaviour
{
    public float rollSpeed;
    public float skipSpeed;
    RectTransform rTransform;

    public Material transition;
    // Start is called before the first frame update
    void Awake()
    {
        rTransform = GetComponent<RectTransform>();
        StartCoroutine(RollCredits());
    }

    IEnumerator RollCredits()
    {
        float endPos = rTransform.anchoredPosition.y;
        rTransform.anchoredPosition = Vector2.down * (endPos + 20);

        while(rTransform.anchoredPosition.y < endPos)
        {
            rTransform.anchoredPosition += Vector2.up * (Input.touchCount > 0 || Input.anyKey ? skipSpeed : rollSpeed);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => Input.touchCount > 0 || Input.anyKey);

        Mat_Intro = transition;

        Image panel = transform.parent.GetChild(1).GetComponent<Image>();
        panel.gameObject.SetActive(true);
        float timeStep = Time.fixedDeltaTime / 3;
        for(float count = 0; count < 1; count += timeStep)
        {
            panel.color = Color.black * count;
            yield return new WaitForFixedUpdate();
        }
        SceneManager.LoadScene(0);
        yield break;
    }
}
