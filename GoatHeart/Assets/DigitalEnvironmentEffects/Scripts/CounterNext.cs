using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CounterNext : MonoBehaviour
{
    public TextMeshProUGUI CounterTitle;
    public ExperienceFlow experienceFlow;
    public UnityEvent OnEnableEvent;

    private void OnEnable()
    {
        OnEnableEvent?.Invoke();
    }

    public void StartRoutine()
    {
        StartCoroutine(counterRoutine());
    }
    IEnumerator counterRoutine()
    {
        CounterTitle.text = "Iniciando en 3";
        yield return new WaitForSeconds(1f);
        CounterTitle.text = "Iniciando en 2";
        yield return new WaitForSeconds(1f);
        CounterTitle.text = "Iniciando en 1";
        yield return new WaitForSeconds(1f);
        experienceFlow.Next();
        CounterTitle.text = "Iniciar";
    }
}
