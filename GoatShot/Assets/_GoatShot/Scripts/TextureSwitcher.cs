using System.Collections.Generic;
using UnityEngine;

public class TextureSwitcher : MonoBehaviour
{
    [Header("Referencia al SkinnedMeshRenderer")]
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    [Header("Lista de Texturas")]
    [SerializeField] private List<Texture2D> textures;

    /// <summary>
    /// Cambia la textura del material según el índice de la lista.
    /// </summary>
    /// <param name="index">Índice de la textura en la lista</param>
    public void SetTextureByIndex(int index)
    {
        if (skinnedMeshRenderer == null)
        {
            Debug.LogError("No se asignó un SkinnedMeshRenderer en el inspector.");
            return;
        }

        if (textures == null || textures.Count == 0)
        {
            Debug.LogError("La lista de texturas está vacía.");
            return;
        }

        if (index < 0 || index >= textures.Count)
        {
            Debug.LogError("Índice fuera de rango: " + index);
            return;
        }

        // Cambiar textura en el material
        skinnedMeshRenderer.material.SetTexture("_BaseMap", textures[index]);

        Debug.Log("Textura cambiada a: " + textures[index].name);
    }
}
