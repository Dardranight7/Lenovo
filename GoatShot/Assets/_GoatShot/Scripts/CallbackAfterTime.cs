using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CallbackAfterTime : MonoBehaviour
{
    [Header("Callback Settings")]
    [SerializeField] private UnityEvent OnTimeReached;
    [SerializeField] public float delay = 1.0f; // Tiempo en segundos antes de la invocación

    [Header("Optional UI Progress")]
    [SerializeField] private Image progressImage; // Debe estar en modo Filled - Horizontal
    [SerializeField] TextMeshProUGUI progressText; // Texto opcional para mostrar el progreso
    [SerializeField] TextMeshProUGUI backCounter;

    private void OnEnable()
    {
        StartCoroutine(Callback());
    }

    IEnumerator Callback()
    {
        float elapsed = 0f;

        // Resetear fill si existe
        if (progressImage != null)
            progressImage.fillAmount = 0f;

        while (elapsed < delay)
        {
            elapsed += Time.deltaTime;

            // Si hay una imagen asignada, actualizar su fill
            if (progressImage != null)
                progressImage.fillAmount = Mathf.Clamp01(elapsed / delay);

            // Si hay un texto asignado, actualizar su valor
            if (progressText != null)
                progressText.text = $"{Mathf.Clamp01(elapsed / delay) * 100f:0}%";

            // Si hay un contador de retroceso, actualizar su valor
            if (backCounter != null)
            {
                backCounter.text = Mathf.CeilToInt(delay - elapsed).ToString();
            }

            yield return null;
        }

        // Asegurar fill completo al terminar
        if (progressImage != null)
            progressImage.fillAmount = 1f;

        // Invocar el callback
        OnTimeReached?.Invoke();
    }
}

