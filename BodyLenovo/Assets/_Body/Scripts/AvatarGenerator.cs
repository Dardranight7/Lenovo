using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class AvatarGenerator : MonoBehaviour
{
    public bool LoadFromCode = false;
    public string code;
    public RandomizeCharacter Male, Female;
    public TextMeshProUGUI CodeText;
    public List<AvatarGenerator> SlaveAvatar;
    public bool IsMale;

    public async void TryToLoadAvatar()
    {
        await LoadAvatarAsync();
    }

    public void SetGenre(bool genre)
    {
        IsMale = genre;
        Debug.Log(genre ? "Es hombre" : "Es mujer");
    }

    private async Task LoadAvatarAsync()
    {
        if (LoadFromCode && string.IsNullOrEmpty(code))
            return;

        // Siempre activar antes de generar
        Male.gameObject.SetActive(true);
        Female.gameObject.SetActive(true);

        bool isMale = IsMale;
        Debug.Log(isMale ? "Es hombre" : "Es mujer");

        if (LoadFromCode)
        {
            isMale = code[0] == '0';
            code = code.Remove(0,1);
        }
        else
        {
            if (isMale)
            {
                Male.GenerateRandomCharacter();
            }
            else 
            {
                Female.GenerateRandomCharacter();
            }
        }

        if (isMale)
        {
            Male.gameObject.SetActive(true);
            Female.gameObject.SetActive(false);
        }
        else
        {
            Male.gameObject.SetActive(false);
            Female.gameObject.SetActive(true);
        }

        // Espera un frame para asegurar que todo se procese bien
        await Task.Yield();

        if (LoadFromCode)
        {
            if (isMale)
            {
                Male.ApplyCharacterFromCode(code);
            }
            else
            {
                Female.ApplyCharacterFromCode(code);
            }
        }
        else
        {
            if (isMale)
            {
                if (CodeText != null)
                    CodeText.text = "0" + Male.currentCode;

                Female.gameObject.SetActive(false);

                foreach (var item in SlaveAvatar)
                {
                    item.code = "0" + Male.currentCode;
                    item.LoadFromCode = true;
                    item.TryToLoadAvatar(); // Llamada async también
                }
            }
            else
            {
                if (CodeText != null)
                    CodeText.text = "1" + Female.currentCode;

                Male.gameObject.SetActive(false);

                foreach (var item in SlaveAvatar)
                {
                    item.code = "1" + Female.currentCode;
                    item.LoadFromCode = true;
                    item.TryToLoadAvatar();
                }
            }
        }
    }
}
