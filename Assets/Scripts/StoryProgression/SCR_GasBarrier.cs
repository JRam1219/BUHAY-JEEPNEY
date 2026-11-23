using UnityEngine;

public class SCR_GasBarrier : MonoBehaviour
{
    [Header("Optional Reminder Dialogue")]
    public SCR_DialogueManager dialogueManager;
    public SCR_DialogueData reminderDialogue; // e.g., "Need to fill up gas first!"

    private float stopX;
    private bool hasShownReminder = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<SCR_JeepneyDriveAnim>() != null)
        {
            // Record the exact X position where the jeepney hit the barrier
            stopX = other.transform.position.x;

            if (!hasShownReminder && dialogueManager != null && reminderDialogue != null && !dialogueManager.IsDialogueActive)
            {
                hasShownReminder = true;
                dialogueManager.StartDialogue(reminderDialogue);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.GetComponent<SCR_JeepneyDriveAnim>() != null)
        {
            // Prevent the jeepney from moving further right
            if (other.transform.position.x > stopX)
            {
                Vector3 pos = other.transform.position;
                pos.x = stopX;
                other.transform.position = pos;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<SCR_JeepneyDriveAnim>() != null)
        {
            hasShownReminder = false;
        }
    }
}
