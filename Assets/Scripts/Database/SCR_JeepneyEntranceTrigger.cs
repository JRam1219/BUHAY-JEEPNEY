using UnityEngine;

public class SCR_JeepneyEntranceTrigger : MonoBehaviour
{
    public SCR_JeepneySeatTracker seatTracker;

    private void Awake()
    {
        if (seatTracker == null)
        {
            seatTracker = GetComponentInParent<SCR_JeepneySeatTracker>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (seatTracker == null) return;

        if (other.TryGetComponent(out SCR_PassengerNPC npc) && !npc.hasBoarded)
        {
            seatTracker.TryAssignSeat(npc, npc.destination, npc.fare);
        }
    }
}