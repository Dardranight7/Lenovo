using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class Funcionalidad_Carta : MonoBehaviour, IPointerClickHandler
{

    public List<Sprite> Default_Frontal = new List<Sprite>();
    public Image Imagen_Carta, CompareColor;
    public Animator anim;
    public Comparador Comparar;
    public bool girado = false;

    // Start is called before the first frame update
    void Start()
    {
        Comparar = FindObjectOfType<Comparador>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (girado == false)
        {
            Comparar.bocina.PlayOneShot(Comparar.vfx_rotar);
            anim.Play("Giro_Carta");
        }
    }

    public void SetColor(Color color)
    {
        CompareColor.color = color;
    }

    public void Girar_carta()
    {
        girado = true;
        Imagen_Carta.sprite = Default_Frontal[1];
        Comparar.Recibir_carta(this.gameObject);
    }
    public void Poner_boca_abajo()
    {
        Comparar.bocina.PlayOneShot(Comparar.vfx_rotar);
        girado = false;
        Imagen_Carta.sprite = Default_Frontal[0];
    }

    public void Tapar_carta()
    {
        StartCoroutine("esperar");
    }

    IEnumerator esperar()
    {
        yield return new WaitForSeconds(.5f);
        anim.Play("Giro_Carta_2");
    }

}
