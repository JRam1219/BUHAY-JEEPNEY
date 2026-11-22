using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string nextSceneName;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name.Trim() == "Jeff")
        {
            if (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive)
                return;

            SceneManager.LoadScene(nextSceneName);
        }
    }
}