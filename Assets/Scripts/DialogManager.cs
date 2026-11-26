using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static System.Collections.Specialized.BitVector32;

public class DialogManager : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogFrame;
    [SerializeField] private TMP_Text dialogText;
    [SerializeField] private Button nextButton;

    [Header("Dialogue Lines")]
    [SerializeField] public string[] mayorsTrueColor;
    [SerializeField] private string[] jeffTheSaviour;
    [SerializeField] private string[] meetMarielle;
    public string[] dialogueLines;
    public int currentLine = 0;
    public bool isDialogActive = false;

    private void Start()
    {
        dialogFrame.SetActive(false);
        nextButton.onClick.AddListener(NextDialogue);
    }

    public void StartDialogue(int section)
    {

        switch (section)
        {
            case 0:
                dialogueLines = mayorsTrueColor;
                break;

            case 1:
                dialogueLines = jeffTheSaviour;
                break;

            case 2:
                dialogueLines = new string[] { "You both got shot, try again!" };
                break;
            case 3:
                dialogueLines = new string[] { "Mission complete!", "Now, find the evidence against the mayor!" };
                break;
            case 4:
                dialogueLines = meetMarielle;
                break;
            default:
                Debug.LogWarning("Invalid dialogue section!");
                return;
        }

        Debug.Log("Starting dialogue section: " + section);

        currentLine = 0;
        isDialogActive = true;
        Debug.Log("lolomo paroparo");
        dialogFrame.SetActive(true);
        dialogText.text = dialogueLines[currentLine];
    }

    public void NextDialogue()
    {
        if (!isDialogActive)
            return;

        currentLine++;

        if (currentLine < dialogueLines.Length)
        {
            dialogText.text = dialogueLines[currentLine];
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        isDialogActive = false;
        dialogFrame.SetActive(false);
    }
}