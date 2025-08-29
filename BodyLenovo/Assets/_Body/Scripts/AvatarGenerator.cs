using System.Collections.Generic;
using System.Collections;
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

    private void OnEnable()
    {
        TryToLoadAvatar();
    }

    public void TryToLoadAvatar()
    {
        StartCoroutine(LoadAvatar());
    }

    public void SetGenre(bool genre)
    {
        IsMale = genre;
        Debug.Log(genre ? "Es hombre" : "Es mujer");
    }

    IEnumerator LoadAvatar()
    {
        if (LoadFromCode && code.Length <= 0)
        {
            yield break;
        }
        Male.gameObject.SetActive(true);
        Female.gameObject.SetActive(true);
        bool isMale = IsMale;
        Debug.Log(isMale ? "Es hombre" : "Es mujer");
        if (LoadFromCode)
        {
            isMale = code[0] == '0';
            code = code.Remove(0);
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
        yield return null;
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
                }
            }
        }
    }
}
