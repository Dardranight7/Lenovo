using UnityEngine;
using TMPro;

public class PostLenovo : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] LenovoAPI lenovoAPI;
    [SerializeField] Transform PanelCellphone;
    [SerializeField] TextMeshProUGUI Error;

    private void Start()
    {
        inputField.onValueChanged.AddListener(SetCellphone);
    }

    private void OnDestroy()
    {
        inputField.onValueChanged.RemoveListener(SetCellphone);
    }

    private void OnEnable()
    {
        Error.text = "";
        inputField.text = "";
    }

    public void Next()
    {
        if (inputField.text.Length <= 8)
        {
            Error.text = "Porfavor ingresa un numero de telefono.";
            return;
        }
        PanelCellphone.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void SetCellphone(string number)
    {
        lenovoAPI.LastUser = number;
        Error.text = "";
    }
}
