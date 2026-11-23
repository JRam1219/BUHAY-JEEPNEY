
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;

    [TextArea(2, 3)]
    public string text;

    public bool showChoices;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("Dialogue UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;
    public GameObject continueIndicator;
    public float typingSpeed = 0.03f;

    [Header("Choice Buttons")]
    public Button yesButton;
    public Button noButton;

    [Header("Choice Responses")]
    public DialogueLine[] yesResponseLines;
    public DialogueLine[] noResponseLines;

    public bool isDialogueActive { get; private set; }

    private DialogueLine[] currentLines;
    private int currentIndex;
    private bool isTyping;
    private bool isChoiceActive;
    private Coroutine typingCoroutine;

    void Awake()
    {
        Instance = this;

        dialoguePanel.SetActive(false);

        if (continueIndicator != null)
            continueIndicator.SetActive(false);

        if (yesButton != null)
        {
            yesButton.gameObject.SetActive(false);
            yesButton.onClick.AddListener(ChooseYes);
        }

        if (noButton != null)
        {
            noButton.gameObject.SetActive(false);
            noButton.onClick.AddListener(ChooseNo);
        }
    }

    void Update()
    {
        if (!isDialogueActive || isChoiceActive)
            return;

        if (Input.GetMouseButtonDown(0) ||
            Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = currentLines[currentIndex].text;
                isTyping = false;

                if (currentLines[currentIndex].showChoices)
                    ShowChoices();
                else if (continueIndicator != null)
                    continueIndicator.SetActive(true);
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
        isChoiceActive = false;

        dialoguePanel.SetActive(true);

        HideChoices();
        ShowLine();
    }

    void ShowLine()
    {
        if (continueIndicator != null)
            continueIndicator.SetActive(false);

        HideChoices();

        speakerNameText.text = currentLines[currentIndex].speakerName;

        typingCoroutine = StartCoroutine(
            TypeLine(currentLines[currentIndex].text)
        );
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

        if (currentLines[currentIndex].showChoices)
            ShowChoices();
        else if (continueIndicator != null)
            continueIndicator.SetActive(true);
    }

    void ShowChoices()
    {
        isChoiceActive = true;

        if (continueIndicator != null)
            continueIndicator.SetActive(false);

        if (yesButton != null)
            yesButton.gameObject.SetActive(true);

        if (noButton != null)
            noButton.gameObject.SetActive(true);
    }

    void HideChoices()
    {
        isChoiceActive = false;

        if (yesButton != null)
            yesButton.gameObject.SetActive(false);

        if (noButton != null)
            noButton.gameObject.SetActive(false);
    }

    public void ChooseYes()
    {
        SelectResponse(yesResponseLines);
    }

    public void ChooseNo()
    {
        SelectResponse(noResponseLines);
    }

    void SelectResponse(DialogueLine[] responseLines)
    {
        HideChoices();

        if (responseLines == null || responseLines.Length == 0)
        {
            EndDialogue();
            return;
        }

        currentLines = responseLines;
        currentIndex = 0;

        ShowLine();
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
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        isTyping = false;

        HideChoices();

        if (continueIndicator != null)
            continueIndicator.SetActive(false);

        dialoguePanel.SetActive(false);
    }
}
