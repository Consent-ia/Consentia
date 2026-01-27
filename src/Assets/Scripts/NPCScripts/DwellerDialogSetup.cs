using UnityEngine;

[RequireComponent(typeof(NPCController))]
public class DwellerDialogSetup : MonoBehaviour
{
    void Start()
    {
        NPCController npc = GetComponent<NPCController>();
        
        // We'll set up dialog with a question
        DialogLine[] newDialog = new DialogLine[]
        {
            // Original dialogs
            new DialogLine("Hello, traveler!"),
            new DialogLine("Welcome to our village."),
            new DialogLine("Is there anything I can help you with?"),
            
            // New question dialog (index 3)
            new DialogLine("Pick a card!", new DialogChoice[]
            {
                new DialogChoice("1", 4),
                new DialogChoice("2", 5),
                new DialogChoice("3", 6),
                new DialogChoice("4", 7)
            }),
            
            // Response dialogs (indexes 4-7)
            // These are the LAST dialogs, so they'll naturally end
            new DialogLine("You picked card 1! Good choice."),
            new DialogLine("You picked card 2! Interesting..."),
            new DialogLine("You picked card 3! Bold move!"),
            new DialogLine("You picked card 4! Lucky number!")
        };
        
        // Use reflection to set the private dialogLines field
        var field = typeof(NPCController).GetField("dialogLines", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (field != null)
        {
            field.SetValue(npc, newDialog);
            Debug.Log("Dweller dialog updated successfully with branching!");
        }
        else
        {
            Debug.LogError("Could not find dialogLines field!");
        }
        
        // Destroy this setup script after running
        Destroy(this);
    }
}
