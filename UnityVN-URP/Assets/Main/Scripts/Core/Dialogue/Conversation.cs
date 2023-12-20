using System.Collections.Generic;

/// <summary>
/// Conversation container that saves lines and indicates the progress (in which part we are).
/// </summary>
public class Conversation
{
    private List<string> lines = new List<string>();
    private int progress = 0;


    public string file { get; private set; }
    public int fileStartIndex { get; private set; }
    public int fileEndIndex { get; private set; }

    /// <summary>
    /// Creates a conversation object with the dialog lines and the progress.
    /// </summary>
    /// <param name="lines">Lines that the conversation will hold.</param>
    /// <param name="progress">In which part we should start, starts with 0 by default.</param>
    public Conversation(List<string> lines, int progress = 0, string file="", int fileStartIndex = -1, int fileEndIndex = -1)
    {
        this.lines = lines;
        this.progress = progress;
        this.file = file;

        if (fileStartIndex == -1)
            fileStartIndex = 0;

        if (fileEndIndex == -1)
            fileEndIndex = lines.Count - 1;

        this.fileStartIndex = fileStartIndex;
        this.fileEndIndex = fileEndIndex; 
    }

    /// <summary>
    /// Gets the actual progress index.
    /// </summary>
    /// <returns>The progress of conversation.</returns>
    public int GetProgress() => progress;
    /// <summary>
    /// Set the current progress of the conversation.
    /// </summary>
    /// <param name="progress">The new progress</param>
    public void SetProgress(int progress) => this.progress = progress;
    /// <summary>
    /// Increments progress by one.
    /// </summary>
    public void IncrementProgress() => progress++;
    public int Count => lines.Count;
    /// <summary>
    /// Gets the saved lines.
    /// </summary>
    /// <returns>Lines of the conversation.</returns>
    public List<string> GetLines() => lines;
    /// <summary>
    /// Gets the line where the progress is.
    /// </summary>
    /// <returns>The actual line.</returns>
    public string GetCurrentLine() => lines[progress];
    /// <summary>
    /// Tells if we have ended the conversation.
    /// </summary>
    /// <returns>Wether the conversation has ended.</returns>
    public bool HasReachedEnd() => progress >= lines.Count;

}
