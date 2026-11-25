
using UnityEngine;
using UnityEngine.InputSystem;

public class BlueBookPickup : MonoBehaviour
{
    private bool playerInRange = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
    }

    private void Update()
    {
        if (!playerInRange || GameEvents.HasBook) return;

        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            CollectBook();
        }
    }

    private void CollectBook()
    {
        GameEvents.Stolen();

        gameObject.SetActive(false);

        Debug.Log("Jeff got the blue book!");
    }
}
