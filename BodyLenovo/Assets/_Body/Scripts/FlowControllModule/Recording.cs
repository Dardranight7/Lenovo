using UnityEngine;
using System.Diagnostics;
using System.IO;
using System;

public class Recording : MonoBehaviour
{
    [Header("Grabación")]
    public RenderTexture targetTexture;
    public string outputFileName = "video.mp4";
    public string fileName = "Body";
    public int frameRate = 30;

    [Header("Audio (opcional)")]
    [Tooltip("Deja vacío si no quieres audio. Puede ser una ruta a un archivo o un dispositivo.")]
    public string audioInput = "";

    private Process ffmpegProcess;
    private BinaryWriter ffmpegStream;
    private Texture2D frameTexture;
    private bool isRecording = false;

    private float frameTimer = 0f; // Acumulador de tiempo

    public VideoUploader videoUploader; // Referencia al VideoUploader

    void Start()
    {
        Application.runInBackground = true;
    }

    public void SetAudioInput(string URI)
    {
        audioInput = URI;
    }

    [ContextMenu("Start Recording")]
    public void StartRecording()
    {
        if (isRecording) return;

        int index = PlayerPrefs.GetInt("videoIndex", 0);
        outputFileName = fileName + index.ToString() + ".mp4";

        // Crear el Texture2D para leer datos
        frameTexture = new Texture2D(targetTexture.width, targetTexture.height, TextureFormat.RGB24, false);

        // Construcción de argumentos de FFmpeg
        string ffmpegArgs =
            $"-y -f rawvideo -pixel_format rgb24 -video_size {targetTexture.width}x{targetTexture.height} " +
            $"-framerate {frameRate} -i -";

        // Si hay audio configurado, lo añadimos como input
        if (!string.IsNullOrEmpty(audioInput))
        {
            ffmpegArgs += $" -i \"{audioInput}\"";
        }

        // Siempre aplica el flip de video después de definir inputs
        ffmpegArgs += " -vf vflip";

        // Codecs de salida
        if (!string.IsNullOrEmpty(audioInput))
        {
            ffmpegArgs += " -c:v libx264 -preset ultrafast -pix_fmt yuv420p -c:a aac -shortest";
        }
        else
        {
            ffmpegArgs += " -c:v libx264 -preset ultrafast -pix_fmt yuv420p";
        }

        // Archivo de salida
        ffmpegArgs += $" \"{outputFileName}\"";

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = Path.Combine(Application.dataPath, "ffmpeg/bin/ffmpeg.exe"),
            Arguments = ffmpegArgs,
            UseShellExecute = false,
            RedirectStandardInput = true,
            CreateNoWindow = true
        };

        ffmpegProcess = new Process();
        ffmpegProcess.StartInfo = psi;
        ffmpegProcess.Start();

        ffmpegStream = new BinaryWriter(ffmpegProcess.StandardInput.BaseStream);
        isRecording = true;
        frameTimer = 0f;

        UnityEngine.Debug.Log("🎥 Grabación iniciada con" + (string.IsNullOrEmpty(audioInput) ? " sin audio" : $" audio: {audioInput}"));
    }

    [ContextMenu("Stop Recording")]
    public void StopRecording()
    {
        if (!isRecording) return;

        isRecording = false;

        try
        {
            ffmpegStream.Close();
            ffmpegProcess.WaitForExit();
            ffmpegProcess.Close();

            videoUploader.UploadAndGenerateQR();
            UnityEngine.Debug.Log("✅ Grabación finalizada: " + outputFileName);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Error cerrando FFmpeg: " + e.Message);
        }

        Destroy(frameTexture);
    }

    void LateUpdate()
    {
        if (!isRecording) return;

        frameTimer += Time.deltaTime;
        float frameDuration = 1f / frameRate;

        if (frameTimer >= frameDuration)
        {
            frameTimer -= frameDuration;

            RenderTexture.active = targetTexture;
            frameTexture.ReadPixels(new Rect(0, 0, targetTexture.width, targetTexture.height), 0, 0);
            frameTexture.Apply();

            byte[] bytes = frameTexture.GetRawTextureData();
            ffmpegStream.Write(bytes, 0, bytes.Length);
        }
    }
}
