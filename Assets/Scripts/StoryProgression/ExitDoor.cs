using UnityEngine;

// Put on the way out of the house (probably LobbyBottom). Needs a Collider2D with "Is Trigger" checked.
public class ExitDoor : MonoBehaviour
{
    public CaughtManager manager;

    void Start()
    {
        if (!manager) manager = FindFirstObjectByType<CaughtManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameEvents.HasBook) manager.Win();
        else manager.Say("Not leaving without that notebook.", 2f);
    }
}