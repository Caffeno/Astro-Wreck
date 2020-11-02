using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Timeline;

public class directorController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float activeAreaRadius;

    [SerializeField] private float minSpawnDistance;
    [SerializeField] private float spawnAreaThickness;
    [SerializeField] private float despawnDistance;




    [SerializeField] private Camera cam;
    [SerializeField] private GameObject[] asteroidPrefabs;
    private Vector3 screenBounds;
    private float lastCalled = 0f;
    private float waitTime = 2f;
    private float rightBound;
    private float upperBound;
    private float calls = 0;
    private playerControler PC;

    // Start is called before the first frame update
    void Start()
    {
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
            Debug.Log(targetIndex);
            Debug.Log(asteroidPrefabs[targetIndex]);

            GameObject astroClone = Instantiate(asteroidPrefabs[targetIndex], spawnLocation, Quaternion.identity);
            Asteroid astroScript = astroClone.GetComponent<Asteroid>();


            Vector3 direction = GetDirection(spawnLocation);
            
            astroScript.SetStart(direction);
            astroScript.SetActiveRadius(activeAreaRadius);
            astroScript.SetPlayer(player);
            astroScript.SetDespawnRadius(despawnDistance);
        }
    }

    private Vector3 GetSpawn()
    {
        Vector3 spawnDirection = new Vector3(Random.value - 0.5f, Random.value - 0.5f).normalized;
        Vector3 relitiveSpawnLocation = spawnDirection * (minSpawnDistance + spawnAreaThickness * Random.value);
        Vector3 spawnLocation = player.transform.position + relitiveSpawnLocation;
        Debug.Log(spawnLocation);
        Debug.Log(calls);
        calls += 1;


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
