using UnityEngine;

public class PerspectiveScaler : MonoBehaviour
{
    [Header("Referencia de perspectiva")]
    public Vector3 scaleNear = Vector3.one; // Escala en el punto cercano
    public Vector3 scaleFar = Vector3.one * 0.5f; // Escala en el punto lejano

    void Update()
    {
        ApplyPerspectiveScale(transform);
    }

    /// <summary>
    /// Ajusta la escala de un objeto dependiendo de su posición en Y
    /// </summary>
    /// <param name="target">Objeto al que se le cambia la escala</param>
    public void ApplyPerspectiveScale(Transform target)
    {
        if (NearbyFarPos.Instance.pointNear == null || NearbyFarPos.Instance.pointFar == null) return;

        // Calcula el factor normalizado entre 0 y 1 según la Y
        float t = Mathf.InverseLerp(NearbyFarPos.Instance.pointFar.position.y, NearbyFarPos.Instance.pointNear.position.y, target.position.y);

        // Interpola entre la escala lejana y la cercana
        target.localScale = Vector3.Lerp(scaleFar, scaleNear, t);
    }
}

