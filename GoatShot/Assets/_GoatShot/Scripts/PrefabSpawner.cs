using UnityEngine;
using System.Collections.Generic;

public class PrefabSpawner : MonoBehaviour
{
    [Header("Prefab a spawnear")]
    public GameObject prefab;

    [Header("Cantidad inicial en pool")]
    public int poolSize = 10;

    private static PrefabSpawner instance;
    private Queue<GameObject> pool;

    void Awake()
    {
        instance = this;
        pool = new Queue<GameObject>();

        // Crear objetos en el pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    /// <summary>
    /// Spawnea un objeto desde la pool en la posición de pantalla indicada.
    /// </summary>
    /// <param name="screenPosition">Posición en pixeles de pantalla</param>
    public static void SpawnAtScreenPosition(Vector2 screenPosition)
    {
        if (instance == null || instance.pool.Count == 0) return;

        // Convertir posición de pantalla a mundo a 50 unidades de la cámara
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 screenPos = new Vector3(screenPosition.x, screenPosition.y, 50f);
        Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);

        // Sacar objeto del pool
        GameObject obj = instance.pool.Dequeue();
        obj.transform.position = worldPos;
        obj.transform.rotation = Quaternion.identity;
        obj.SetActive(true);

        // Volver a ponerlo al final de la cola (para reciclar)
        instance.pool.Enqueue(obj);
    }
}
