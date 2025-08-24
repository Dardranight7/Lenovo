using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomTitle : MonoBehaviour
{
    public Image TitleImage;

    public List<Sprite> TitleSprites;

    public void OnEnable()
    {
        TitleImage.sprite = TitleSprites[Random.Range(0, TitleSprites.Count)];
    }
}
