using UnityEngine;
using UnityEngine.SceneManagement;

public class RoadEndTrigger : MonoBehaviour
{
    public DialogueLine[] endOfRouteDialogue;
    public string nextSceneName = ""; // Leave blank until Scene 5 is ready

    private bool triggered = false;
    private bool waitingForDialogue = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || other.GetComponent<JeepMovement>() == null) return;

        triggered = true;
        if (DialogueManager.Instance != null && endOfRouteDialogue.Length > 0)
        {
            DialogueManager.Instance.StartDialogue(endOfRouteDialogue);
            waitingForDialogue = true;
        }
    }

    void Update()
    {
        if (waitingForDialogue && !DialogueManager.Instance.isDialogueActive)
        {
            waitingForDialogue = false;
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }
}