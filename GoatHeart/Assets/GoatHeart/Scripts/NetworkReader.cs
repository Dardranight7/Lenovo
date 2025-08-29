using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Collections.Concurrent; // para la cola segura
using System.Collections.Generic;
using UnityEngine.Events;

public class NetworkReader : MonoBehaviour
{
    private UdpClient udpClient;
    private Thread listenerThread;
    private bool running = false;

    public UnityEvent<string> OnBpmChange;
    public UnityEvent<string> OnSaturationChange;
    public int listenPort = 5067; // mismo puerto que en el Python

    // Cola thread-safe para mensajes entrantes
    private ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();

    public List<HearthBeat> hearthBeat;

    [Serializable]
    public class BeatData
    {
        public int bpm;
        public float beat_interval;
        public int spo2;
    }

    void Start()
    {
        StartServer(listenPort);
    }

    void Update()
    {
        // Procesar mensajes pendientes
        while (messageQueue.TryDequeue(out string json))
        {
            try
            {
                BeatData beat = JsonUtility.FromJson<BeatData>(json);
                if (beat != null)
                {
                    ReadData(beat);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("❌ Error deserializando JSON: " + ex.Message + "\n" + json);
            }
        }
    }

    [ContextMenu("Testing Data")]
    public void TestingData()
    {
        ReadData(new BeatData
        {
            bpm = UnityEngine.Random.Range(60, 100),
            beat_interval = UnityEngine.Random.Range(0.5f, 1.0f),
            spo2 = UnityEngine.Random.Range(90, 100)
        });
    }

    void OnApplicationQuit()
    {
        StopServer();
    }

    public void StartServer(int port)
    {
        running = true;
        listenerThread = new Thread(() =>
        {
            try
            {
                udpClient = new UdpClient(port);
                Debug.Log("Servidor UDP escuchando en puerto " + port);

                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

                while (running)
                {
                    byte[] data = udpClient.Receive(ref remoteEP);
                    string json = Encoding.UTF8.GetString(data);

                    // Encolar el JSON para procesarlo en el main thread
                    messageQueue.Enqueue(json);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error en servidor UDP: " + e);
            }
        });
        listenerThread.IsBackground = true;
        listenerThread.Start();
    }

    public void StopServer()
    {
        running = false;
        udpClient?.Close();
        listenerThread?.Abort();
    }

    // Aquí procesas la data ya deserializada
    public void ReadData(BeatData data)
    {
        Debug.Log($"📥 Data recibida: BPM={data.bpm}, Intervalo={data.beat_interval}");
        OnBpmChange?.Invoke($"{data.bpm} bpm");
        OnSaturationChange?.Invoke($"{data.spo2} %");

        if (hearthBeat != null && hearthBeat.Count > 0)
        {
            foreach (var heart in hearthBeat)
            {
                heart.SetTiempoDeCiclo(data.beat_interval);
            }
        }
    }
}
