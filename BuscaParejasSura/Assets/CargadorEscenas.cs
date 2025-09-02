using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CargadorEscenas : MonoBehaviour
{
    [SerializeField] private CanvasGroup cortinilla;

    public static CargadorEscenas instancia;

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if (instancia != this)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public async void cargarEscena()
    {
        cortinilla.alpha = 1;
        await Task.Delay(TimeSpan.FromSeconds(1));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        await Task.Delay(TimeSpan.FromSeconds(1));
        cortinilla.alpha = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
