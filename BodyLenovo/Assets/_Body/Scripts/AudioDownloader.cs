using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class AudioDownloader : MonoBehaviour
{
    public string audioURL;
    private string audioPath;
    public string audioName;
    public UnityEvent<string> OnAudioDownloaded;


    [ContextMenu("Download Video")]
    public void DownloadAudio()
    {
        int index = PlayerPrefs.GetInt("audioIndex" + audioName, 0);
        audioPath = Path.Combine(Application.persistentDataPath, audioName + index + ".mp3");
        StartCoroutine(DownloadAndStore());
    }

    public void DownloadAudio(string customURL)
    {
        int index = PlayerPrefs.GetInt("audioIndex" + audioName, 0);
        audioPath = Path.Combine(Application.persistentDataPath, audioName + index + ".mp3");
        audioURL = customURL;
        StartCoroutine(DownloadAndStore());
    }

    public void DownloadAudioFromUser(LenovoAPI.UserResponse userResponse)
    {
        int index = PlayerPrefs.GetInt("audioIndex" + audioName, 0);
        audioPath = Path.Combine(Application.persistentDataPath, audioName + index + ".mp3");
        audioURL = userResponse.projects.goatMusic.url;
        StartCoroutine(DownloadAndStore());
    }

    private IEnumerator DownloadAndStore()
    {
        // 1. Descargar archivos
        yield return DownloadFile(audioURL, audioPath);
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
            OnAudioDownloaded?.Invoke(savePath);
            PlayerPrefs.SetInt("audioIndex" + audioName, PlayerPrefs.GetInt("audioIndex" + audioName, 0) + 1);
        }
    }
}
