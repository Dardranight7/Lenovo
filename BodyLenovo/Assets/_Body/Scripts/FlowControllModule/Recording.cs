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

    [ContextMenu("Start Recording")]
    public void StartRecording()
    {
        if (isRecording) return;

        int index = PlayerPrefs.GetInt("videoIndex", 0);
        outputFileName = fileName + index.ToString() + ".mp4";

        // Crear el Texture2D para leer datos
        frameTexture = new Texture2D(targetTexture.width, targetTexture.height, TextureFormat.RGB24, false);

        // Comando FFmpeg para recibir datos crudos y comprimir a MP4
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = Path.Combine(Application.dataPath, "ffmpeg/bin/ffmpeg.exe"),
            Arguments = $"-y -f rawvideo -pixel_format rgb24 -video_size {targetTexture.width}x{targetTexture.height} " +
                        $"-framerate {frameRate} -i - -vf vflip -c:v libx264 -preset ultrafast -pix_fmt yuv420p \"{outputFileName}\"",
            UseShellExecute = false,
            RedirectStandardInput = true,
            CreateNoWindow = true
        };

        ffmpegProcess = new Process();
        ffmpegProcess.StartInfo = psi;
        ffmpegProcess.Start();

        ffmpegStream = new BinaryWriter(ffmpegProcess.StandardInput.BaseStream);
        isRecording = true;
        frameTimer = 0f; // Reinicia el acumulador

        UnityEngine.Debug.Log("🎥 Grabación iniciada");
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

            videoUploader.UploadAndGenerateQR(); // Llama al método para subir el video y generar el QR
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

        // Acumula tiempo real
        frameTimer += Time.deltaTime;
        float frameDuration = 1f / frameRate;

        // Solo escribir un frame cuando haya pasado suficiente tiempo
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
