using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.IO;
using System.Threading.Tasks;

public class LenovoAPI : MonoBehaviour
{
    private const string BASE_URL = "https://us-central1-lenovo-experiences.cloudfunctions.net/api";
    private const string TOKEN = "gen-lenovo";
    public string LastUser;

    #region ==== MODELOS ====

    [Serializable]
    public class IngestRequest
    {
        public string phone;
        public string project; // goatHeart | goatMusic | goatBody
        public string originalUrl; // opcional
    }

    [Serializable]
    public class IngestResponse
    {
        public string url;
        public string path;
        public string updatedAt;
    }

    [System.Serializable]
    public class UserResponse
    {
        public string phone;
        public Timestamp lastUpdated;
        public Projects projects;
    }

    [System.Serializable]
    public class Timestamp
    {
        public long _seconds;
        public int _nanoseconds;
    }

    [System.Serializable]
    public class Projects
    {
        public ProjectData goatHeart;
        public ProjectData goatMusic;
        public ProjectData goatBody;
    }

    [System.Serializable]
    public class ProjectData
    {
        public string url;
        public Timestamp updatedAt;
    }

    #endregion

    #region ==== HELPERS ====

    // 🔹 Normaliza un teléfono eliminando espacios y caracteres no numéricos
    public static string NormalizePhone(string input)
    {
        var digits = new StringBuilder();
        foreach (char c in input)
        {
            if (char.IsDigit(c)) digits.Append(c);
        }
        return digits.ToString();
    }

    // 🔹 Envoltorio genérico para requests con JSON
    private async Task<string> SendRequest(UnityWebRequest www)
    {
        var tcs = new TaskCompletionSource<string>();

        www.SendWebRequest().completed += (asyncOp) =>
        {
            if (www.result == UnityWebRequest.Result.Success)
            {
                tcs.SetResult(www.downloadHandler.text);
            }
            else
            {
                tcs.SetException(new Exception($"Error {www.responseCode}: {www.error}"));
            }
        };

        return await tcs.Task;
    }

    #endregion

    #region ==== FUNCIONES ====

    /// <summary>
    /// POST /ingest con JSON
    /// </summary>
    public async Task<IngestResponse> IngestJson(IngestRequest req)
    {
        string json = JsonUtility.ToJson(req);
        byte[] body = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest www = new UnityWebRequest(BASE_URL + "/ingest", "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(body);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("x-ingest-token", TOKEN);

            string response = await SendRequest(www);
            return JsonUtility.FromJson<IngestResponse>(response);
        }
    }

    /// <summary>
    /// POST /ingest con archivo binario
    /// </summary>
    public async Task<IngestResponse> IngestFile(string phone, string project, byte[] fileData, string fileName)
    {
        WWWForm form = new WWWForm();
        form.AddField("phone", phone);
        form.AddField("project", project);
        form.AddBinaryData("file", fileData, fileName);

        using (UnityWebRequest www = UnityWebRequest.Post(BASE_URL + "/ingest", form))
        {
            www.SetRequestHeader("x-ingest-token", TOKEN);

            string response = await SendRequest(www);
            return JsonUtility.FromJson<IngestResponse>(response);
        }
    }

    /// <summary>
    /// GET /users/:phone
    /// </summary>
    public async Task<UserResponse> GetUser(string phone)
    {
        string url = $"{BASE_URL}/users/{NormalizePhone(phone)}";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("x-ingest-token", TOKEN);

            string response = await SendRequest(www);
            LastUser = phone;
            return JsonUtility.FromJson<UserResponse>(response);
        }
    }

    /// <summary>
    /// GET /users/:phone/projects/:project
    /// </summary>
    public async Task<ProjectData> GetProject(string phone, string project)
    {
        string url = $"{BASE_URL}/users/{NormalizePhone(phone)}/projects/{project}";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("x-ingest-token", TOKEN);

            string response = await SendRequest(www);
            return JsonUtility.FromJson<ProjectData>(response);
        }
    }

    /// <summary>
    /// GET /healthz
    /// </summary>
    public async Task<string> HealthCheck()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(BASE_URL + "/healthz"))
        {
            return await SendRequest(www);
        }
    }

    #endregion
}
