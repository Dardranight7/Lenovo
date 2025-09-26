using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CounterMinigame : MonoBehaviour
{
    [Header("Scoring")]
    public int excelent, good, bad;

    [Header("Target Settings")]
    public GameObject prefabCounterTarget;
    public int poolSize = 10;
    public List<GameObject> counterPreInstanced = new List<GameObject>();

    [Header("Spawn Settings")]
    public RectTransform spawnArea; // Zona delimitada en UI
    public float spawnInterval = 1.0f; // Cada cuánto instanciar
    public float gameDuration = 20f;   // Duración del minijuego
    private float elapsedTime;

    public static System.Action<CounterTarget, float> OnCounterTouched;

    public TextMeshProUGUI timerText;
    private Coroutine gameLoop;

    public System.Action<int,int, int> OnGameEnd; // excelent, good, bad
    public UnityEvent OnEndGame;
    private void Start()
    {
        OnCounterTouched += ReadCounterTarget;
    }

    private void OnEnable()
    {
        gameLoop = StartCoroutine(GameRoutine());
        CreatePool();
        List<CounterTarget> targets = counterPreInstanced.Select(a=>a.GetComponent<CounterTarget>()).ToList();
        foreach (var item in targets)
        {
            item.disable = true;
            item.Init(this,10);
            item.spriteImage.color = new Color(1, 1, 1, 0.1f);
        }
        EnableOne();
        EnableOne();
        EnableOne();
    }

    private void OnDestroy()
    {
        OnCounterTouched -= ReadCounterTarget;
    }

    public List<GameObject> Options;

    private void CreatePool()
    {
        Options = new List<GameObject>(counterPreInstanced);
    }

    private GameObject GetFromPool()
    {
        GameObject option = Options[Random.Range(0, Options.Count)];
        Options.Remove(option);
        return option;
    }

    public void ReturnToPool(GameObject obj)
    {
        Options.Add(obj);
    }

    private IEnumerator GameRoutine()
    {
        excelent = good = bad = 0;
        elapsedTime = Time.time + gameDuration;
        timerText.text = (elapsedTime - Time.time).ToString("F1") + "s";
        while (elapsedTime > Time.time)
        {
            timerText.text = (elapsedTime - Time.time).ToString("F1") + "s";
            yield return null;
        }
        EndGame();
    }

    public GameObject poupView;
    public TextMeshProUGUI popupText;

    public List<string> GooMessages = new List<string>();
    public List<string> ExcelentMessages = new List<string>();
    public List<string> BadMessages = new List<string>();

    public void ReadCounterTarget(CounterTarget counterTarget, float distance)
    {
        poupView?.SetActive(true);

        if (distance <= 20)
        {
            excelent++;
            popupText.text = ExcelentMessages[Random.Range(0,ExcelentMessages.Count)];
        }
        else if (distance <= 50)
        {
            good++;
            popupText.text = GooMessages[Random.Range(0,GooMessages.Count)];
        }
        else
        {
            bad++;
            popupText.text = BadMessages[Random.Range(0,BadMessages.Count)];
        }
        elapsedTime += 2.5f;
        StartCoroutine(DisableMessageAfterTime());
        EnableOne();        
    }

    public void CountBad()
    {
        bad++;
        elapsedTime += 2.5f;
        poupView?.SetActive(true);
        popupText.text = BadMessages[Random.Range(0, BadMessages.Count)];
        StartCoroutine(DisableMessageAfterTime());
    }

    IEnumerator DisableMessageAfterTime()
    {
        yield return new WaitForSeconds(4.9f);
        poupView?.SetActive(false);
    }

    public void EnableOne()
    {
        GameObject nextTarget = GetFromPool();
        CounterTarget newTarget = nextTarget.GetComponent<CounterTarget>();
        newTarget.disable = false;
        newTarget.spriteImage.color = new Color(1, 1, 1, 1f);
    }

    private void EndGame()
    {
        Debug.Log("Juego terminado! Puntos: " + (excelent * 3 + good * 2 + bad));
        OnGameEnd?.Invoke(excelent, good, bad);
        OnEndGame?.Invoke();
        // Aquí puedes disparar un evento, mostrar UI de resultados, etc.
    }
}
