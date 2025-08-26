using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class FFmpegRecorder : MonoBehaviour
{
    [Header("Recording Settings")]
    public Camera targetCamera;
    public int width = 1280;
    public int height = 720;
    public int fps = 30;
    public string outputPath = "output.mp4";

    private RenderTexture rt;
    private Process ffmpeg;
    private Stream ffmpegStdin;
    private Queue<byte[]> frameQueue = new Queue<byte[]>();
    private bool isRecording = false;

    [ContextMenu("StartRecording")]
    void StartRecording()
    {
        if (isRecording) return;
        isRecording = true;

        // RenderTexture donde pintamos la cámara
        rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        rt.Create();
        targetCamera.targetTexture = rt;

        // Lanzar FFmpeg con pipe stdin
        var psi = new ProcessStartInfo
        {
            FileName = Path.Combine(Application.dataPath, "ffmpeg/bin/ffmpeg.exe"), // Usa "ffmpeg.exe" si no está en el PATH
            Arguments = $"-y -f rawvideo -vcodec rawvideo -pix_fmt rgb24 -s {width}x{height} -r {fps} -i - -c:v libx264 -preset ultrafast -pix_fmt yuv420p \"{outputPath}\"",
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        ffmpeg = new Process { StartInfo = psi };
        ffmpeg.Start();
        ffmpegStdin = ffmpeg.StandardInput.BaseStream;

        UnityEngine.Debug.Log("🎥 Recording started!");
    }

    [ContextMenu("StopRecording")]
    void StopRecording()
    {
        if (!isRecording) return;
        isRecording = false;

        targetCamera.targetTexture = null;
        rt.Release();

        // Cerrar el pipe de FFmpeg
        ffmpegStdin.Close();
        ffmpeg.WaitForExit();
        ffmpeg.Close();

        UnityEngine.Debug.Log($"✅ Recording finished: {outputPath}");
    }

    void Update()
    {
        if (!isRecording) return;

        // Pedir un frame async
        AsyncGPUReadback.Request(rt, 0, TextureFormat.RGB24, OnFrameReady);

        // Vaciar cola hacia FFmpeg
        while (frameQueue.Count > 0)
        {
            var frame = frameQueue.Dequeue();
            ffmpegStdin.Write(frame, 0, frame.Length);
        }
    }

    void OnFrameReady(AsyncGPUReadbackRequest req)
    {
        if (!req.hasError)
        {
            NativeArray<byte> data = req.GetData<byte>();
            byte[] frame = new byte[data.Length];
            data.CopyTo(frame);
            frameQueue.Enqueue(frame);
        }
    }

    void OnApplicationQuit()
    {
        StopRecording();
    }
}