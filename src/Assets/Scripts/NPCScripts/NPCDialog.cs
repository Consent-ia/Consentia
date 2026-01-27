using UnityEngine;

[CreateAssetMenu(fileName = "NPCDialog", menuName = "NPCDialog")]
public class NPCDialog : ScriptableObject
{
    public string NPCName;
    public DialogLine[] DialogLines;

}
