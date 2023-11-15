using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager
{
    //TODO: Split into multiple classes.
    //En este caso podríamos usar el Factory Pattern para poder leer incluyendo archivos JSON.
    public static List<string> ReadTextFile(string path, bool includeBlankLines = true) {
        if(!path.StartsWith("/")) path = FilePaths.rootPath + path;

        try
        {
            using (StreamReader sr = new StreamReader(path))
            {
                List <string> savedLines = new List<string>();
                while(!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (includeBlankLines || !string.IsNullOrWhiteSpace(line)) {
                        savedLines.Add(line);
                    }

                }

                return savedLines;
            }
        }
        catch (FileNotFoundException e){
            Debug.LogError(e.Message);
        }
   
        return null;
    }

    public static List<string> ReadTextAsset(string path, bool includeBlankLines = true) { 
        TextAsset asset = Resources.Load<TextAsset>(path);

        if (asset == null)
        {
            Debug.LogError($"There is not asset file for {path}");
            return null;
        }

        return ReadTextAsset(asset, includeBlankLines);

    }

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

}
