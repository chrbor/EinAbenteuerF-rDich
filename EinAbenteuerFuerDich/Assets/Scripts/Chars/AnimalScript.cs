using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AnimalSpawn;

public class AnimalScript : MonoBehaviour
{
    public float speed;
    Vector3 step;
    // Start is called before the first frame update
    void Start()
    {
        if (Camera.main.transform.position.x - transform.position.x > 0)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);
        else speed *= -1;
        step = Vector3.right * speed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += step;
        if(Mathf.Abs(Camera.main.transform.position.x - transform.position.x) > Camera.main.orthographicSize * 3) { animalCount--; Destroy(gameObject); }
    }
}
