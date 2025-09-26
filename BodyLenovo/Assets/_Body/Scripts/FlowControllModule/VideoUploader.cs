using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using Firebase.Storage;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

public class VideoUploader : MonoBehaviour
{
    [Header("Firebase")]
    public string firebaseStoragePath = "videos"; // Carpeta en Storage
    private string localFilePath = "C:/Users/usuario/video.mp4"; // Ruta al archivo .mp4 local
    [SerializeField] string FileName;

    [Header("UI")]
    public Image qrImageDisplay; // Arrastra un UI Image aquí para mostrar el QR

    private FirebaseStorage storage;
    private StorageReference storageRef;

    public bool GenerateQR = true;
    public UnityEvent OnVideoUploaded;

    public LenovoAPI lenovoAPI;
    public string projectName;

    private void Start()
    {
        // Inicializar Firebase
        // Inicialización manual (no necesita google-services.json)
        FirebaseApp app = FirebaseApp.Create(new AppOptions()
        {
            ApiKey = "AIzaSyB0iYSMU7tuWyMw-q5h4VKSgCq5LTZJoM4",
            AppId = "1:472633703949:web:c424fcf34b2f983c779f44",
            ProjectId = "lenovo-experiences",
            StorageBucket = "lenovo-experiences.firebasestorage.app",
            MessageSenderId = "472633703949",
        });


        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.Result == DependencyStatus.Available)
            {
                storage = FirebaseStorage.GetInstance(app);
                storageRef = storage.GetReferenceFromUrl("gs://lenovo-experiences.firebasestorage.app");
            }
            else
            {
                Debug.LogError("No se pudo inicializar Firebase: " + task.Result);
            }
        });
    }

    public void UploadAndGenerateQR()
    {
        StartCoroutine(UploadVideoAndGenerateQR());
    }

    private IEnumerator UploadVideoAndGenerateQR()
    {
        string exeFolder = Path.GetDirectoryName(Application.dataPath);
        string videoPath = Path.Combine(exeFolder, FileName + PlayerPrefs.GetInt("videoIndex", 0).ToString() + ".mp4");
        localFilePath = videoPath; // Asegura que la ruta es correcta para el dispositivo
        string fileName = Path.GetFileName(localFilePath);
        var fileRef = storageRef.Child($"{firebaseStoragePath}/{fileName}");

        Debug.Log("Subiendo archivo a Firebase: " + fileName);

        var uploadTask = fileRef.PutFileAsync(localFilePath);

        bool isDone = false;
        string downloadUrl = null;

        uploadTask.ContinueWithOnMainThread(task => {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Error al subir archivo: " + task.Exception);
                isDone = true;
            }
            else
            {
                fileRef.GetDownloadUrlAsync().ContinueWithOnMainThread(urlTask => {
                    if (!urlTask.IsFaulted && !urlTask.IsCanceled)
                    {
                        downloadUrl = urlTask.Result.ToString();
                        Debug.Log("Archivo disponible en: " + downloadUrl);
                        PlayerPrefs.SetInt("videoIndex", PlayerPrefs.GetInt("videoIndex",0) + 1);
                    }
                    else
                    {
                        Debug.LogError("Error al obtener URL de descarga");
                    }
                    isDone = true;
                });
            }
        });

        // Esperar hasta que la subida termine
        yield return new WaitUntil(() => isDone);
        OnVideoUploaded?.Invoke();

        if (GenerateQR)
        {
            if (!string.IsNullOrEmpty(downloadUrl))
            {
                GenerateQRCode(downloadUrl);
            }
        }
        if (lenovoAPI != null)
        {
            lenovoAPI.IngestJson(new LenovoAPI.IngestRequest()
            {
                phone = lenovoAPI.LastUser,
                project = projectName,
                originalUrl = downloadUrl,
            });
        }
    }

    private void GenerateQRCode(string text)
    {
        var qrWriter = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = 256,
                Width = 256,
                Margin = 1
            }
        };

        Color32[] qrPixels = qrWriter.Write(text);
        Texture2D qrTexture = new Texture2D(256, 256);
        qrTexture.SetPixels32(qrPixels);
        qrTexture.Apply();

        Sprite qrSprite = Sprite.Create(qrTexture, new Rect(0, 0, 256, 256), new Vector2(0.5f, 0.5f));
        qrImageDisplay.sprite = qrSprite;

        Debug.Log("QR generado con URL: " + text);
    }
}

