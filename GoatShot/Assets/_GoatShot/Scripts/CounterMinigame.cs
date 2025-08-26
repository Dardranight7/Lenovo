using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterMinigame : MonoBehaviour
{
    [Header("Scoring")]
    public int excelent, good, bad;

    [Header("Target Settings")]
    public GameObject prefabCounterTarget;
    public int poolSize = 10;
    private Queue<GameObject> pool;

    [Header("Spawn Settings")]
    public RectTransform spawnArea; // Zona delimitada en UI
    public float spawnInterval = 1.0f; // Cada cuánto instanciar
    public float gameDuration = 20f;   // Duración del minijuego
    private float elapsedTime;

    public static System.Action<CounterTarget> OnCounterTouched;

    private Coroutine gameLoop;

    public System.Action<int,int, int> OnGameEnd; // excelent, good, bad

    private void Start()
    {
        OnCounterTouched += ReadCounterTarget;
    }

    private void Awake()
    {
        CreatePool();
    }

    private void OnEnable()
    {
        gameLoop = StartCoroutine(GameRoutine());
    }

    private void OnDestroy()
    {
        OnCounterTouched -= ReadCounterTarget;
    }

    private void CreatePool()
    {
        pool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefabCounterTarget, spawnArea);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    private GameObject GetFromPool()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            // Si se acaban los objetos, puedes expandir el pool o reciclar
            GameObject obj = Instantiate(prefabCounterTarget, spawnArea);
            return obj;
        }
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    private IEnumerator GameRoutine()
    {
        excelent = good = bad = 0;
        elapsedTime = 0;
        while (elapsedTime < gameDuration)
        {
            SpawnTarget();
            yield return new WaitForSeconds(spawnInterval);
            elapsedTime += spawnInterval;
        }

        EndGame();
    }

    private void SpawnTarget()
    {
        GameObject obj = GetFromPool();

        // Calcular posición aleatoria dentro del RectTransform
        Vector2 size = spawnArea.rect.size;
        Vector2 randomPos = new Vector2(
            Random.Range(-size.x / 2, size.x / 2),
            Random.Range(-size.y / 2, size.y / 2)
        );

        obj.transform.localPosition = randomPos;

        // Inicializar el target
        CounterTarget target = obj.GetComponent<CounterTarget>();
        target.Init(this, 4f); // 4 segundos de vida
    }

    private IEnumerator ReturnAfterLifetime(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj.activeSelf) // Asegurar que no fue destruido por click
        {
            ReturnToPool(obj);
        }
    }

    public void ReadCounterTarget(CounterTarget counterTarget)
    {
        float diff = Time.time - counterTarget.lifeTime;

        counterTarget.HideAll();
        if (diff < 2f)
        {
            excelent++;
            counterTarget.excelentView.SetActive(true);
        }
        else if (diff < 3f)
        {
            good++;
            counterTarget.goodView.SetActive(true);
        }
        else
        {
            bad++;
            counterTarget.badView.SetActive(true);
        }
    }

    private void EndGame()
    {
        Debug.Log("Juego terminado! Puntos: " + (excelent * 3 + good * 2 + bad));
        OnGameEnd?.Invoke(excelent, good, bad);
        // Aquí puedes disparar un evento, mostrar UI de resultados, etc.
    }
}
