using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreen : MonoBehaviour
{
    public UnitInfo[] info;

    [System.Serializable]
    public class UnitInfo
    {
        public GameObject unit;

        public Vector2 speed;
        public Vector2 height;
        public Vector2 waitTime;
        public bool reverseable;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AnimateStartScreen());
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    IEnumerator AnimateStartScreen()
    {
        //initialize:
        int my_ptr;
        for(int i = Random.Range(2, 6); i > 0; i--)
        {
            StartCoroutine(MoveUnit(ptr: my_ptr = Random.Range(2, info.Length), rotStart: Random.Range(-80, 80)));
        }

        while (true)
        {
            StartCoroutine(MoveUnit(ptr: my_ptr = Random.Range(0, info.Length)));
            yield return new WaitForSeconds(Random.Range(info[my_ptr].waitTime.x, info[my_ptr].waitTime.y));
        }
    }

    IEnumerator MoveUnit(int ptr, float rotStart = 100)
    {
        bool rotrev = Random.Range(0, 2) == 0 && info[ptr].reverseable;

        float start = rotrev ? -rotStart : rotStart;
        float goal = info[ptr].reverseable ? Mathf.Abs(start) + 100 : 100 + start;
        float rotStep = (rotrev ? 1 : -1) * Random.Range(info[ptr].speed.x, info[ptr].speed.y);


        GameObject obj = Instantiate(info[ptr].unit, Vector3.down * 8, Quaternion.Euler(0,0,start));
        obj.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = start > 0;
        obj.transform.GetChild(0).localPosition += Random.Range(info[ptr].height.x, info[ptr].height.y) * Vector3.up;

        float step = Mathf.Abs(rotStep);
        for (float count = 0; count < goal; count += step)
        {
            obj.transform.Rotate(Vector3.forward, rotStep);
            yield return new WaitForFixedUpdate();
        }

        Destroy(obj);
        yield break;
    }
}
