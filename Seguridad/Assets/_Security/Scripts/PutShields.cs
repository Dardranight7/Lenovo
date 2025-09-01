using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PutShields : MonoBehaviour, IPointerClickHandler
{
    [Header("Prefab del Escudo (UI)")]
    public GameObject shieldPrefab;

    [Header("Zona contenedora de escudos (RectTransform)")]
    public RectTransform shieldsParent;

    [Header("Distancia mínima entre escudos (UI units)")]
    public float minDistance = 50f;

    [Header("Evento al poner escudo")]
    public UnityEvent OnPutShield;

    private List<RectTransform> spawnedShields = new List<RectTransform>();

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 localPoint;

        // Convertir la posición del click a coordenadas locales del contenedor
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            shieldsParent, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            // Revisar si ya hay un escudo muy cerca
            if (IsTooClose(localPoint)) return;

            // Instanciar el escudo como hijo del parent
            GameObject newShield = Instantiate(shieldPrefab, shieldsParent);
            RectTransform rt = newShield.GetComponent<RectTransform>();
            rt.anchoredPosition = localPoint;

            spawnedShields.Add(rt);
            OnPutShield?.Invoke();
        }
    }

    public void ClearShields()
    {
        foreach (var shield in spawnedShields)
        {
            if (shield != null)
            {
                Destroy(shield.gameObject);
            }
        }
        spawnedShields.Clear();
    }

    private bool IsTooClose(Vector2 pos)
    {
        float minSqr = minDistance * minDistance;
        foreach (var shield in spawnedShields)
        {
            if (shield == null) continue;
            float sqrDist = (shield.anchoredPosition - pos).sqrMagnitude;
            if (sqrDist < minSqr)
            {
                return true; // Demasiado cerca
            }
        }
        return false;
    }
}

