using UnityEngine;
using UnityEngine.EventSystems;

public class Touch : MonoBehaviour, IPointerClickHandler
{
    public FlowManager FlowManager;

    public void OnPointerClick(PointerEventData eventData)
    {
        FlowManager.UpdatePanel();
    }
}
