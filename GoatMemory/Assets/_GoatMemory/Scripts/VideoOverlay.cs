using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class RuntimeVideoOverlay : MonoBehaviour
{
    [Header("Links a los archivos (https o file://)")]
    public string mainVideoURL;
    public string overlayVideoURL;
    public string audioURL;

    [Header("Config FFmpeg")]
    public Vector2 overlaySize = new Vector2(320, 180);
    public Corner overlayCorner = Corner.TopRight;

    private string mainVideoPath;
    private string overlayVideoPath;
    private string audioPath;
    private string outputPath;

    public enum Corner { TopLeft, TopRight, BottomLeft, BottomRight }

    private void Start()
    {
        // En runtime, guardaremos todo en persistentDataPath
        mainVideoPath = Path.Combine(Application.persistentDataPath, "main.mp4");
        overlayVideoPath = Path.Combine(Application.persistentDataPath, "overlay.mp4");
        audioPath = Path.Combine(Application.persistentDataPath, "audio.mp3");
        outputPath = Path.Combine(Application.persistentDataPath, "final.mp4");

        StartCoroutine(DownloadAndCombine());
    }

    private IEnumerator DownloadAndCombine()
    {
        // 1. Descargar archivos
        yield return DownloadFile(mainVideoURL, mainVideoPath);
        yield return DownloadFile(overlayVideoURL, overlayVideoPath);
        yield return DownloadFile(audioURL, audioPath);

        // 2. Procesar con FFmpeg
        CombineVideos();
    }

    private IEnumerator DownloadFile(string url, string savePath)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.downloadHandler = new DownloadHandlerFile(savePath);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            UnityEngine.Debug.LogError("❌ Error descargando " + url + " -> " + www.error);
        }
        else
        {
            UnityEngine.Debug.Log("✅ Archivo descargado en: " + savePath);
        }
    }

    private void CombineVideos()
    {
        string positionFilter = GetPositionFilter();

        string filter = $"[1:v]scale={overlaySize.x}:{overlaySize.y}[ov];" +
                        $"[0:v][ov]overlay={positionFilter}[v]";

        string args = $"-i \"{mainVideoPath}\" -i \"{overlayVideoPath}\" -i \"{audioPath}\" " +
                      $"-filter_complex \"{filter};[2:a]anull[a]\" " +
                      "-map \"[v]\" -map \"[a]\" -shortest " +
                      $"\"{outputPath}\" -y";

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = Path.Combine(Application.dataPath, "ffmpeg/bin/ffmpeg.exe"), // ⚡ Pon ffmpeg.exe en StreamingAssets
            Arguments = args,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        Process process = new Process { StartInfo = startInfo };
        process.Start();

        string output = process.StandardError.ReadToEnd();
        process.WaitForExit();

        UnityEngine.Debug.Log("FFmpeg output: " + output);
        UnityEngine.Debug.Log("🎥 Video generado en: " + outputPath);
    }

    private string GetPositionFilter()
    {
        return overlayCorner switch
        {
            Corner.TopLeft => "10:10",
            Corner.TopRight => "main_w-overlay_w-10:10",
            Corner.BottomLeft => "10:main_h-overlay_h-10",
            Corner.BottomRight => "main_w-overlay_w-10:main_h-overlay_h-10",
            _ => "10:10"
        };
    }
}
