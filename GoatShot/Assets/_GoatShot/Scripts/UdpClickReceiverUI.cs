using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using UnityEngine.EventSystems;
using System.Collections.Generic;

// Mapea el JSON:
// { "pixel_coordinates":[x,y], "is_goal":true/false, "status":"GOL!/FAIL" }
[System.Serializable]
public class GoalMessage
{
    public int[] pixel_coordinates; // [x, y]
}

public class UdpClickReceiverUI : MonoBehaviour
{
    private UdpClient udpClient;
    private Thread receiveThread;

    // Cola thread-safe para pasar mensajes al main thread
    private readonly ConcurrentQueue<GoalMessage> messageQueue = new ConcurrentQueue<GoalMessage>();

    [Header("UDP Settings")]
    public int listenPort = 5067;

    [Header("Debug Simulator")]
    public int debugX = 640;
    public int debugY = 360;
    public string debugStatus = "DEBUG_CLICK";

    void Start()
    {
        udpClient = new UdpClient(listenPort);
        receiveThread = new Thread(ReceiveData) { IsBackground = true };
        receiveThread.Start();
        Debug.Log($"[UDP] Escuchando en puerto {listenPort}...");
    }

    void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, listenPort);

        try
        {
            while (true)
            {
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                string json = Encoding.UTF8.GetString(data);

                try
                {
                    var msg = JsonConvert.DeserializeObject<GoalMessage>(json);
                    if (msg != null)
                        messageQueue.Enqueue(msg);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[UDP] Error deserializando JSON: {e.Message}\n{json}");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[UDP] Error en recepción UDP: {e.Message}");
        }
    }

    void Update()
    {
        // Capturar posición real del cursor en debug con clic de rueda (opcional)
        if (Input.GetMouseButtonDown(2))
        {
            var m = Input.mousePosition;
            debugX = (int)m.x;
            debugY = (int)m.y;
            SimulateMessage();
            Debug.Log($"[DEBUG] Cursor capturado: X={debugX}, Y={debugY}");
        }

        // Procesar mensajes pendientes (main thread)
        while (messageQueue.TryDequeue(out var msg))
        {
            if (msg?.pixel_coordinates == null || msg.pixel_coordinates.Length < 2)
                continue;

            int x = msg.pixel_coordinates[0];
            int y = msg.pixel_coordinates[1];

            Debug.Log($"[UI] Simular click en: ({x},{y})");
            SimulateUIClick(new Vector2(x, y));
        }
    }

    /// <summary>
    /// Simula un click de UI en coordenadas de pantalla (sin invertir Y para Screen Space - Overlay).
    /// Ejecuta pointerDown -> pointerUp -> pointerClick sobre el primer objeto con IPointerClickHandler.
    /// </summary>
    void SimulateUIClick(Vector2 screenPosition)
    {
        if (EventSystem.current == null)
        {
            Debug.LogWarning("[UI] No hay EventSystem en la escena. Agrega uno (EventSystem + *InputModule).");
            return;
        }

        PrefabSpawner.SpawnAtScreenPosition(screenPosition);

        var pointerData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition,
            button = PointerEventData.InputButton.Left,
            clickCount = 1,
            eligibleForClick = true,
            pressPosition = screenPosition
        };

        // Raycast UI (usa todos los GraphicRaycaster activos)
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        if (results.Count == 0)
        {
            Debug.Log("[UI] No se encontró ningún elemento UI bajo ese punto.");
            return;
        }

        // Busca el primer objeto en la jerarquía que maneje IPointerClickHandler
        GameObject target = null;
        RaycastResult targetRaycast = default;

        foreach (var r in results)
        {
            var handler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(r.gameObject);
            if (handler != null)
            {
                target = handler;
                targetRaycast = r;
                break;
            }
        }

        if (target == null)
        {
            Debug.Log("[UI] Hay elementos UI bajo el punto, pero ninguno maneja IPointerClickHandler.");
            return;
        }

        // Completar datos de raycast en el PointerEventData
        pointerData.pointerCurrentRaycast = targetRaycast;
        pointerData.pointerPressRaycast = targetRaycast;

        // Secuencia típica de click
        ExecuteEvents.Execute(target, pointerData, ExecuteEvents.pointerEnterHandler);
        ExecuteEvents.Execute(target, pointerData, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(target, pointerData, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(target, pointerData, ExecuteEvents.pointerClickHandler);
        ExecuteEvents.Execute(target, pointerData, ExecuteEvents.pointerExitHandler);
    }

    [ContextMenu("Simular Mensaje Debug")]
    public void SimulateMessage()
    {
        var fakeMsg = new GoalMessage
        {
            pixel_coordinates = new[] { debugX, debugY },
        };

        Debug.Log($"[DEBUG] Encolando mensaje simulado en ({debugX}, {debugY}) - {debugStatus}");
        messageQueue.Enqueue(fakeMsg);
    }

    void OnApplicationQuit()
    {
        try { udpClient?.Close(); } catch { }
        try { if (receiveThread != null && receiveThread.IsAlive) receiveThread.Abort(); } catch { }
    }
}
