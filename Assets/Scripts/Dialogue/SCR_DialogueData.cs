using UnityEngine;

// Determines which side the speaker's nameplate appears on.
public enum SpeakerSide
{
    Left,
    Right
}

// Stores the information for one dialogue line.
[System.Serializable]
public class SCR_DialogueLine
{
    [Header("Character Information")]

    // Name of the character currently speaking.
    public string characterName;

    // Determines whether the nameplate appears on the left or right.
    public SpeakerSide speakerSide;

    [Header("Dialogue Text")]

    // The actual dialogue sentence.
    [TextArea(2, 5)]
    public string dialogueText;

    [Header("Character Portraits")]

    // Portrait displayed on the left side.
    public Sprite leftCharacterPortrait;

    // Portrait displayed on the right side.
    public Sprite rightCharacterPortrait;

    [Header("Individual Name Text Settings")]

    // Controls this speaker's individual name font size.
    public float nameFontSize = 24f;

    // Controls this speaker's horizontal name text position.
    public float nameTextOffsetX = 0f;

    // Controls this speaker's vertical name text position.
    public float nameTextOffsetY = 0f;
}


// Stores the complete conversation as a ScriptableObject.
[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data")]
public class SCR_DialogueData: ScriptableObject
{
    [Header("Dialogue Information")]

    // Unique identifiers for tracking dialogue data.
    [SerializeField] private string dialogueID;
    [SerializeField] private string npcID;

    [Header("Dialogue Lines")]

    // All dialogue lines belonging to this conversation.
    [SerializeField] private SCR_DialogueLine[] dialogueLines;

    // Allows other scripts to access the dialogue information.
    public float nameFontSize = 36f;
    public string DialogueID => dialogueID;
    public string NpcId => npcID;
    public SCR_DialogueLine[] DialogueLines => dialogueLines;
}