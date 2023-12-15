
public class ChoicePanelDecision
{
    public string question = string.Empty;
    public int answerIndex = -1;
    public string[] choices = new string[0];

    public ChoicePanelDecision(string question, string[] choices)
    {
        this.question = question;
        this.choices = choices;
        answerIndex = -1;
    }   
}
