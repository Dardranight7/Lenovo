using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Autopass : MonoBehaviour
{
    async void Start()
    {
        await Task.Delay(3000);
        GameManager.gameManager.NextWindow();
    }
}
