using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class directorController : MonoBehaviour
{

    [SerializeField] private Camera cam;
    [SerializeField] private GameObject[] asteroidPrefabs;
    private Vector3 screenBounds;
    private float lastCalled = 0f;
    private float waitTime = 2f;
    private float rightBound;
    private float upperBound;

    // Start is called before the first frame update
    void Start()
    {
        screenBounds = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        rightBound = screenBounds.x + 6f;
        upperBound = screenBounds.y + 6f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.realtimeSinceStartup - lastCalled > waitTime)
        {
            SpawnAstroids(1);
            lastCalled = Time.realtimeSinceStartup;
            //waitTime -= 1;
        }
    }

    void SpawnAstroids(int score)
    {
        float total = 0;
        while (total < score)
        {
            float x = Mathf.Round(Random.value * 3f) + 1f;
            total += x;

            int targetIndex = Mathf.FloorToInt(Mathf.Round(Random.value * 3));
            targetIndex = targetIndex == 3 ? 0 : targetIndex;
            Vector3 spawnLocation = GetSpawn();
            GameObject astroClone = Instantiate(asteroidPrefabs[targetIndex], spawnLocation, Quaternion.identity);
            Asteroid astroScript = astroClone.GetComponent<Asteroid>();


            Vector3 direction = GetDirection(spawnLocation);
            
            astroScript.SetStart(direction);
        }
    }

    private Vector3 GetSpawn()
    {
        float xStart = (Random.value * 2f * rightBound) - (rightBound);
        float yStart = (Random.value * 2f * upperBound) - (upperBound);
        if (xStart < rightBound - 5f && xStart > 5f - rightBound &&
            yStart < upperBound - 5f && yStart > 5f - upperBound)
        {
            if (upperBound - Mathf.Abs(yStart) > rightBound - Mathf.Abs(xStart))
            {
                xStart = rightBound * Mathf.Sign(xStart);
            }
            else
            {
                yStart = upperBound * Mathf.Sign(yStart);
            }
        }
        Vector3 spawnLocation = new Vector3(xStart, yStart, 0f);
        return spawnLocation;
    }

    private Vector3 GetDirection(Vector3 spawnLocation)
    {
        float xStart = Random.value * (rightBound - 6f) * 2f - (rightBound - 6f);
        float yStart = Random.value * (upperBound - 6f) * 2f - (upperBound - 6f);
        Vector3 target = new Vector3(xStart, yStart, 0f);
        Vector3 direction  = Vector3.Normalize(target - spawnLocation);
        return direction;
    }
}
