using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Seq_Preparty : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        Text text = transform.GetChild(0).GetComponent<Text>();

        float timeStep = Time.fixedDeltaTime / 3;
        for(float count = 0; count < 1; count += timeStep)
        {
            text.color = new Color(1, 1, 1, count);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(1);

        for (float count = 0; count < 1; count += timeStep)
        {
            text.color = new Color(1, 1, 1, 1-count);
            yield return new WaitForFixedUpdate();
        }

        SceneManager.LoadScene("Party");
        yield break;
    }
}
