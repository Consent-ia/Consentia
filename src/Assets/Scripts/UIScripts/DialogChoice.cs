using System;

[Serializable]
public class DialogChoice
{
    public string choiceText;
    public int nextDialogIndex;

    public DialogChoice(string text, int nextIndex = -1)
    {
        choiceText = text;
        nextDialogIndex = nextIndex;
    }
}
