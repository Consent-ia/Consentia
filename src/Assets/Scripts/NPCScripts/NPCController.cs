using UnityEngine;

public class NPCController : MonoBehaviour, INPC
{
    [SerializeField]
    private string npcName = "NPC";
    
    [SerializeField]
    private string[] dialogLines = new string[]
    {
        "Hello, traveler!",
        "Welcome to our village.",
        "Is there anything I can help you with?"
    };
    
    private int currentDialogIndex = 0;
    
    public void Interact()
    {
        // Display dialog or trigger NPC behavior
        if (dialogLines.Length > 0 && DialogManager.Instance != null)
        {
            Debug.Log($"{npcName}: {dialogLines[currentDialogIndex]}");
            
            DialogManager.Instance.StartDialog(npcName, dialogLines);
        }
        
        // Add your dialog system or NPC interaction logic here
    }
}