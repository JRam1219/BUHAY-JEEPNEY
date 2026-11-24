using UnityEngine;

[System.Serializable]
public struct RouteOption
{
    public string destinationName;
    public int fareAmount;
    public float travelDistance;
}

public class SCR_NPCSpawner : MonoBehaviour
{
    [Header("Spawner Setup")]
    public GameObject[] npcPrefabs;
    public Transform spawnPoint;
    public Transform jeepneyEntrance;
    
    [Header("Possible Destinations, Fares & Distances")]
    public RouteOption[] routeOptions = new RouteOption[]
    {
        new RouteOption { destinationName = "Kanto", fareAmount = 13, travelDistance = 40f },
        new RouteOption { destinationName = "Palengke", fareAmount = 15, travelDistance = 75f },
        new RouteOption { destinationName = "Hospital", fareAmount = 20, travelDistance = 120f }
    };

    [Header("Random Spawn Timing")]
    public float minSpawnInterval = 2f;
    public float maxSpawnInterval = 7f;

    private float timer;
    private float nextSpawnTime;

    private void Start()
    {
        nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= nextSpawnTime)
        {
            SpawnRandomNPC();
            timer = 0f;
            nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    public void SpawnRandomNPC()
    {
        if (npcPrefabs.Length == 0 || routeOptions.Length == 0) return;

        GameObject prefab = npcPrefabs[Random.Range(0, npcPrefabs.Length)];
        RouteOption chosenRoute = routeOptions[Random.Range(0, routeOptions.Length)];

        GameObject npcObj = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        if (npcObj.TryGetComponent(out SCR_PassengerNPC npc))
        {
            npc.Initialize(chosenRoute.destinationName, chosenRoute.fareAmount, chosenRoute.travelDistance, jeepneyEntrance);
        }
    }
}