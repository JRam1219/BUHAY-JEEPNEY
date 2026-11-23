using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class SCR_DialogueManager : MonoBehaviour
{
    [Header("Dialogue Settings")]

    [SerializeField] private SCR_DialogueData dialogueToPlay;
    [SerializeField] private float typingSpeed = 0.04f;


    [Header("Dialogue UI References")]

    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private RectTransform nameplateRect;
    [SerializeField] private TMP_Text npcNameText;
    [SerializeField] private TMP_Text dialogueText;


    [Header("Character Portraits")]

    [SerializeField] private Image leftCharacterImage;
    [SerializeField] private Image rightCharacterImage;


    // Dialogue states
    private SCR_DialogueData currentDialogue;
    private int currentLineIndex;

    private bool isDialogueActive;
    public bool IsDialogueActive => isDialogueActive;
    private bool isTyping;

    private Coroutine typingCoroutine;


    // INITIALIZATION
    private void Awake()
    {
        if (dialogueBox != null)
        dialogueBox.SetActive(false);
    }


    private void Start()
    {
        if (dialogueToPlay != null)
        {
            StartDialogue(dialogueToPlay);
        }
        else
        {
            Debug.LogWarning("No Dialogue Data assigned!");
        }
    }


    // INPUT HANDLING
    private void Update()
    {
        if (!isDialogueActive)
        return;

        if (Keyboard.current == null)
        return;

        bool advancePressed =
        Keyboard.current.spaceKey.wasPressedThisFrame ||
        Keyboard.current.enterKey.wasPressedThisFrame;

        if (advancePressed)
        {
            Debug.Log("Dialogue input detected!");

            if (isTyping)
            {
                // First press: finish typing immediately.
                ShowFullLine();
            }
            else
            {
                // Second press: advance dialogue.
                NextLine();
            }
        }
    }


    // START DIALOGUE
    public void StartDialogue(SCR_DialogueData dialogueData)
    {
        if (dialogueData == null ||
        dialogueData.DialogueLines == null ||
        dialogueData.DialogueLines.Length == 0)
        {
            Debug.LogWarning("Dialogue data is missing or empty.");
            return;
        }

        // Stop any previous typing coroutine.
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        currentDialogue = dialogueData;
        currentLineIndex = 0;

        isDialogueActive = true;
        isTyping = false;

        if (dialogueBox != null)
        dialogueBox.SetActive(true);

        DisplayCurrentLine();
    }

    // DISPLAY CURRENT DIALOGUE LINE
    private void DisplayCurrentLine()
    {
        if (currentDialogue == null)
        return;

        if (currentLineIndex >= currentDialogue.DialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        SCR_DialogueLine line =
        currentDialogue.DialogueLines[currentLineIndex];

        // SPEAKER NAME
        if (npcNameText != null)
        {
            npcNameText.text = line.characterName;
            npcNameText.fontSize = line.nameFontSize;
            if (line.nameFontSize > 0)
            {
                npcNameText.fontSize = line.nameFontSize;
            }

            RectTransform nameTextRect =
            npcNameText.GetComponent<RectTransform>();

            if (nameTextRect != null)
            {
                Vector2 textPosition =
                nameTextRect.anchoredPosition;

                textPosition.x = line.nameTextOffsetX;
                textPosition.y = line.nameTextOffsetY;

                nameTextRect.anchoredPosition = textPosition;
            }
        }


        // NAMEPLATE POSITION
        if (nameplateRect != null)
        {
            float anchorX =
            line.speakerSide == SpeakerSide.Left ? 0f : 1f;

            Vector2 minAnchor = nameplateRect.anchorMin;
            Vector2 maxAnchor = nameplateRect.anchorMax;

            minAnchor.x = anchorX;
            maxAnchor.x = anchorX;

            nameplateRect.anchorMin = minAnchor;
            nameplateRect.anchorMax = maxAnchor;

            Vector2 pivot = nameplateRect.pivot;
            pivot.x = anchorX;

            nameplateRect.pivot = pivot;

            Vector2 position =
            nameplateRect.anchoredPosition;

            position.x =
            line.speakerSide == SpeakerSide.Left ? 350f : -350f;

            nameplateRect.anchoredPosition = position;
        }

        // DIALOGUE TEXT
        if (dialogueText != null)
        {
            dialogueText.text = line.dialogueText;

            dialogueText.maxVisibleCharacters = 0;

            dialogueText.ForceMeshUpdate();

            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            typingCoroutine = StartCoroutine(TypeDialogue());
        }


        // LEFT PORTRAIT
        if (leftCharacterImage != null)
        {
            leftCharacterImage.sprite =
            line.leftCharacterPortrait;

            leftCharacterImage.gameObject.SetActive(
            line.leftCharacterPortrait != null
            );
        }


            // RIGHT PORTRAIT
        if (rightCharacterImage != null)
        {
            rightCharacterImage.sprite =
            line.rightCharacterPortrait;

            rightCharacterImage.gameObject.SetActive(
            line.rightCharacterPortrait != null
            );
        }
    }


    // TYPEWRITER ANIMATION
    private IEnumerator TypeDialogue()
    {
        isTyping = true;

        dialogueText.ForceMeshUpdate();

        int totalCharacters =
        dialogueText.textInfo.characterCount;

        dialogueText.maxVisibleCharacters = 0;

        for (int i = 0; i < totalCharacters; i++)
        {
            if (!isDialogueActive)
            yield break;

            dialogueText.maxVisibleCharacters = i + 1;

            // Uses real time, even when the game is paused.
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        // Reveal the complete sentence.
        dialogueText.maxVisibleCharacters = totalCharacters;

        isTyping = false;
        typingCoroutine = null;
    }


        // SKIP TYPING ANIMATION
    private void ShowFullLine()
    {
        if (dialogueText == null)
        return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialogueText.ForceMeshUpdate();

        dialogueText.maxVisibleCharacters =
        dialogueText.textInfo.characterCount;

        isTyping = false;
    }


    // ADVANCE TO NEXT LINE
    public void NextLine()
    {
        if (!isDialogueActive || currentDialogue == null)
        return;

        if (isTyping)
        {
            ShowFullLine();
            return;
        }

        currentLineIndex++;

        if (currentLineIndex <
        currentDialogue.DialogueLines.Length)
        {
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }


    // END DIALOGUE
    public void EndDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isTyping = false;
        isDialogueActive = false;

        currentDialogue = null;

        if (dialogueBox != null)
        dialogueBox.SetActive(false);
    }
}