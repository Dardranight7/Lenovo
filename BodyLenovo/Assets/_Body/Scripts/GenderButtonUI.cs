using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class GenderButtonUI : MonoBehaviour
{
    [Header("UI References")]
    public Button maleButton;
    public Button femaleButton;
    public TextMeshProUGUI maleText;
    public TextMeshProUGUI femaleText;

    [Header("Colors")]
    public Color selectedColor = Color.green;
    public Color deselectedColor = Color.white;
    public Color selectedTextColor = Color.white;
    public Color deselectedTextColor = Color.black;

    public UnityEvent<bool> OnSelectGenre;

    private void Start()
    {
        // Asignar eventos de los botones
        maleButton.onClick.AddListener(() => SelectGender(false));
        femaleButton.onClick.AddListener(() => SelectGender(true));
        SelectGender(false);
    }

    private void SelectGender(bool isFemale)
    {
        UpdateUI(isFemale);
        OnSelectGenre?.Invoke(!isFemale);
    }

    private void UpdateUI(bool isFemale)
    {
        // Hombre
        maleButton.image.color = isFemale ? deselectedColor : selectedColor;
        maleText.color = isFemale ? deselectedTextColor : selectedTextColor;

        // Mujer
        femaleButton.image.color = isFemale ? selectedColor : deselectedColor;
        femaleText.color = isFemale ? selectedTextColor : deselectedTextColor;
    }
}
