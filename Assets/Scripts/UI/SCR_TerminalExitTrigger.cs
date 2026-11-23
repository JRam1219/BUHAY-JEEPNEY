using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_TerminalExitTrigger : MonoBehaviour
{
    public string endlessRoadSceneName = "SCN_Sq5_Scn4";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Find the seat tracker on the jeepney and save boarded passengers
            SCR_JeepneySeatTracker seatTracker = other.GetComponentInParent<SCR_JeepneySeatTracker>();
            if (seatTracker != null)
            {
                seatTracker.SavePassengersForTransfer();
            }

            // Load the Endless Road scene
            SceneManager.LoadScene(endlessRoadSceneName);
        }
    }
}