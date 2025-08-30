using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarViewManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeCounter;
    [SerializeField] TextMeshProUGUI CarName, Kilometers, Owners;
    float time;
    System.Action OnTimeEndCallback;
    Coroutine CountdownCoroutine;

    public void ShowCarData(GameManager.ExistingCars existingCar)
    {
        existingCar.ShowCar();
        CarName.text = existingCar.Name;
        Kilometers.text = existingCar.Kilometers;
        Owners.text = existingCar.Owners;
    }

    public void StartCountDown(System.Action Callback, float time)
    {
        this.time = Time.time + time;
        CountdownCoroutine = StartCoroutine(CountdownRoutine());
        OnTimeEndCallback = Callback;
    }

    IEnumerator CountdownRoutine()
    {
        while (time - Time.time > 0)
        {
            float currentTime = time - Time.time;
            int minutes = Mathf.FloorToInt(currentTime / 60F);
            int seconds = Mathf.FloorToInt(currentTime - minutes * 60);

            string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);
            timeCounter.text = niceTime;
            yield return null;
        }
        OnTimeEndCallback?.Invoke();
    }
}
