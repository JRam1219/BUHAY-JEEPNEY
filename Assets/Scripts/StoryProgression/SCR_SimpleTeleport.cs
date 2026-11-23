using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_SimpleTeleport : MonoBehaviour
{
    [Header("Option A: Teleport within the SAME scene")]
    public Transform destinationPoint;

    [Header("Option B: Load a DIFFERENT scene (leave blank if using Option A)")]
    public string targetSceneName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Checks if the object hitting the collider is tagged "Player"
        if (other.CompareTag("Player"))
        {
            if (!string.IsNullOrEmpty(targetSceneName))
            {
                SceneManager.LoadScene(targetSceneName);
            }
            else if (destinationPoint != null)
            {
                other.transform.position = destinationPoint.position;
            }
        }
    }
}   