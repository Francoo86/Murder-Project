using UnityEngine;

/// <summary>
/// Static class that holds paths of different resources.
/// </summary>
public class FilePaths
{
    private const string HOME_DIRECTORY_SYMBOL = "~/";

    public static readonly string rootPath = $"{Application.dataPath}/Main/Resources/";

    //RunTime Paths
    public static readonly string gameSaves = $"{runTimePath}Save Files/";

    //Paneles graficos, vale decir escenarios, videos de fondo.
    public static readonly string ResourcesGraphics = "Graphics/";

    //Fondos y videos.
    public static readonly string ResourcesBGImages = $"{ResourcesGraphics}BG Images/";
    public static readonly string ResourcesBGVideos = $"{ResourcesGraphics}BG Videos/";
    public static readonly string ResourcesBlendTexture = $"{ResourcesGraphics}Transition Effects/";

    //Audio.
    public static readonly string ResourcesAudio = "Audio/";
    public static readonly string ResourcesSFX = $"{ResourcesAudio}SFX/";
    public static readonly string ResourcesMusic = $"{ResourcesAudio}Music/";
    //Puede darse el caso? He visto que tenemos ese material.
    public static readonly string ResourcesAmbience = $"{ResourcesAudio}Ambience/";
    public static readonly string ResourcesDialogFiles = $"Dialog Files/";
    public static readonly string ResourcesFonts = $"Fonts/";

    /// <summary>
    /// Gets the path to resources, also checks if we have the ~/ in the path.
    /// </summary>
    /// <param name="defPath">The full path.</param>
    /// <param name="resourceName">The resource name, may be a txt, image, or music file.</param>
    /// <returns>The full path.</returns>
    public static string GetPathToResource(string defPath, string resourceName)
    {
        if (resourceName.StartsWith(HOME_DIRECTORY_SYMBOL))
        {
            return resourceName.Substring(HOME_DIRECTORY_SYMBOL.Length);
        }

        return defPath + resourceName;
    }

    public static string runTimePath 
    { 
        get
        {
            #if UNITY_EDITOR
                  return "Assets/appdata/";
            #else
                  return Application.persistentDataPath + "/appdata";
            #endif
        }
    }

}