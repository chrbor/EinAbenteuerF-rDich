using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Seq_Death : MonoBehaviour
{
    Text text;
    // Start is called before the first frame update
    void Start()
    {
        LoadSave.DeleteSaveFile();
        text = transform.GetChild(0).GetComponent<Text>();
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        text.color *= new Color(1, 1, 1, 0);
        Color stepColor = Color.black * Time.fixedDeltaTime / 3;
        for(float count = 0; count < 3; count += Time.fixedDeltaTime)
        {
            text.color += stepColor;
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(1);
        for (float count = 0; count < 3; count += Time.fixedDeltaTime)
        {
            text.color -= stepColor;
            yield return new WaitForFixedUpdate();
        }

        SceneManager.LoadScene("Credits");
        yield break;
    }
}
