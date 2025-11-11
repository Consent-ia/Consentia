using UnityEngine;

public class NPCController : MonoBehaviour, INPC
{
    [SerializeField]
    private string npcName = "NPC";
    
    [SerializeField]
    private string[] dialogueLines = new string[]
    {
        "Hello, traveler!",
        "Welcome to our village.",
        "Is there anything I can help you with?"
    };
    
    private int currentDialogueIndex = 0;
    
    public void Interact()
    {
        // Display dialogue or trigger NPC behavior
        if (dialogueLines.Length > 0)
        {
            Debug.Log($"{npcName}: {dialogueLines[currentDialogueIndex]}");
            
            // Cycle through dialogue
            currentDialogueIndex = (currentDialogueIndex + 1) % dialogueLines.Length;
        }
        
        // Add your dialogue system or NPC interaction logic here
    }
}