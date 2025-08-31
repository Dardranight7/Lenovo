using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RandomizeTMPText : MonoBehaviour
{
    [Header("Lista de Textos TMP")]
    [SerializeField] private List<TextMeshProUGUI> texts;

    private void OnEnable()
    {
        if (texts == null || texts.Count == 0) return;

        for (int i = 0; i < texts.Count; i++)
        {
            if (texts[i] != null)
            {
                int randomValue = Random.Range(60, 100); // 100 es exclusivo, así que será 60–99
                texts[i].text = randomValue.ToString();
            }
        }
    }
}
