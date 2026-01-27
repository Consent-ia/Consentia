using System;
using System.Collections.Generic;

[Serializable]
public class DialogChoiceData
{
    public string npcName;
    public string questionText;
    public string selectedChoice;
    public int choiceIndex;
    public string timestamp;

    public DialogChoiceData(string npc, string question, string choice, int index)
    {
        npcName = npc;
        questionText = question;
        selectedChoice = choice;
        choiceIndex = index;
        timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

[Serializable]
public class PlayerChoicesSaveData
{
    public List<DialogChoiceData> choices = new List<DialogChoiceData>();
}