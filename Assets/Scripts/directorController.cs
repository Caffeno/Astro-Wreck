using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class directorController : MonoBehaviour
{
    private float lastCalled = 0f;
    private float waitTime = 60f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.realtimeSinceStartup - lastCalled > waitTime)
        {
            SpawnAstroids(1);
            lastCalled = Time.realtimeSinceStartup;
            waitTime -= 1;
        }
    }

    void SpawnAstroids(int score)
    {
        float total = 0;
        while (total < score)
        {
            float x = Mathf.Ceil(Random.value * 3f);
            x = x == 0 ? 1 : x;
            total += x;

        }
    }
}
