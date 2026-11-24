
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneExitTrigger : MonoBehaviour
{
    public string nextSceneName = "Driving_SQ1";
    public GasAttendantInteractable gasAttendant;

    private bool jeepInside = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<JeepMovement>() != null)
        {
            jeepInside = true;
            TryExit();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.GetComponent<JeepMovement>() != null)
        {
            jeepInside = true;
            TryExit();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<JeepMovement>() != null)
        {
            jeepInside = false;
        }
    }

    private void TryExit()
    {
        if (!jeepInside) return;

        if (gasAttendant != null && !gasAttendant.hasRefueled)
            return;

        SceneManager.LoadScene(nextSceneName);
    }
}
