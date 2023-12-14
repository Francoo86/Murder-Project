using System.Collections.Generic;

/// <summary>
/// Contenedor de datos de una conversación incluyendo sus lineas y su progreso.
/// </summary>
public class Conversation
{
    private List<string> lines = new List<string>();
    private int progress = 0;

    public Conversation(List<string> lines, int progress = 0)
    {
        this.lines = lines;
        this.progress = progress;
    }

    public int GetProgress() => progress;
    public void SetProgress(int progress) => this.progress = progress;
    public void IncrementProgress() => progress++;
    public int Count => lines.Count;
    public List<string> GetLines() => lines;
    public string GetCurrentLine() => lines[progress];
    public bool HasReachedEnd() => progress >= lines.Count;

}
