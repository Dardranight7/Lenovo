using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CounterTarget : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public float lifeTime;
    [HideInInspector] public float maxLifeTime = 4f;


    private CounterMinigame manager;
    bool beTouched = false;
    public bool disable = true;

    public Image spriteImage;

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
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (disable)
        {
            return;
        }
        if (beTouched)
            return; // Si ya ha sido tocado, no hacer nada más
        Vector3 worldPos = eventData.pointerCurrentRaycast.worldPosition;
        CounterMinigame.OnCounterTouched?.Invoke(this, (transform.position - worldPos).sqrMagnitude);
        beTouched = true;
        spriteImage.color = new Color(1,1,1,0.1f);
        touchedtime = Time.time +0.2f; // Mostrar 2 segundos
        disable = true;
    }
}
