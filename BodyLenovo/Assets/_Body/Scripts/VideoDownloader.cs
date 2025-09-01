using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class VideoDownloader : MonoBehaviour
{
    public string videoURL;
    public string videoPath;
    public string videoName;
    public UnityEvent OnVideoDownloaded;

    private void Start()
    {
        int index = PlayerPrefs.GetInt("videoIndex" + videoName, 0);
        videoPath = Path.Combine(Application.persistentDataPath, videoName + index + ".mp4");
    }

    public void DownloadVideo()
    {
        StartCoroutine(DownloadAndStore());
    }

    private IEnumerator DownloadAndStore()
    {
        // 1. Descargar archivos
        yield return DownloadFile(videoURL, videoPath);
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
            OnVideoDownloaded?.Invoke();
            PlayerPrefs.SetInt("videoIndex" + videoName, PlayerPrefs.GetInt("videoIndex" + videoName, 0) + 1);
        }
    }
}
