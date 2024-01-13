using System.Collections;
using System.IO;
using UnityEngine;

public class MyScreenshot : MonoBehaviour
{
    public static string PngPath = Path.Combine(Application.persistentDataPath, "screenshot.png");
    public static string JpgPath = Path.Combine(Application.persistentDataPath, "screenshot.jpg");

    public static void CaptureScreenshot()
    {
        ScreenCapture.CaptureScreenshot(PngPath);
    }

    public static IEnumerator SaveScreenJPG()
    {
        // Read the screen buffer after rendering is complete
        yield return new WaitForEndOfFrame();

        // Create a texture in RGB24 format the size of the screen
        int width = Screen.width;
        int height = Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        // Read the screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        // Encode the texture in JPG format
        var bytes = tex.EncodeToJPG();
        Object.Destroy(tex);

        // Write the returned byte array to a file in the project folder
        File.WriteAllBytes(JpgPath, bytes);
    }
}
