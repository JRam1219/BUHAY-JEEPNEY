using UnityEngine;
using UnityEngine.SceneManagement;

public class GarageExitTrigger : MonoBehaviour
{
    public GarageDeskEndInteractable desk;
    public DialogueLine[] reminderDialogue;
    public string nextSceneName = "Store_SQ1";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name != "Jeff") return;

        if (desk != null && !desk.hasSignedOut)
        {
            if (DialogueManager.Instance != null && reminderDialogue.Length > 0)
            {
                DialogueManager.Instance.StartDialogue(reminderDialogue);
            }
            return;
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}