using System.Collections.Generic;
using UnityEngine;

public class HearthBeat : MonoBehaviour
{
    [Header("Configuración")]
    public List<SkinnedMeshRenderer> meshRenderer = new List<SkinnedMeshRenderer>(); // Asignar en el inspector
    public float tiempoDeCiclo = 2f; // Tiempo total en segundos para ir y volver
    public int shapeKeyIndex = 0; // ShapeKey que vamos a controlar

    private float valorActual = 0f;
    private bool subiendo = true;
    private float velocidad; // Se calcula según el tiempo de ciclo

    void Start()
    {
        if (meshRenderer.Count < 0)
        {
            Debug.LogError("Asigna un SkinnedMeshRenderer en el inspector.");
            enabled = false;
            return;
        }

        CalcularVelocidad();
    }

    void Update()
    {
        // Movimiento
        if (subiendo)
        {
            valorActual = Mathf.MoveTowards(valorActual, 100f, velocidad * Time.deltaTime);
            if (Mathf.Approximately(valorActual, 100f))
                subiendo = false;
        }
        else
        {
            valorActual = Mathf.MoveTowards(valorActual, 0f, velocidad * Time.deltaTime);
            if (Mathf.Approximately(valorActual, 0f))
                subiendo = true;
        }

        // Aplicar valor al shape key
        foreach (var item in meshRenderer)
        {
            item.SetBlendShapeWeight(shapeKeyIndex, valorActual);
        }
    }

    // Permite cambiar el tiempo de ciclo desde otro script
    public void SetTiempoDeCiclo(float nuevoTiempo)
    {
        tiempoDeCiclo = Mathf.Max(0.01f, nuevoTiempo); // evitar división por cero
        CalcularVelocidad();
    }

    [ContextMenu("Recalcular Velocidad")]
    // Calcula velocidad según tiempo de ciclo (tiempo para ir y volver)
    private void CalcularVelocidad()
    {
        velocidad = (100f * 2f) / tiempoDeCiclo; // 100 ida + 100 vuelta
    }
}
