
/// <summary>
/// A decision that holds the question that will be asked to the player, and the choices that will be displayed.
/// </summary>
public class ChoicePanelDecision
{
    public string question = string.Empty;
    public int answerIndex = -1;
    public string[] choices = new string[0];

    /// <summary>
    /// Creates the Decision object.
    /// </summary>
    /// <param name="question">The title question.</param>
    /// <param name="choices">The choices.</param>
    public ChoicePanelDecision(string question, string[] choices)
    {
        this.question = question;
        this.choices = choices;
        answerIndex = -1;
    }   
}
