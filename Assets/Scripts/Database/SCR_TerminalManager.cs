using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_TerminalManager : MonoBehaviour
{
    [Header("Waiting Area Bounds")]
    public Transform areaMin;
    public Transform areaMax;

    [Header("NPC Spawning")]
    public GameObject[] npcPrefabs;
    public int initialWaitingCount = 8;
    public float boardingDelay = 1.2f;
    public float minSpacing = 1.0f;

    [Header("Routes & Jeepney")]
    public RouteOption[] routeOptions;
    public Transform jeepneyEntrance;
    public SCR_JeepneySeatTracker seatTracker;

    private Queue<SCR_PassengerNPC> waitingQueue = new Queue<SCR_PassengerNPC>();
    private bool isBoarding = false;

    private void Start()
    {
        SpawnWaitingCrowd();
    }

    public void SpawnWaitingCrowd()
    {
        if (npcPrefabs.Length == 0 || routeOptions.Length == 0) return;

        List<Vector2> usedPositions = new List<Vector2>();

        for (int i = 0; i < initialWaitingCount; i++)
        {
            Vector2 spawnPos = Vector2.zero;
            bool validSpot = false;

            // Try up to 30 times to find a spot that isn't too close to another NPC
            for (int attempt = 0; attempt < 30; attempt++)
            {
                float randX = Random.Range(areaMin.position.x, areaMax.position.x);
                float randY = Random.Range(areaMin.position.y, areaMax.position.y);
                spawnPos = new Vector2(randX, randY);

                validSpot = true;
                foreach (Vector2 pos in usedPositions)
                {
                    if (Vector2.Distance(spawnPos, pos) < minSpacing)
                    {
                        validSpot = false;
                        break;
                    }
                }

                if (validSpot) break;
            }

            usedPositions.Add(spawnPos);

            GameObject prefab = npcPrefabs[Random.Range(0, npcPrefabs.Length)];
            RouteOption route = routeOptions[Random.Range(0, routeOptions.Length)];

            GameObject npcObj = Instantiate(prefab, spawnPos, Quaternion.identity);
            if (npcObj.TryGetComponent(out SCR_PassengerNPC npc))
            {
                npc.Initialize(route.destinationName, route.fareAmount, route.travelDistance, null);
                waitingQueue.Enqueue(npc);
            }
        }
    }

    public void StartBoarding()
    {
        if (!isBoarding)
        {
            StartCoroutine(BoardPassengersRoutine());
        }
    }

    private IEnumerator BoardPassengersRoutine()
    {
        isBoarding = true;

        while (waitingQueue.Count > 0)
        {
            if (seatTracker.IsFull())
            {
                Debug.Log("Jeepney is full! Ready to head to the endless road.");
                break;
            }

            SCR_PassengerNPC nextNpc = waitingQueue.Dequeue();
            if (nextNpc != null)
            {
                nextNpc.SetEntranceTarget(jeepneyEntrance);
            }

            yield return new WaitForSeconds(boardingDelay);
        }

        isBoarding = false;
    }
}