using UnityEngine;

public class ScreenshootMaster : MonoBehaviour
{
    /// <summary>
    /// Captures an screenshot by using the main camera (the one who is active on the scene).
    /// </summary>
    /// <param name="width">Width of the final image.</param>
    /// <param name="height">Height of the final image.</param>
    /// <param name="supersize">Scale</param>
    /// <param name="filePath">The output path.</param>
    /// <returns>A Texture2D image taken by the main camera.</returns>
    public static Texture2D CaptureScreenshot(int width, int height, float supersize = 1, string filePath = "") => CaptureScreenshot(Camera.main, width, height, supersize, filePath);
    
    /// <summary>
    /// Captures the image and saves it into a texture using the camera scene.
    /// </summary>
    /// <param name="cam">Camera scene.</param>
    /// <param name="width">Width of the final texture..</param>
    /// <param name="height">Height of the final texture.</param>
    /// <param name="supersize">Scale</param>
    /// <param name="filePath">The path where it should be saved.</param>
    /// <returns>A Texture2D image taken by the camera.</returns>
    public static Texture2D CaptureScreenshot(Camera cam, int width, int height, float supersize = 1, string filePath = "")
    {
        if (supersize != 1)
        {
            width = Mathf.RoundToInt(width * supersize);
            height = Mathf.RoundToInt(height * supersize);
        }

        RenderTexture rt = RenderTexture.GetTemporary(width, height, 32);
        cam.targetTexture = rt;

        Texture2D screenshot = new Texture2D(width, height, TextureFormat.ARGB32, false);

        cam.Render();
        
        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);

        cam.targetTexture = null;
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        if (filePath != "")
            SaveScreenshotToFile(screenshot, filePath);

        return screenshot;
    }

    public enum ImageType { PNG, JPG }

    /// <summary>
    /// Saves the screenshot to an image format.
    /// </summary>
    /// <param name="screenshot">The Texture image.</param>
    /// <param name="filePath">The path where to store.</param>
    /// <param name="fileType">The filetype of the output (jpg, png)</param>
    public static void SaveScreenshotToFile(Texture2D screenshot, string filePath, ImageType fileType = ImageType.PNG)
    {
        byte[] bytes = new byte[0];
        string extension = "";

        switch (fileType) { 
            case ImageType.PNG:
                bytes = screenshot.EncodeToPNG();
                extension = ".png";
                break;
            case ImageType.JPG:
                bytes = screenshot.EncodeToJPG();
                extension = ".jpg";
                break;
        };

        if(!filePath.Contains('.'))
            filePath = filePath + extension;

        FileManager.TryCreateDirectoryFromPath(filePath);

        System.IO.File.WriteAllBytes(filePath, bytes);
    }
}
