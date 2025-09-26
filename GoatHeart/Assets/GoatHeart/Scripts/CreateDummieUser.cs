using UnityEngine;

public class CreateDummieUser : MonoBehaviour
{
    [SerializeField] LenovoAPI lenovoAPI;
    public LenovoAPI.UserResponse userResponse;
    public string linkASubir;
    public string numeroCelular = "3007677421";

    [ContextMenu("CreateDummie")]
    public async void CreateDummieUserFunction()
    {
        await lenovoAPI.IngestJson(new LenovoAPI.IngestRequest()
        {
            phone = numeroCelular,
            project = "goatHeart",
            originalUrl = linkASubir,
        });
        Debug.Log("Se supone que ya se subio hearth");
        await lenovoAPI.IngestJson(new LenovoAPI.IngestRequest()
        {
            phone = numeroCelular,
            project = "goatMusic",
            originalUrl = linkASubir,
        });
        Debug.Log("Se supone que ya se subio Music");
    }
}
