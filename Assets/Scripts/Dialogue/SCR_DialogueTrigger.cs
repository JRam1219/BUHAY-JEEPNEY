using UnityEngine;

public class SCR_DialogueTrigger : MonoBehaviour, SCR_Interactable
{
    [Header("Dialogue References")]
    [SerializeField] private SCR_DialogueManager dialogueManager;
    [SerializeField] private SCR_DialogueData dialogueData;

    [Header("Optional Location Transition After Dialogue")]
    [SerializeField] private bool switchLocationAfterDialogue = false;
    [SerializeField] private SCR_SceneIntroFade sceneFadeController;
    [SerializeField] private GameObject currentCamera;
    [SerializeField] private GameObject targetCamera;
    [SerializeField] private CharacterTeleport[] charactersToMove;

    public void Interact()
    {
        if (dialogueManager == null || dialogueData == null)
        {
            Debug.LogWarning("Dialogue Manager or Dialogue Data is missing.", this);
            return;
        }

        if (!dialogueManager.IsDialogueActive)
        {
            dialogueManager.StartDialogue(dialogueData);

            if (switchLocationAfterDialogue && sceneFadeController != null)
            {
                sceneFadeController.FadeAndSwitchCameraAfterDialogue(
                    currentCamera,
                    targetCamera);
            }
        }

        if (switchLocationAfterDialogue && sceneFadeController != null)
            {
                sceneFadeController.FadeAndSwitchCameraAfterDialogue(
                    currentCamera,
                    targetCamera,
                    charactersToMove);
            }
    }
}