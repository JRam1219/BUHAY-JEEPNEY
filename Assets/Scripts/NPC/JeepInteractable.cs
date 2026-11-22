using UnityEngine;
using UnityEngine.SceneManagement;

public class JeepInteractable : MonoBehaviour
{
    public float interactRange = 2.5f;
    public DialogueLine[] conversationLines;
    public string nextSceneName = "GasStation_SQ1";

    private Transform player;
    private bool waitingForDialogueToEnd = false;

    void Start()
    {
        GameObject jeffObj = GameObject.Find("Jeff");
        if (jeffObj != null) player = jeffObj.transform;
    }

    void Update()
    {
        if (DialogueManager.Instance == null) return;

        // Once the jeep dialogue finishes, load the Gas Station
        if (waitingForDialogueToEnd)
        {
            if (!DialogueManager.Instance.isDialogueActive)
            {
                waitingForDialogueToEnd = false;
                SceneManager.LoadScene(nextSceneName);
            }
            return;
        }

        if (DialogueManager.Instance.isDialogueActive || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= interactRange && Input.GetKeyDown(KeyCode.E))
        {
            DialogueManager.Instance.StartDialogue(conversationLines);
            waitingForDialogueToEnd = true;
        }
    }
}