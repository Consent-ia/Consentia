using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TeleportScene : MonoBehaviour, INPC
{
    [SerializeField] 
    private NPCDialog npcDialog;

    private bool isWaitingForDialogEnd;
    private bool hasInteractedWithArc;

    public void Interact()
    {
        if (npcDialog.DialogLines.Length <= 0 || !DialogManager.Instance) 
            return;
        Debug.Log($"{npcDialog.NPCName}: Starting dialog");

        DialogManager.Instance.StartDialog(npcDialog.NPCName, npcDialog.DialogLines, hasInteractedWithArc);

        // Start waiting for dialog to end
        isWaitingForDialogEnd = true;
        hasInteractedWithArc = true;

        GameObject exclamation = GameObject.FindGameObjectWithTag("EA");
        if (exclamation)
        {
            exclamation.SetActive(false);
        }
    }

    private void Update()
    {
        // Only check when actively waiting for dialog to end
        if (!isWaitingForDialogEnd)
            return;

        // Trigger scene change only when dialog has ended
        if (!DialogManager.Instance || DialogManager.Instance.IsDialogActive()) 
            return;
        
        isWaitingForDialogEnd = false;
        ProgressManager.Instance.PanCameraToPortal();
    }
}
