using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TeleportScene : MonoBehaviour, INPC
{
    [SerializeField] 
    private NPCDialog npcDialog;
    private ChangeScene changeScene;

    private bool isWaitingForDialogEnd = false;

    private void Awake()
    {
        changeScene = GetComponent<ChangeScene>();
    }

    public void Interact()
    {
        if (npcDialog.DialogLines.Length > 0 && DialogManager.Instance != null)
        {
            Debug.Log($"{npcDialog.NPCName}: Starting dialog");

            DialogManager.Instance.StartDialog(npcDialog.NPCName, npcDialog.DialogLines);

            // Start waiting for dialog to end
            isWaitingForDialogEnd = true;
        }
    }

    private void Update()
    {
        // Only check when actively waiting for dialog to end
        if (!isWaitingForDialogEnd)
            return;

        // Trigger scene change only when dialog has ended
        if (DialogManager.Instance != null && !DialogManager.Instance.IsDialogActive())
        {
            isWaitingForDialogEnd = false;
            changeScene.Change();
        }
    }
}
