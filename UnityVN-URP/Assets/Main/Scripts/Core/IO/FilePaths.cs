using UnityEngine;

/// <summary>
/// Clase que se encarga de manejar los directorios. Aunque funciona más como una estatica.
/// </summary>
public class FilePaths
{
    private const string HOME_DIRECTORY_SYMBOL = "~/";

    public static readonly string rootPath = $"{Application.dataPath}/Main/Resources/";

    //Paneles graficos, vale decir escenarios, videos de fondo.
    public static readonly string resourcesGraphics = "Graphics/";

    //Fondos y videos.
    public static readonly string resourcesBGImages = $"{resourcesGraphics}BG Images/";
    public static readonly string resourcesBGVideos = $"{resourcesGraphics}BG Videos/";
    public static readonly string resourcesBlendTexture = $"{resourcesGraphics}Transition Effects/";

    //Audio.
    public static readonly string resourcesAudio = "Audio/";
    public static readonly string resourcesSFX = $"{resourcesAudio}SFX";
    public static readonly string resourcesMusic = $"{resourcesAudio}Music";

    public static string GetPathToResource(string defPath, string resourceName)
    {
        if (resourceName.StartsWith(HOME_DIRECTORY_SYMBOL))
        {
            return resourceName.Substring(HOME_DIRECTORY_SYMBOL.Length);
        }

        return defPath + resourceName;
    }
}
