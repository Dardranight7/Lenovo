using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCarousel : MonoBehaviour
{
    [Header("Objetos a mostrar (solo uno activo a la vez)")]
    public List<GameObject> objects = new List<GameObject>();

    [Header("Tiempo en pantalla de cada objeto")]
    public float displayTime = 2f;

    [Header("Modo de paso")]
    public bool randomMode = false; // true = aleatorio, false = secuencial

    private int currentIndex = 0;
    private Coroutine loopCoroutine;

    void OnEnable()
    {
        StartCarousel();
    }

    void OnDisable()
    {
        StopCarousel();
    }

    public void StartCarousel()
    {
        if (loopCoroutine == null && objects.Count > 0)
            loopCoroutine = StartCoroutine(CarouselLoop());
    }

    public void StopCarousel()
    {
        if (loopCoroutine != null)
        {
            StopCoroutine(loopCoroutine);
            loopCoroutine = null;
        }
    }

    private IEnumerator CarouselLoop()
    {
        while (true)
        {
            // Apagar todos
            foreach (var obj in objects)
                if (obj != null) obj.SetActive(false);

            // Seleccionar índice
            if (randomMode)
            {
                currentIndex = Random.Range(0, objects.Count);
            }
            else
            {
                currentIndex = (currentIndex + 1) % objects.Count;
            }

            // Encender objeto seleccionado
            if (objects[currentIndex] != null)
                objects[currentIndex].SetActive(true);

            yield return new WaitForSeconds(displayTime);
        }
    }
}
