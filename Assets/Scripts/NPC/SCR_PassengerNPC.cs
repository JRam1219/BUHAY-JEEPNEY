using UnityEngine;

public class SCR_PassengerNPC : MonoBehaviour
{
    [Header("Generated Passenger Data")]
    public string destination;
    public int fare;
    public float distanceRemaining;

    [Header("Movement")]
    public float moveSpeed = 2f;
    private Transform entranceTarget;
    public bool hasBoarded { get; private set; } = false;
    public bool wantsToAlight { get; set; } = false;

    public void Initialize(string assignedDestination, int assignedFare, float travelDistance, Transform jeepneyEntrance)
    {
        destination = assignedDestination;
        fare = assignedFare;
        distanceRemaining = travelDistance;
        entranceTarget = jeepneyEntrance;
    }

    public void SetEntranceTarget(Transform target)
    {
        entranceTarget = target;
    }

    private void Update()
    {
        if (hasBoarded || entranceTarget == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            entranceTarget.position,
            moveSpeed * Time.deltaTime
        );
    }

    public void AssignSeat(Transform seatTransform)
    {
        hasBoarded = true;
        transform.SetParent(seatTransform, true);
        transform.position = seatTransform.position;

        if (TryGetComponent(out Collider2D col))
        {
            col.enabled = false;
        }

        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.enabled = false;
        }
    }

    public void LeaveJeepney(Transform exitPoint)
    {
        transform.SetParent(null);
        if (exitPoint != null)
        {
            transform.position = exitPoint.position;
        }

        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.enabled = true;
        }

        Destroy(gameObject, 0.5f);
    }
}