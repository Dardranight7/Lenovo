using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldPositionateImage : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Transform target;
    [SerializeField] CanvasGroup visibility;
    bool isGlobalPositionate;
    bool isModelOcluded;

    private void Update()
    {
        Vector3 direction = target.position - Camera.main.transform.position;
        Ray ray = new Ray(Camera.main.transform.position, direction);
        if ((Vector3.Angle((target.position - Camera.main.transform.position), Camera.main.transform.forward) > 90) || Physics.Raycast(ray, direction.magnitude))
        {
            visibility.alpha = 0;
            image.raycastTarget = false;
        }
        else
        {
            visibility.alpha = 1;
            image.raycastTarget = true;
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            if (isGlobalPositionate)
            {
                Destroy(gameObject);
            }
            return;
        }
        transform.position = Camera.main.WorldToScreenPoint(target.position);
    }
}
