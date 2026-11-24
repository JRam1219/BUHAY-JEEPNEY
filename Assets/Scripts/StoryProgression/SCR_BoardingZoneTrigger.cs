using UnityEngine;

public class SCR_BoardingZoneTrigger : MonoBehaviour
{
    public SCR_TerminalManager terminalManager;
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            if (terminalManager != null)
            {
                terminalManager.StartBoarding();
            }
        }
    }
}