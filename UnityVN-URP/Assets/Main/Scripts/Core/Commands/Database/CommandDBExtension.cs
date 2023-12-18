/// <summary>
/// Class that adds more flexibility to add custom commands (or methods) to the command database.
/// </summary>
public abstract class CommandDBExtension
{
    /// <summary>
    /// Adds the commands to database.
    /// </summary>
    /// <param name="database">The command database.</param>
    public static void Extend(CommandDB database) { 
    }

    /// <summary>
    /// Extracts the parameters and their associated values of a command. EX: Command(param1 -50), the extractor will get the 50.
    /// </summary>
    /// <param name="data">The parameters passed by a command.</param>
    /// <returns>The Parameters parsed.</returns>
    public static CommandParameters ConvertToParams(string[] data) => new CommandParameters(data);
}
