using System.Collections.Generic;
using UnityEngine;

public class RandomDancer : MonoBehaviour
{
    [SerializeField] List<Animator> animator;
    private void OnEnable()
    {
        int random = Random.Range(0, 10);
        foreach (var anim in animator)
        {
            anim.Play(random.ToString());
        }
    }
}
