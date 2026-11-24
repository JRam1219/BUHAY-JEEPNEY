
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoadEndTrigger : MonoBehaviour
{
    public DialogueLine[] endOfRouteDialogue;
    public DialogueLine[] insufficientMoneyDialogue;
    public string nextSceneName = "";

    private bool triggered = false;
    private bool waitingForDialogue = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || other.GetComponent<JeepMovement>() == null)
            return;

        if (MoneyManager.Instance == null)
        {
            Debug.LogError("MoneyManager is missing!");
            return;
        }

        if (!MoneyManager.Instance.HasReachedTarget())
        {
            if (DialogueManager.Instance != null &&
                insufficientMoneyDialogue != null &&
                insufficientMoneyDialogue.Length > 0)
            {
                DialogueManager.Instance.StartDialogue(insufficientMoneyDialogue);
            }

            return;
        }

        triggered = true;

        if (DialogueManager.Instance != null &&
            endOfRouteDialogue != null &&
            endOfRouteDialogue.Length > 0)
        {
            DialogueManager.Instance.StartDialogue(endOfRouteDialogue);
            waitingForDialogue = true;
        }
        else
        {
            LoadNextScene();
        }
    }

    void Update()
    {
        if (waitingForDialogue &&
            DialogueManager.Instance != null &&
            !DialogueManager.Instance.isDialogueActive)
        {
            waitingForDialogue = false;
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
