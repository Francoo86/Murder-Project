using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Static class that handles the file reading stuff.
/// </summary>
public class FileManager
{
    //TODO: Split into multiple classes.
    //En este caso podríamos usar el Factory Pattern para poder leer incluyendo archivos JSON.
    /// <summary>
    /// Reads a .txt file or a binary file with some format.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <param name="includeBlankLines">Include lines that are only empty spaces.</param>
    /// <returns>The list of lines that were read.</returns>
    public static List<string> ReadTextFile(string path, bool includeBlankLines = true) {
        if (!path.StartsWith("/")) path = FilePaths.rootPath + path;

        try
        {
            using (StreamReader sr = new StreamReader(path))
            {
                List<string> savedLines = new List<string>();
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    //Skip goofy aahh comments.
                    if (line.StartsWith("//")) continue;

                    if (includeBlankLines || !string.IsNullOrWhiteSpace(line)) {
                        savedLines.Add(line);
                    }

                }

                return savedLines;
            }
        }
        catch (FileNotFoundException e) {
            Debug.LogError(e.Message);
        }

        return null;
    }

    /// <summary>
    /// Loads a TextAsset to be read, wrapper function for the overloaded method.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <param name="includeBlankLines">Include empty lines.</param>
    /// <returns>The lines that were read.</returns>
    public static List<string> ReadTextAsset(string path, bool includeBlankLines = true) {
        TextAsset asset = Resources.Load<TextAsset>(path);

        if (asset == null)
        {
            Debug.LogError($"There is not asset file for {path}");
            return null;
        }

        return ReadTextAsset(asset, includeBlankLines);

    }

    /// <summary>
    ///  Reads a TextAsset file.
    /// </summary>
    /// <param name="asset">The TextAsset file object.</param>
    /// <param name="includeBlankLines">Include empty space.</param>
    /// <returns>The lines that were read.</returns>
    public static List<string> ReadTextAsset(TextAsset asset, bool includeBlankLines = true)
    {
        using (StringReader sr = new StringReader(asset.text))
        {
            List<string> savedLines = new List<string>();
            while (sr.Peek() > -1)
            {
                string line = sr.ReadLine();
                if (includeBlankLines || !string.IsNullOrWhiteSpace(line))
                {
                    savedLines.Add(line);
                }

            }

            return savedLines;
        }
    }

    /// <summary>
    /// Creates a directory if no directory exists, otherwise it uses in the passed parameter.
    /// </summary>
    /// <param name="path">The path to check or create.</param>
    /// <returns>Wether the directory is usable or exists.</returns>
    public static bool TryCreateDirectoryFromPath(string path)
    {
        if (Directory.Exists(path) || File.Exists(path))
            return true;

        if (path.Contains("."))
        {
            path = Path.GetDirectoryName(path);
            if (Directory.Exists(path))
                return true;
        }

        if (path == string.Empty)
            return false;

        try
        {
            Directory.CreateDirectory(path);
            return true;

        }
        catch (System.Exception e)
        {
            Debug.LogError($"Could not create directory! {e}");
            return false;
        }
    }

    /// <summary>
    /// Saves data in JSON format.
    /// </summary>
    /// <param name="filePath">The filepath where it should be stored.</param>
    /// <param name="JSONData">The JSON data to be saved.</param>
    public static void Save(string filePath, string JSONData)
    {
        if(!TryCreateDirectoryFromPath(filePath))
        {
            Debug.LogError($"Failed to save file '{filePath}' ");
            return;
        }

        StreamWriter sw = new StreamWriter(filePath);
        sw.Write(JSONData);
        sw.Close();

        Debug.Log($"Saved data to '{filePath}'");
    }

    /// <summary>
    /// Loads a file on a object.
    /// </summary>
    /// <typeparam name="T">The base object.</typeparam>
    /// <param name="filePath">The filepath where the resource is.</param>
    /// <returns>The JSON loaded.</returns>
    public static T Load<T>(string filePath) 
    {
        if (File.Exists(filePath))
        {
            string JSONData = File.ReadAllLines(filePath)[0];
            return JsonUtility.FromJson<T>(JSONData);
        }
        else 
        {
            Debug.LogError($"Error file does not exists '{filePath}'");
            return default(T);
        }
    }
}
