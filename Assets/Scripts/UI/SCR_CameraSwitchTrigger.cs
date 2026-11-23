using UnityEngine;

public class SCR_CameraSwitchTrigger : MonoBehaviour
{
    [Header("Cameras to Switch")]
    public GameObject gasStationCamera;
    public GameObject terminalCamera;

    private bool hasSwitched = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasSwitched) return;

        if (other.CompareTag("Player") ||
            other.GetComponentInParent<SCR_PlayerControl>() != null ||
            other.GetComponentInParent<SCR_JeepneyDriveAnim>() != null)
        {
            hasSwitched = true;

            if (gasStationCamera != null)
                gasStationCamera.SetActive(false);

            if (terminalCamera != null)
                terminalCamera.SetActive(true);
        }
    }
}