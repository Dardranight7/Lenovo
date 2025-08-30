using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Calification : MonoBehaviour
{
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Subtitle, Counter;
    public Image FaceIcon;
    public Sprite Smile, NoSmile;

    private void OnEnable()
    {
        ShowMessage();
    }
    public void ShowMessage()
    {
        int total = 0;
        total = GameManager.gameManager.valueMazda == 0 ? total + 1 : total;
        total = GameManager.gameManager.valueChevrolet == 2 ? total + 1 : total;
        total = GameManager.gameManager.valueToyotaHilux == 0 ? total + 1 : total;
        if (total == 2)
        {
            FaceIcon.gameObject.SetActive(false);
            Title.text = "¡Estuviste muy cerca!";
            Subtitle.text = "Para que tengas más tranquilidad en saber el estado del carro que quieres comprar, haz el peritaje en SURA para que el negocio salga bien.";
        }
        else if (total == 3)
        {
            FaceIcon.sprite = Smile;
            Title.text = "¡Muy bien!";
            Subtitle.text = "Has acertado en todas las opciones. \nRealiza el peritaje SURA para que el negocio salga bien.";
        }
        else
        {
            FaceIcon.sprite = NoSmile;
            Title.text = "¡Intenta de nuevo!";
            Subtitle.text = "No te preocupes si no te fue bien en el juego. Para esto tenemos un equipo de expertos que te pueden ayudar a mirar cuál es el carro usado que está en mejor estado y puedas comprarlo con toda tranquilidad. \nHaz el peritaje SURA para que el negocio salga bien.";
        }
        Counter.text = $"Acertaste {total} de 3";
    }
}
