using UnityEngine;
using UnityEngine.EventSystems;

public class BadTarget : MonoBehaviour, IPointerClickHandler
{
    public CounterMinigame counterMinigame;
    public void OnPointerClick(PointerEventData eventData)
    {
        counterMinigame.CountBad();
    }
}
