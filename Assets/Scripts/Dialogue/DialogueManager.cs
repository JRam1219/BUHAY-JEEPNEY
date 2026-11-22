using System.Collections;
using UnityEngine;
using TMPro;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    [TextArea(2, 3)]
    public string text;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;
    public GameObject continueIndicator; // Optional: little "▼" icon
    public float typingSpeed = 0.03f;

    public bool isDialogueActive { get; private set; }

    private DialogueLine[] currentLines;
    private int currentIndex;
    private bool isTyping;
    private Coroutine typingCoroutine;

    void Awake()
    {
        Instance = this;
        dialoguePanel.SetActive(false);
        if (continueIndicator != null) continueIndicator.SetActive(false);
    }

    void Update()
    {
        if (!isDialogueActive) return;

        // Left-click, Space, or E to advance dialogue
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = currentLines[currentIndex].text;
                isTyping = false;
                if (continueIndicator != null) continueIndicator.SetActive(true);
            }
            else
            {
                NextLine();
            }
        }
    }

    public void StartDialogue(DialogueLine[] lines)
    {
        currentLines = lines;
        currentIndex = 0;
        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        ShowLine();
    }

    void ShowLine()
    {
        if (continueIndicator != null) continueIndicator.SetActive(false);
        speakerNameText.text = currentLines[currentIndex].speakerName;
        typingCoroutine = StartCoroutine(TypeLine(currentLines[currentIndex].text));
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
        if (continueIndicator != null) continueIndicator.SetActive(true);
    }

    void NextLine()
    {
        currentIndex++;
        if (currentIndex < currentLines.Length)
        {
            ShowLine();
        }
        else
        {
            isDialogueActive = false;
            dialoguePanel.SetActive(false);
            if (continueIndicator != null) continueIndicator.SetActive(false);
        }
    }
}