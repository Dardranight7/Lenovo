using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

[Serializable]
public class GoatShotResponse
{
    public string status;
    public string taskId;
    public string url;
    public string outputPath;
    public string promptUsed;
}

public class IAUrlProcess : MonoBehaviour
{
    private const string endpoint = "https://us-central1-lenovo-experiences.cloudfunctions.net/processGoatShotHttp";
    public Image image;
    public UnityEvent OnReceiveIAImage;

    public string urlTest = "https://firebasestorage.googleapis.com/v0/b/lenovo-experiences.firebasestorage.app/o/fotos%2FFoto12.png?alt=media&token=85476368-2861-442b-b244-ca00a49e3811";
    public string imageUrlTest = "https://firebasestorage.googleapis.com/v0/b/lenovo-experiences.firebasestorage.app/o/goat-shot%2Foutputs%2F2eaafe95-568c-4e45-a340-020149970e7f%2Foutput.png?alt=media&token=00cd1e7d-0162-45c2-93cc-8fa53a177cd7";

    [ContextMenu("Test")]
    public void Test()
    {
        SendImage(urlTest);
    }

    [ContextMenu("Test Download Image")]
    public void TestDownloadImage()
    {
        LoadImageFromUrl(imageUrlTest, image);
    }

    public async Task SendImage(string url)
    {
        GoatShotResponse task = await ProcessImageAsync(url);
        
        if (task != null)
        {
            Debug.Log("✅ Imagen procesada. URL de salida: " + task.url);
            // Aquí puedes manejar la respuesta, por ejemplo, mostrar la imagen en la U
            // Dowload from URL and display it in RawImage
            string imageUrl = task.url;
            await LoadImageFromUrl(imageUrl, image);

        }
        else
        {
            Debug.LogError("❌ Falló el procesamiento de la imagen.");
        }
    }

    public async Task LoadImageFromUrl(string url, Image targetImage)
    {
        Debug.Log("➡️ Descargando imagen desde: " + url);
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        Debug.Log("before await");
        await request.SendWebRequest(); // 👈 Unity 2021+ soporta esto
        Debug.Log("after await");

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );

            targetImage.sprite = sprite;
            Debug.Log("✅ Imagen descargada y mostrada");
            Debug.Log("✅ Imagen asignada correctamente");

            OnReceiveIAImage?.Invoke();
        }
        else
        {
            Debug.LogError($"❌ Error {request.responseCode}: {request.error}");
        }
        
    }


    [Serializable]
    public class GoatShotRequest
    {
        public string inputUrl;
        public string color;
    }

    /// <summary>
    /// Llama a la API GoatShot con una URL de imagen en Firebase y devuelve el objeto con la respuesta.
    /// </summary>
    /// <param name="imageUrl">URL pública de la imagen en Firebase Storage</param>
    /// <param name="color">Paleta de color opcional (ej: "gold and cyan")</param>
    public async Task<GoatShotResponse> ProcessImageAsync(string imageUrl, string color = null)
    {
        var payload = new GoatShotRequest
        {
            inputUrl = imageUrl
        };

        string jsonData = JsonUtility.ToJson(payload);
        Debug.Log("➡️ Enviando request a GoatShot: " + jsonData);

        using (UnityWebRequest request = new UnityWebRequest(endpoint, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            try
            {
                var operation = request.SendWebRequest();

                // Esperar async
                while (!operation.isDone)
                    await Task.Yield();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("✅ Respuesta recibida: " + request.downloadHandler.text);
                    GoatShotResponse response = JsonUtility.FromJson<GoatShotResponse>(request.downloadHandler.text);
                    return response;
                }
                else
                {
                    Debug.LogError("❌ Error: " + request.error);
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("❌ Excepción en la request: " + ex.Message);
                return null;
            }
        }
    }
}
