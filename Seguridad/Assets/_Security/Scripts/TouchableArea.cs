using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchableArea : MonoBehaviour,IPointerClickHandler
{
    public UnityEvent<PointerEventData> OnAreaClick;
    private void Start()
    {
        // ignore transparent areas using alpha treshold
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f; // Adjust the threshold as needed
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnAreaClick?.Invoke(eventData);
    }
}
