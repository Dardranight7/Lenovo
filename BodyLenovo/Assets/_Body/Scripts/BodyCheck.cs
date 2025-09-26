using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class BodyCheck : MonoBehaviour
{
    [SerializeField] TMP_InputField TMP_InputField;
    public LenovoAPI lenovoAPI;
    string cellphone;
    public TextMeshProUGUI ErrorMessage;
    public UnityEvent<LenovoAPI.UserResponse> OnConectionCorrectly;
    public UnityEvent<string> Cellphone;

    public LenovoAPI.UserResponse userResponse;

    private void Start()
    {
        TMP_InputField.onValueChanged.AddListener(UpdateCell);
    }

    public void UpdateCell(string number)
    {
        cellphone = number;
    }

    public void Send()
    {
        SendRequest();
    }

    public async void SendRequest()
    {
        try
        {        
            var response = await lenovoAPI.GetUser(cellphone);
        
            if (response != null)
            {
                userResponse = response;
                ErrorMessage.text = "Te falta participar en las experiencias:";
                bool problem = false;
                if (string.IsNullOrEmpty(response.projects.goatMusic.url))
                {
                    //ErrorMessage.text += "\n - GoatMusic";
                    //problem = true;
                    response.projects.goatMusic.url = "https://firebasestorage.googleapis.com/v0/b/lenovo-experiences.firebasestorage.app/o/AudioBaked%2F1.%20Soccer%20(Main).mp3?alt=media&token=3c81b1e9-15cf-4ecb-844b-49623e99fe4e";
                }
                if (string.IsNullOrEmpty(response.projects.goatHeart.url))
                {
                    ErrorMessage.text += "\n - GoatHearth";
                    problem = true;
                }
                if (problem)
                {
                    return;
                }
                else
                {
                    ErrorMessage.text = "";
                    //Dejarlo jugar :D
                    OnConectionCorrectly?.Invoke(response);
                }
            }
            else
            {
                ErrorMessage.text = "Debes participar en orden para estar registrado o revisa que el numero de celular este bien escrito.";
            }
        }
        catch
        {
            ErrorMessage.text = "Debes participar en orden para estar registrado o revisa que el numero de celular este bien escrito.";
        }
    }
}
