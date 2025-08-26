using UnityEngine;
using UnityEngine.Events;

public class OnEnableCallback : MonoBehaviour
{
    public UnityEvent OnEnableCall;
    private void OnEnable()
    {
        OnEnableCall?.Invoke();
    }
}
