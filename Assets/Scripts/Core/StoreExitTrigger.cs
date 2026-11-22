using UnityEngine;
using UnityEngine.SceneManagement;

public class StoreExitTrigger : MonoBehaviour
{
    public DialogueLine[] reminderDialogue;
    public string nextSceneName = ""; // Put your next scene name here when ready

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name != "Jeff") return;

        if (StoreMoneyManager.Instance != null && !StoreMoneyManager.Instance.HasBoughtAll())
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
        else
        {
            Debug.Log("Jeff finished grocery shopping with ₱100 left!");
        }
    }
}