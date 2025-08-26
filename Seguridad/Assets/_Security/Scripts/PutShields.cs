using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class PutShields : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent OnPutShield;
    public List<GameObject> Shields = new List<GameObject>();
    int index = 0;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (index >= Shields.Count) return;
        Shields[index].SetActive(true);
        index++;
        OnPutShield.Invoke();
    }
}
