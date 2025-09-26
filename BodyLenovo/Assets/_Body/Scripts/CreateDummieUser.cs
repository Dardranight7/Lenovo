using UnityEngine;

public class CreateDummieUser : MonoBehaviour
{
    [SerializeField] LenovoAPI lenovoAPI;
    public LenovoAPI.UserResponse userResponse;

    [ContextMenu("CreateDummie")]
    public async void CreateDummieUserFunction()
    {
        await lenovoAPI.IngestJson(new LenovoAPI.IngestRequest()
        {
            phone = "3058665096",
            project = "goatHeart",
            originalUrl = "https://firebasestorage.googleapis.com/v0/b/lenovo-experiences.firebasestorage.app/o/Videos%2FGoatHearth%2Fvideo6.mp4?alt=media&token=bb7aae91-ef0c-4769-9120-31d43a273ba4",
        });
        Debug.Log("Se supone que ya se subio hearth");
        await lenovoAPI.IngestJson(new LenovoAPI.IngestRequest()
        {
            phone = "3058665096",
            project = "goatMusic",
            originalUrl = "https://firebasestorage.googleapis.com/v0/b/lenovo-experiences.firebasestorage.app/o/Videos%2FPara%20No%20Verte%20M%C3%A1s%20-%20La%20Mosca%20Ts%C3%A9%20-%20Ts%C3%A9.mp3?alt=media&token=4c1a9b71-da07-4557-861d-aa9547de8076",
        });
        Debug.Log("Se supone que ya se subio Music");
    }
}
