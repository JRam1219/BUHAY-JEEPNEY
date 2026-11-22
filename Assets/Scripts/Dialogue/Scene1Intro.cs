using UnityEngine;

public class Scene1Intro : MonoBehaviour
{
    public DialogueLine[] tvNewsLines;

    void Start()
    {
        if (tvNewsLines.Length > 0)
        {
            DialogueManager.Instance.StartDialogue(tvNewsLines);
        }
    }
}