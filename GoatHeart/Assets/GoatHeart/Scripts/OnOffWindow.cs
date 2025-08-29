using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class OnOffWindow : MonoBehaviour
{
    public GameObject ObjectTo;

    private void OnEnable()
    {
        StartCoroutine(Toggle());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator Toggle()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            ObjectTo.SetActive(!ObjectTo.activeSelf);
        }
    }
}
