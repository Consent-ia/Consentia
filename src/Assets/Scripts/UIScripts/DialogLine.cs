using System;

[Serializable]
public class DialogLine
{
    public string text;
    public bool isQuestion;
    public DialogChoice[] choices;

    public DialogLine(string dialogText)
    {
        text = dialogText;
        isQuestion = false;
        choices = null;
    }

    public DialogLine(string questionText, DialogChoice[] dialogChoices)
    {
        text = questionText;
        isQuestion = true;
        choices = dialogChoices;
    }
}
