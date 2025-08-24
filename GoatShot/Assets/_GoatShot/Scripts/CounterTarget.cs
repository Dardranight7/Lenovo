using UnityEngine;
using UnityEngine.EventSystems;

public class CounterTarget : MonoBehaviour, IPointerDownHandler
{
    [HideInInspector] public float lifeTime;
    [HideInInspector] public float maxLifeTime = 4f;
    public GameObject goodView,excelentView, badView;

    private CounterMinigame manager;
    bool beTouched = false;

    public void Init(CounterMinigame minigame, float maxLife)
    {
        manager = minigame;
        maxLifeTime = maxLife;
        lifeTime = Time.time;
    }

    float touchedtime;

    private void Update()
    {
        if (beTouched)
        {
            if (touchedtime - Time.time <= 0)
            {
                beTouched = false;
                manager.ReturnToPool(gameObject); // En vez de Destroy
            }
            return;
        }
        if (Time.time - lifeTime > maxLifeTime)
        {
            manager.ReturnToPool(gameObject); // En vez de Destroy
        }
    }

    private void OnEnable()
    {
        HideAll();
    }

    public void HideAll()
    {
        excelentView.SetActive(false);
        goodView.SetActive(false);
        badView.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (beTouched)
            return; // Si ya ha sido tocado, no hacer nada más
        CounterMinigame.OnCounterTouched?.Invoke(this);
        beTouched = true;
        touchedtime = Time.time + 1f; // Mostrar 2 segundos
    }


}
