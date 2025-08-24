using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FlowManager : MonoBehaviour
{
    public List<GameObject> gameObjects;
    int currentIndex = 0;

    public void UpdatePanel()
    {
        currentIndex = currentIndex + 1 > gameObjects.Count - 1 ? 0 : currentIndex + 1;
        foreach (GameObject go in gameObjects) {
            go.SetActive(false);
        }
        gameObjects[currentIndex].SetActive(true);
    }
}
