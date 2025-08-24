using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CallbackAfterTime : MonoBehaviour
{
    [SerializeField] UnityEvent OnTimeReached;
    private void OnEnable()
    {
        StartCoroutine(Callback());
    }

    public float delay = 1.0f; // Time in seconds before the callback is invoked

    IEnumerator Callback()
    {
        yield return new WaitForSeconds(delay);
        OnTimeReached?.Invoke();
    }
}
