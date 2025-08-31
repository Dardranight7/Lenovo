using System;
using System.Collections;
using System.IO;
using Firebase;
using Firebase.Extensions;
using Firebase.Storage;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

public class ScreenshotUploader : MonoBehaviour
{
    [Header("Firebase")]
    public string firebaseStoragePath = "fotos"; // Carpeta en Storage

    [Header("UI")]
    public Image qrImageDisplay; // Arrastra un UI Image aquí para mostrar el QR

    private FirebaseStorage storage;
    private StorageReference storageRef;

    public ExperienceFlow flowManager; // Referencia a tu flow manager

    private void Start()
    {
        // Inicializar Firebase manualmente
        FirebaseApp app = FirebaseApp.Create(new AppOptions()
        {
            ApiKey = "AIzaSyB0iYSMU7tuWyMw-q5h4VKSgCq5LTZJoM4",
            AppId = "1:472633703949:web:c424fcf34b2f983c779f44",
            ProjectId = "lenovo-experiences",
            StorageBucket = "lenovo-experiences.firebasestorage.app",
            MessageSenderId = "472633703949",
        });

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
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

    public void CaptureAndUploadScreenshot()
    {
        StartCoroutine(CaptureScreenshotAndUpload());
    }

    private IEnumerator CaptureScreenshotAndUpload()
    {
        yield return new WaitForEndOfFrame(); // Espera a que termine el frame

        // Crear textura con el tamaño de la pantalla
        Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshot.Apply();

        // Convertir a PNG
        byte[] pngData = screenshot.EncodeToPNG();
        UnityEngine.Object.Destroy(screenshot);

        // Crear archivo temporal
        string exeFolder = Path.GetDirectoryName(Application.dataPath);
        int fotoIndex = PlayerPrefs.GetInt("fotoIndex", 0);
        string fileName = $"Foto{fotoIndex}.png";
        string filePath = Path.Combine(exeFolder, fileName);
        File.WriteAllBytes(filePath, pngData);

        Debug.Log("Captura guardada temporalmente en: " + filePath);

        // Subir a Firebase
        var fileRef = storageRef.Child($"{firebaseStoragePath}/{fileName}");
        var uploadTask = fileRef.PutFileAsync(filePath);

        bool isDone = false;
        string downloadUrl = null;

        uploadTask.ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Error al subir captura: " + task.Exception);
                isDone = true;
            }
            else
            {
                fileRef.GetDownloadUrlAsync().ContinueWithOnMainThread(urlTask =>
                {
                    if (!urlTask.IsFaulted && !urlTask.IsCanceled)
                    {
                        downloadUrl = urlTask.Result.ToString();
                        Debug.Log("Captura subida en: " + downloadUrl);

                        // Incrementar el contador
                        PlayerPrefs.SetInt("fotoIndex", fotoIndex + 1);
                    }
                    else
                    {
                        Debug.LogError("Error al obtener URL de descarga");
                    }
                    isDone = true;
                });
            }
        });

        yield return new WaitUntil(() => isDone);

        if (!string.IsNullOrEmpty(downloadUrl))
        {
            GenerateQRCode(downloadUrl);
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

        flowManager.Next();
    }
}
