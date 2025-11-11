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

    private string[] currentDialogLines;
    private string currentNPCName;
    private int currentLineIndex = 0;
    private bool isTyping = false;

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
    }

    public void StartDialog(string npcName, string[] dialogLines)
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
        // Don't advance if still typing
        if (isTyping)
        {
            // Skip typing animation and show full text immediately
            StopAllCoroutines();
            dialogText.text = currentDialogLines[currentLineIndex - 1];
            isTyping = false;
            return;
        }

        if (currentLineIndex < currentDialogLines.Length)
        {
            StartCoroutine(TypeDialog(currentDialogLines[currentLineIndex]));
            currentLineIndex++;
        }
        else
        {
            EndDialog();
        }
    }

    public void EndDialog()
    {
        dialogBox.SetActive(false);
        currentDialogLines = null;
        currentLineIndex = 0;
        isTyping = false;
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

    public bool IsDialogActive()
    {
        return dialogBox.activeSelf;
    }

    public bool IsTyping()
    {
        return isTyping;
    }
}