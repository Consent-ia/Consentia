using System.Collections;
using UnityEngine;
using TMPro;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    [SerializeField]
    private GameObject dialogBox;

    [SerializeField]
    private TextMeshProUGUI npcNameText;

    [SerializeField]
    private TextMeshProUGUI dialogText;

    [SerializeField]
    [Range(0.01f, 0.1f)]
    private float typeSpeed = 0.02f;

    [Header("Choice Box")]
    [SerializeField]
    private GameObject choiceBox;

    [SerializeField]
    private TextMeshProUGUI[] choiceTexts = new TextMeshProUGUI[4];

    [SerializeField]
    private GameObject selectionArrow;

    private DialogLine[] currentDialogLines;
    private string currentNPCName;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool isWaitingForChoice = false;
    private int currentChoiceIndex = 0;
    private int currentlyTypingIndex = -1;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Hide dialog box at start
        dialogBox.SetActive(false);
        choiceBox.SetActive(false);
    }

    public void StartDialog(string npcName, DialogLine[] dialogLines)
    {
        currentNPCName = npcName;
        currentDialogLines = dialogLines;
        currentLineIndex = 0;

        // Show dialog box
        dialogBox.SetActive(true);

        // Display NPC name
        if (npcNameText != null)
        {
            npcNameText.text = currentNPCName;
        }

        // Display first line
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            DialogLine skippedLine = currentDialogLines[currentlyTypingIndex];
            dialogText.text = skippedLine.text;
            isTyping = false;
            
            // If we skipped a question, show the choices now
            if (skippedLine.isQuestion)
            {
                ShowChoices(skippedLine);
                return;
            }
            
            // Check if we're at the end after showing full text
            if (currentLineIndex >= currentDialogLines.Length)
            {
                // Don't end immediately - wait for next Interact press
                return;
            }
            return;
        }

        if (currentLineIndex < currentDialogLines.Length)
        {
            DialogLine currentLine = currentDialogLines[currentLineIndex];
            currentlyTypingIndex = currentLineIndex;

            if (currentLine.isQuestion)
            {
                StartCoroutine(TypeDialogThenShowChoices(currentLine));
            }
            else
            {
                StartCoroutine(TypeDialog(currentLine.text));
            }

            currentLineIndex++;
        }
        else
        {
            EndDialog();
        }
    }

    private void ShowChoices(DialogLine questionLine)
    {
        if (questionLine.choices == null || questionLine.choices.Length == 0)
        {
            Debug.LogWarning("Question has no choices!");
            return;
        }

        choiceBox.SetActive(true);
        isWaitingForChoice = true;
        currentChoiceIndex = 0;

        for (int i = 0; i < choiceTexts.Length; i++)
        {
            if (i < questionLine.choices.Length)
            {
                choiceTexts[i].text = questionLine.choices[i].choiceText;
                choiceTexts[i].gameObject.SetActive(true);
            }
            else
            {
                choiceTexts[i].gameObject.SetActive(false);
            }
        }

        UpdateSelectionArrow();
    }

    private void HideChoices()
    {
        choiceBox.SetActive(false);
        isWaitingForChoice = false;
    }

    private void UpdateSelectionArrow()
    {
        if (selectionArrow != null && choiceTexts.Length > 0)
        {
            RectTransform arrowRect = selectionArrow.GetComponent<RectTransform>();
            RectTransform choiceRect = choiceTexts[currentChoiceIndex].GetComponent<RectTransform>();
            
            arrowRect.anchoredPosition = new Vector2(arrowRect.anchoredPosition.x, choiceRect.anchoredPosition.y);
        }
    }

    // Called from PlayerInteract via new Input System
    public void NavigateChoices(Vector2 direction)
    {
        if (!isWaitingForChoice)
            return;

        DialogLine currentQuestion = currentDialogLines[currentLineIndex - 1];
        int maxChoices = currentQuestion.choices.Length;

        if (direction.y > 0) // Up
        {
            currentChoiceIndex--;
            if (currentChoiceIndex < 0)
                currentChoiceIndex = maxChoices - 1;
            UpdateSelectionArrow();
        }
        else if (direction.y < 0) // Down
        {
            currentChoiceIndex++;
            if (currentChoiceIndex >= maxChoices)
                currentChoiceIndex = 0;
            UpdateSelectionArrow();
        }
    }

    private IEnumerator TypeDialogThenShowChoices(DialogLine questionLine)
    {
        yield return StartCoroutine(TypeDialog(questionLine.text));
        ShowChoices(questionLine);
    }

    public void EndDialog()
    {
        dialogBox.SetActive(false);
        choiceBox.SetActive(false);
        currentDialogLines = null;
        currentLineIndex = 0;
        currentlyTypingIndex = -1;
        isTyping = false;
        isWaitingForChoice = false;
    }

    private IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
    }

    public void SelectChoice()
    {
        if (!isWaitingForChoice)
            return;

        DialogLine currentQuestion = currentDialogLines[currentLineIndex - 1];
        DialogChoice selectedChoice = currentQuestion.choices[currentChoiceIndex];

        Debug.Log($"Selected: {selectedChoice.choiceText}");

        // Save the choice
        SaveChoice(currentQuestion, selectedChoice);

        HideChoices();

        if (selectedChoice.nextDialogIndex >= 0 && selectedChoice.nextDialogIndex < currentDialogLines.Length)
        {
            currentLineIndex = selectedChoice.nextDialogIndex;
            DisplayNextLine();
            
            // After displaying the response, set index to end so next press ends dialog
            currentLineIndex = currentDialogLines.Length;
        }
        else
        {
            EndDialog();
        }
    }

    private void SaveChoice(DialogLine question, DialogChoice selectedChoice)
    {
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.SaveChoice(
                currentNPCName, 
                question.text, 
                selectedChoice.choiceText, 
                currentChoiceIndex
            );
        }
        else
        {
            Debug.LogWarning("SaveSystem not found! Choice not saved.");
        }
    }

    public bool IsDialogActive()
    {
        return dialogBox.activeSelf;
    }

    public bool IsTyping()
    {
        return isTyping;
    }

    public bool IsWaitingForChoice()
    {
        return isWaitingForChoice;
    }
}