using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TMPTextLoop : MonoBehaviour
{
    [TextArea] public string texto1 = "Texto 1";
    [TextArea] public string texto2 = "Texto 2";
    [Min(0.1f)] public float intervaloSegundos = 5f;
    public bool comenzarConTexto1 = true;

    TMP_Text tmp;
    Coroutine rutina;

    void Awake()
    {
        tmp = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        // Texto inicial
        tmp.text = comenzarConTexto1 ? texto1 : texto2;
        rutina = StartCoroutine(Loop());
    }

    void OnDisable()
    {
        if (rutina != null) StopCoroutine(rutina);
    }

    System.Collections.IEnumerator Loop()
    {
        bool mostrandoTexto1 = comenzarConTexto1;
        var wait = new WaitForSeconds(intervaloSegundos);

        while (true)
        {
            yield return wait;
            mostrandoTexto1 = !mostrandoTexto1;
            tmp.text = mostrandoTexto1 ? texto1 : texto2;
        }
    }
}

