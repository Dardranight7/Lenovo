using UnityEngine;
using UnityEngine.Events;

public class ListenHearthDevice : MonoBehaviour
{
    public NetworkReader callbackWhenDetectHearth;

    public UnityEvent OnHearth;

    public bool NeedBooleanCheck = false;
    public bool BooleanCheck = false;

    private void OnEnable()
    {
        callbackWhenDetectHearth.OnBpmChange.AddListener(SendMessageToCallback);
    }

    private void OnDisable()
    {

        callbackWhenDetectHearth.OnBpmChange.RemoveListener(SendMessageToCallback);
    }

    public void SetBooleanCheck(bool newValue)
    {
        BooleanCheck = newValue;
    }

    public void SendMessageToCallback(string value)
    {
        if (NeedBooleanCheck)
        {
            if (BooleanCheck)
            {
                OnHearth?.Invoke();
                BooleanCheck = false;
            }
        }
        else
        {
            OnHearth?.Invoke();
            BooleanCheck = false;
        }
    }
}
