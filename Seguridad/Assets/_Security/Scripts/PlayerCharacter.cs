using UnityEngine;
using UnityEngine.EventSystems;

public class Drag3DFollower : MonoBehaviour
{
    [Header("Objeto 3D a mover")]
    public Transform target;

    [Header("Cámara que proyecta (si es null usa Camera.main)")]
    public Camera cam;

    [Header("Distancia desde la cámara para la proyección (en unidades de mundo)")]
    public float projectionDistance = 5f;

    [Header("Velocidad máxima (unidades/seg) para seguir el toque")]
    public float followSpeed = 20f;

    [Header("Bounds en pantalla (UI RectTransform opcional)")]
    public RectTransform bounds;

    private bool isDragging;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        // --- Mouse ---
        if (Input.GetMouseButtonDown(0)) isDragging = true;
        if (Input.GetMouseButtonUp(0)) isDragging = false;

        // --- Touch (opcional) ---
        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began) isDragging = true;
            else if (t.phase == TouchPhase.Ended ||
                     t.phase == TouchPhase.Canceled) isDragging = false;
        }

        if (!isDragging || target == null || cam == null) return;

        // Posición actual del puntero (mouse o touch)
        Vector2 screenPos = Input.touchCount > 0 ? (Vector2)Input.GetTouch(0).position : (Vector2)Input.mousePosition;

        // 1) Clampear dentro de bounds en PANTALLA (si hay)
        if (bounds != null)
        {
            screenPos = ClampScreenToBounds(screenPos, bounds, cam);
        }

        // 2) Proyectar a mundo en un plano paralelo a la cámara a 'projectionDistance'
        Vector3 desiredWorld = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, projectionDistance));

        // 3) Mover el target hacia el punto deseado respetando velocidad
        if (followSpeed <= 0f)
        {
            // Pegado inmediato (sin suavizado)
            target.position = desiredWorld;
        }
        else
        {
            float step = followSpeed * Time.deltaTime;
            target.position = Vector3.MoveTowards(target.position, desiredWorld, step);
        }
    }

    /// <summary>
    /// Clampea una posición de pantalla dentro de un RectTransform (independiente del modo del Canvas).
    /// </summary>
    private static Vector2 ClampScreenToBounds(Vector2 screenPos, RectTransform rect, Camera eventCamera)
    {
        // Obtener esquinas del rect en mundo
        Vector3[] worldCorners = new Vector3[4];
        rect.GetWorldCorners(worldCorners);

        // Convertir esas esquinas a coordenadas de PANTALLA
        Vector2 s0 = RectTransformUtility.WorldToScreenPoint(eventCamera, worldCorners[0]); // bottom-left
        Vector2 s1 = RectTransformUtility.WorldToScreenPoint(eventCamera, worldCorners[1]); // top-left
        Vector2 s2 = RectTransformUtility.WorldToScreenPoint(eventCamera, worldCorners[2]); // top-right
        Vector2 s3 = RectTransformUtility.WorldToScreenPoint(eventCamera, worldCorners[3]); // bottom-right

        float minX = Mathf.Min(Mathf.Min(s0.x, s1.x), Mathf.Min(s2.x, s3.x));
        float maxX = Mathf.Max(Mathf.Max(s0.x, s1.x), Mathf.Max(s2.x, s3.x));
        float minY = Mathf.Min(Mathf.Min(s0.y, s1.y), Mathf.Min(s2.y, s3.y));
        float maxY = Mathf.Max(Mathf.Max(s0.y, s1.y), Mathf.Max(s2.y, s3.y));

        return new Vector2(
            Mathf.Clamp(screenPos.x, minX, maxX),
            Mathf.Clamp(screenPos.y, minY, maxY)
        );
    }
}
