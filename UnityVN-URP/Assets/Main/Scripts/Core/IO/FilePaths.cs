using UnityEngine;

/// <summary>
/// Clase que se encarga de manejar los directorios. Aunque funciona m�s como una estatica.
/// </summary>
public class FilePaths
{
    private const string HOME_DIRECTORY_SYMBOL = "~/";

    public static readonly string rootPath = $"{Application.dataPath}/Main/Resources/";

    //Paneles graficos, vale decir escenarios, videos de fondo.
    public static readonly string ResourcesGraphics = "Graphics/";

    //Fondos y videos.
    public static readonly string ResourcesBGImages = $"{ResourcesGraphics}BG Images/";
    public static readonly string ResourcesBGVideos = $"{ResourcesGraphics}BG Videos/";
    public static readonly string ResourcesBlendTexture = $"{ResourcesGraphics}Transition Effects/";

    //Audio.
    public static readonly string ResourcesAudio = "Audio/";
    public static readonly string ResourcesSFX = $"{ResourcesAudio}SFX";
    public static readonly string ResourcesMusic = $"{ResourcesAudio}Music";

    public static string GetPathToResource(string defPath, string resourceName)
    {
        if (resourceName.StartsWith(HOME_DIRECTORY_SYMBOL))
        {
            return resourceName.Substring(HOME_DIRECTORY_SYMBOL.Length);
        }

        return defPath + resourceName;
    }
}
