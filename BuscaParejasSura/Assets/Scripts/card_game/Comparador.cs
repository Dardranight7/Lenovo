using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Comparador : MonoBehaviour
{

    public List<GameObject> Comparables = new List<GameObject>();
    public int contador = 0;
    public AudioSource bocina;
    public AudioClip vfx_rotar;

    // Start is called before the first frame update
    public void Recibir_carta( GameObject Carta)
    {
        Comparables[contador] = Carta;
        contador += 1;
        if (contador == 2)
        {
            Comparar_GO();
        }
    }

    public void Comparar_GO()
    {
        contador = 0;
        if (Comparables[0].GetComponent<Image>().sprite == Comparables[1].GetComponent<Image>().sprite)
        {
            Llenador.llenador.puntaje += 1;
            switch (Llenador.llenador.marca)
            {
                case Marca.ECO3:
                    Llenador.llenador.textoPuntaje.text = Llenador.llenador.puntaje.ToString("00") + "/08";
                    if (Llenador.llenador.puntaje != 8)
                    {
                        Llenador.llenador.textoPuntajeCortinilla.text = Llenador.llenador.puntaje.ToString("00");
                    }
                    else if (Llenador.llenador.puntaje == 8)

                    {
                        float tfinal = Time.time;
                        var tiempoRestante = tfinal - (Llenador.llenador.tiempoAntesTerminar - Llenador.llenador.duracion);
                        Llenador.llenador.textopuuntajeconrtinilla.text = "Parejas encontradas:";
                        Llenador.llenador.textoPuntajeCortinilla.text = Llenador.llenador.puntaje.ToString("00");
                        Llenador.llenador.DescripcionTiempo.text = "en:" + tiempoRestante + "s";
                    }

                    break;
                case Marca.Hikvision:
                    Llenador.llenador.textoPuntaje.text = Llenador.llenador.puntaje.ToString("00") + "/06";
                    if (Llenador.llenador.puntaje != 6)
                    {
                        Llenador.llenador.textopuuntajeconrtinilla.text = "Parejas encontradas:";
                        Llenador.llenador.textoPuntajeCortinilla.text = Llenador.llenador.puntaje.ToString("00");
                        Llenador.llenador.DescripcionTiempo.text = "Se acabo el tiempo";
                    }
                    else if (Llenador.llenador.puntaje == 6)

                    {
                        float tfinal = Time.time;
                        var tiempoRestante = tfinal - (Llenador.llenador.tiempoAntesTerminar - Llenador.llenador.duracion);
                        Llenador.llenador.textopuuntajeconrtinilla.text = "Parejas encontradas:";
                        Llenador.llenador.textoPuntajeCortinilla.text = Llenador.llenador.puntaje.ToString("00");
                        Llenador.llenador.DescripcionTiempo.text = "en:" + tiempoRestante + "s";
                    }

                    break;
                case Marca.HikvisionDark:
                    Llenador.llenador.textoPuntaje.text = Llenador.llenador.puntaje.ToString("00") + "/06";
                    if (Llenador.llenador.puntaje != 6)
                    {
                        Llenador.llenador.textopuuntajeconrtinilla.text = "Parejas encontradas:";
                        Llenador.llenador.textoPuntajeCortinilla.text = Llenador.llenador.puntaje.ToString("00");
                        Llenador.llenador.DescripcionTiempo.text = "Se acabo el tiempo";
                    }
                    else if (Llenador.llenador.puntaje == 6)

                    {
                        float tfinal = Time.time;
                        var tiempoRestante = tfinal - (Llenador.llenador.tiempoAntesTerminar - Llenador.llenador.duracion);
                        Llenador.llenador.textopuuntajeconrtinilla.text = "Parejas encontradas:";
                        Llenador.llenador.textoPuntajeCortinilla.text = Llenador.llenador.puntaje.ToString("00");
                        Llenador.llenador.DescripcionTiempo.text = "en:" + tiempoRestante + "s";
                    }

                    break;
            }

            Llenador.llenador.ShowContent(Comparables[0].GetComponent<Image>().sprite);
            Debug.Log("Son Iguales");
        }
        else
        {
            foreach (GameObject Carta in Comparables)
            {
                Carta.GetComponent<Funcionalidad_Carta>().Tapar_carta();
            }
            Debug.Log("Son Diferentes");
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
