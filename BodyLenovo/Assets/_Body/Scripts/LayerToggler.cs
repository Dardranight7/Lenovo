using System.Collections.Generic;
using UnityEngine;

public class LayerToggler : MonoBehaviour
{
    // Lista de nombres de layers configurados en Unity
    public List<string> layerNames = new List<string>();

    /// <summary>
    /// Cambia el layer de un objeto y de todos sus hijos usando el índice de la lista de layers.
    /// </summary>
    /// <param name="target">Objeto al que se le cambiará el layer</param>
    /// <param name="index">Índice de la lista de layers</param>
    public void ChangeLayer(GameObject target, int index)
    {
        if (target == null)
        {
            Debug.LogWarning("No se asignó un objeto.");
            return;
        }

        if (index < 0 || index >= layerNames.Count)
        {
            Debug.LogWarning("Índice fuera de rango en la lista de layers.");
            return;
        }

        string layerName = layerNames[index];
        int layer = LayerMask.NameToLayer(layerName);

        if (layer == -1)
        {
            Debug.LogWarning("El nombre del layer no existe en el proyecto: " + layerName);
            return;
        }

        // Cambiar el layer en el objeto y todos sus hijos
        foreach (Transform t in target.GetComponentsInChildren<Transform>(true))
        {
            t.gameObject.layer = layer;
        }

        Debug.Log($"Se cambió el layer de {target.name} y todos sus hijos a '{layerName}'");
    }

    /// <summary>
    /// Cambia el layer de este mismo objeto y sus hijos.
    /// </summary>
    public void PutIndexLayer(int index)
    {
        ChangeLayer(gameObject, index);
    }
}

