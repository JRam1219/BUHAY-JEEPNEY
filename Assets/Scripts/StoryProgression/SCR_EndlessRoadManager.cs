using UnityEngine;

public class SCR_EndlessRoadManager : MonoBehaviour
{
    [Header("Day / Night Timer")]
    public float dayDurationSeconds = 120f;
    private float currentTime = 0f;
    public bool isNight { get; private set; } = false;

    [Header("Sky Time-of-Day Transition")]
    public SpriteRenderer[] skyRenderers; // Assign all scrolling sky BG tiles here
    public Gradient skyColorGradient;

    [Header("References")]
    public SCR_NPCSpawner roadSpawner;
    public SCR_JeepneySeatTracker seatTracker;

    [Header("Return to Garage")]
    public GameObject garageReturnCollider;
    public Transform garageSpawnPoint;
    private bool garageColliderSpawned = false;

    private void Start()
    {
        if (garageReturnCollider != null)
        {
            garageReturnCollider.SetActive(false);
        }

        UpdateSkyColor(0f); // Start with Morning color
    }

    private void Update()
    {
        if (!isNight)
        {
            currentTime += Time.deltaTime;
            float progress = Mathf.Clamp01(currentTime / dayDurationSeconds);
            UpdateSkyColor(progress);

            if (currentTime >= dayDurationSeconds)
            {
                StartNightPhase();
            }
        }
    }

    private void UpdateSkyColor(float progress)
    {
        if (skyRenderers == null || skyColorGradient == null) return;

        Color currentSkyColor = skyColorGradient.Evaluate(progress);
        foreach (SpriteRenderer sr in skyRenderers)
        {
            if (sr != null)
            {
                sr.color = currentSkyColor;
            }
        }
    }

    private void StartNightPhase()
    {
        isNight = true;
        Debug.Log("Night has fallen! Drop off remaining passengers to return to the garage.");

        if (roadSpawner != null)
        {
            roadSpawner.enabled = false;
        }

        if (seatTracker != null)
        {
            seatTracker.canAcceptPassengers = false;
        }

        CheckForGarageReturn();
    }

    public void CheckForGarageReturn()
    {
        if (isNight && !garageColliderSpawned && seatTracker != null && seatTracker.IsEmpty())
        {
            garageColliderSpawned = true;
            Debug.Log("Last passenger dropped off! Heading back to the garage.");

            if (garageReturnCollider != null)
            {
                if (garageSpawnPoint != null)
                {
                    garageReturnCollider.transform.position = garageSpawnPoint.position;
                }
                garageReturnCollider.SetActive(true);
            }
        }
    }
}