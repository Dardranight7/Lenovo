using UnityEngine;
using UnityEngine.Video;
using System.IO;

[RequireComponent(typeof(VideoPlayer))]
public class VideoLoader : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.isLooping = true; // Repetir en bucle
        videoPlayer.playOnAwake = false;
    }

    /// <summary>
    /// Carga un video desde una ruta absoluta o relativa.
    /// </summary>
    public void LoadVideo(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("⚠️ La ruta del video está vacía.");
            return;
        }

        string fullPath = path;

        // Si es ruta relativa, la buscamos en StreamingAssets
        if (!Path.IsPathRooted(path))
        {
            fullPath = Path.Combine(Application.streamingAssetsPath, path);
        }

        if (!File.Exists(fullPath))
        {
            Debug.LogError("❌ No se encontró el archivo de video en: " + fullPath);
            return;
        }

        videoPlayer.url = fullPath;
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPrepared;
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        source.prepareCompleted -= OnVideoPrepared;
        source.Play();
        Debug.Log("▶️ Reproduciendo video en loop: " + source.url);
    }
}
