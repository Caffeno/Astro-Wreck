using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Timeline;

public class directorController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Vector2 playAreaSize;

    [SerializeField] private Vector2 minSpawnDistances;




    [SerializeField] private Camera cam;
    [SerializeField] private GameObject[] asteroidPrefabs;
    private Vector3 screenBounds;
    private float lastCalled = 0f;
    private float waitTime = 2f;
    private float rightBound;
    private float upperBound;
    private float calls = 0;
    private playerControler PC;
    private float leftRightCuttoff;

    // Start is called before the first frame update
    void Start()
    {
        float sideArea = 4 * (playAreaSize.x - minSpawnDistances.x) * minSpawnDistances.y;
        float capArea = 4 * playAreaSize.x * (playAreaSize.y - minSpawnDistances.y);
        leftRightCuttoff = sideArea / (sideArea + capArea);
        screenBounds = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        rightBound = screenBounds.x + 6f;
        upperBound = screenBounds.y + 6f;
        PC = player.GetComponent<playerControler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.realtimeSinceStartup - lastCalled > waitTime)
        {
            SpawnAstroids(10);
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
            astroScript.SetPlayAreaSize(playAreaSize);
            astroScript.SetPlayer(player);
        }
    }

    private Vector3 GetSpawn()
    {
        Vector3 spawnDirection = new Vector3(Random.value - 0.5f, Random.value - 0.5f);
        float spawnRegion = Random.value;
        float spawnX;
        float spawnY;

        if (spawnRegion < leftRightCuttoff)
        {
            spawnX = Mathf.Sign(spawnDirection.x) * minSpawnDistances.x + spawnDirection.x * (playAreaSize.x - minSpawnDistances.x);
            spawnY = minSpawnDistances.y * spawnDirection.y;
        }
        else
        {
            spawnX = playAreaSize.x * spawnDirection.x;
            spawnY = Mathf.Sign(spawnDirection.y) * minSpawnDistances.y + spawnDirection.y * (playAreaSize.y - minSpawnDistances.y);
        }

        Vector3 relitiveSpawnLocation = new Vector3(spawnX, spawnY);
        Vector3 spawnLocation = player.transform.position + relitiveSpawnLocation;



        return spawnLocation;
    }

    private Vector3 GetDirection(Vector3 spawnLocation)
    {
        Vector3 relitiveTarget = new Vector3(Random.value, Random.value, 0f).normalized * (Random.value * 10);
        Vector3 target = relitiveTarget + player.transform.position + PC.GetPlayerVelocity() * 100;
        Vector3 direction  = Vector3.Normalize(target - spawnLocation);
        return direction;
    }
}
