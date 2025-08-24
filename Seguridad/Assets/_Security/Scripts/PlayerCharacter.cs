using Lean.Touch;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [Header("Elemento UI que se va a mover")]
    public RectTransform targetUI;

    [Header("Contenedor de límites (ej: Canvas, Panel)")]
    public RectTransform bounds;

    [Header("Velocidad de seguimiento")]
    public float followSpeed = 10f;

    private bool isDragging = false;
    private Vector2 lastTouchPos;

    void Update()
    {
        // --- PC / Editor con mouse ---
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastTouchPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Drag(Input.mousePosition);
        }
    }

    private void Drag(Vector2 currentPos)
    {
        Vector2 delta = currentPos - lastTouchPos;
        lastTouchPos = currentPos;

        // Aplicar movimiento
        Vector2 newPos = targetUI.position + new Vector3(delta.x, delta.y, 0);

        //take bounds into account
        if (bounds != null)
        {
            Vector2 min = bounds.position - new Vector3(bounds.sizeDelta.x, bounds.sizeDelta.y, 0) / 2;
            Vector2 max = bounds.position + new Vector3(bounds.sizeDelta.x, bounds.sizeDelta.y, 0) / 2;
            newPos.x = Mathf.Clamp(newPos.x, min.x, max.x);
            newPos.y = Mathf.Clamp(newPos.y, min.y, max.y);
        }

        targetUI.position = Vector2.Lerp(targetUI.position, newPos, followSpeed * Time.deltaTime);
    }
}
