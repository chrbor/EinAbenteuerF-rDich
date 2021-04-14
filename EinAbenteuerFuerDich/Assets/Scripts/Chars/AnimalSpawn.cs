using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class AnimalSpawn : MonoBehaviour
{
    public int maxCount;
    public Vector2 timeFrame;
    public GameObject[] animals;
    public static int animalCount;


    // Start is called before the first frame update
    void Start()
    {
        animalCount = 0;
        StartCoroutine(SpawnAroundCamera());
    }

    IEnumerator SpawnAroundCamera()
    {
        while (true)
        {
            yield return new WaitUntil(() => animalCount < maxCount);
            yield return new WaitForSeconds(Random.Range(timeFrame.x, timeFrame.y));

            animalCount++;
            int ran = Random.Range(0, animals.Length);
            Instantiate(animals[ran], new Vector3(player.transform.position.x + Camera.main.orthographicSize * (-3 + Random.Range(0, 2) * 6), ran == 0 ? -2 : Random.Range(0, 8) ), Quaternion.identity);
        }

    }
}
