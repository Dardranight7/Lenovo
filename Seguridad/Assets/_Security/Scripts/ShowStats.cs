using System.Collections;
using TMPro;
using UnityEngine;

public class ShowStats : MonoBehaviour
{
    public TextMeshProUGUI think, sentinel, entered;

    public float animationDuration = 0.5f; // duración de la animación en segundos

    private void OnEnable()
    {
        UpdateStats();
    }

    public void UpdateStats()
    {
        // Animar cada valor de 0 hasta su valor final
        StartCoroutine(AnimateText(think, DeffenseMinigame.Instance.RejectedShield));
        StartCoroutine(AnimateText(sentinel, DeffenseMinigame.Instance.RejectedShield2));
        StartCoroutine(AnimateText(entered, DeffenseMinigame.Instance.Entered));
    }

    private IEnumerator AnimateText(TextMeshProUGUI targetText, int finalValue)
    {
        float elapsed = 0f;
        int startValue = 0;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);

            int currentValue = Mathf.RoundToInt(Mathf.Lerp(startValue, finalValue, t));
            targetText.text = currentValue.ToString("00");

            yield return null;
        }

        // Aseguramos que quede exactamente en el valor final
        targetText.text = finalValue.ToString("00");
    }
}
