using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public enum Marca
{
    ECO3,
    Hikvision,
    HikvisionDark
}

public class Llenador : MonoBehaviour
{
    public List<Sprite> sprites;
    public HashSet<Sprite> ListaVolcada = new HashSet<Sprite>();
    public HashSet<Sprite> ListaVolcada2 = new HashSet<Sprite>();
    public List<Sprite> ListaVolcada_List = new List<Sprite>();
    public List<Sprite> ListaVolcada2_List = new List<Sprite>();
    public List<Color> colorList = new List<Color>();
    public List<Sprite> backList = new List<Sprite>();

    public TextMeshProUGUI textoPuntaje;
    public TextMeshProUGUI textoPuntajeCortinilla, textopuuntajeconrtinilla, DescripcionTiempo;
    [SerializeField] private TextMeshProUGUI textoTiempo;
    public GameObject instancia;

    public int numberOfCards = 8;
    public int puntaje;
    public float tiempoAntesTerminar;
    public float duracion;
    public static Llenador llenador;
    public GameObject cortinilla;

    public Marca marca;


     #if UNITY_EDITOR
    private void OnValidate()  // Se actualiza al cambiar en el Inspector
    {
        AplicarMarca(marca);
    }

#endif

    public void AplicarMarca(Marca m)
    {
        switch (m)
        {
            case Marca.ECO3:
                sprites = (Resources.LoadAll<Sprite>("Imagenes/Eco")).ToList();

                break;

            case Marca.Hikvision:
                sprites = (Resources.LoadAll<Sprite>("Imagenes/hikvision")).ToList();

                break;

            case Marca.HikvisionDark:
                sprites = (Resources.LoadAll<Sprite>("Imagenes/HikvisionDark")).ToList();
                break ;

            default:
                sprites = (Resources.LoadAll<Sprite>("Imagenes/Eco")).ToList();
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        switch (marca)  
        {
            case Marca.ECO3:
                textoPuntaje.text = puntaje.ToString("00") + "/08"; 
                break;
            case Marca.Hikvision:
                textoPuntaje.text = puntaje.ToString("00") + "/06"; 
                break;
            case Marca.HikvisionDark:
                textoPuntaje.text = puntaje.ToString("00") + "/06";
                break;
            default:
                textoPuntaje.text = puntaje.ToString("00") + "/06";
                break;
        }

        llenador = this;
        tiempoAntesTerminar = Time.time + duracion;
        AplicarMarca(marca);
        while (sprites.Count > numberOfCards)
        {
            sprites.RemoveAt(Random.Range(0, sprites.Count));
        }
        while (ListaVolcada.Count < sprites.Count)
        {
            ListaVolcada.Add(sprites[Random.Range(0, sprites.Count)]);
        }
        while (ListaVolcada2.Count < sprites.Count)
        {
            ListaVolcada2.Add(sprites[Random.Range(0, sprites.Count)]);
        }
        foreach (Sprite spr in ListaVolcada)
        {
            ListaVolcada_List.Add(spr);
        }
        foreach (Sprite spr in ListaVolcada2)
        {
            ListaVolcada2_List.Add(spr);
        }
        colorList = colorList.OrderBy((a) => Random.Range(0f, 1f)).ToList();
        int v = -2;
        int u = -1;
        for (int i = 0; i < ListaVolcada_List.Count; i++)
        {
            Debug.Log("hello" + i);
            v += 2;
            crear_carta(ListaVolcada_List[i], v);
            u += 2;
            crear_carta(ListaVolcada2_List[i], u);
        }
    }

    public void crear_carta(Sprite imagen, int indice = 0)
    {
        GameObject temporal;
        temporal = Instantiate(instancia,transform);
        temporal.GetComponent<Funcionalidad_Carta>().Default_Frontal[1] = imagen;
        temporal.GetComponent<Funcionalidad_Carta>().Default_Frontal[0] = backList[indice];
        temporal.GetComponent<Image>().sprite = backList[indice];
    }

    bool timeIsPaused;
    [SerializeField] GameObject showContentParent;
    [SerializeField] TextMeshProUGUI cardText, cardResponseText;
    [SerializeField] Image image_Bg;
    [SerializeField] List<SpriteTextContainer> spriteDefinitions = new List<SpriteTextContainer>();

    [System.Serializable]
    public class SpriteTextContainer
    {
        public Sprite sprite;
        public string text, textResponse;
        public Sprite Imagebg;
    }

    public void ShowContent(Sprite sprite)
    {
        showContentParent.SetActive(true);
        timeIsPaused = true;
        SpriteTextContainer text = spriteDefinitions.Where((a) => a.sprite == sprite).FirstOrDefault();
        SpriteTextContainer imagen = spriteDefinitions.Where((a) => a.sprite == sprite).FirstOrDefault();

        cardText.text = text.text;
        cardResponseText.text = text.textResponse;
        image_Bg.sprite = imagen.Imagebg;
        StartCoroutine(HideCard());
    }

    IEnumerator HideCard()
    {
        switch (marca)
        {
            case Marca.ECO3:
                yield return new WaitForSeconds(5);
                break;  
            case Marca.Hikvision:
                yield return new WaitForSeconds(2);
                break;
            case Marca.HikvisionDark:
                yield return new WaitForSeconds(2);
                break;
            default:
                yield return new WaitForSeconds(2.5f);

                break;
        }
        showContentParent.SetActive(false);
        timeIsPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeIsPaused)
        {
            tiempoAntesTerminar += Time.deltaTime;
        }
        if ((tiempoAntesTerminar < Time.time || puntaje >= 6) && !timeIsPaused)
        {
            StartCoroutine(finalizar());
        }
        textoTiempo.text = (tiempoAntesTerminar - Time.time).ToString("00 seg");
    }

    IEnumerator finalizar()
    {
        cortinilla.SetActive(true);
        yield return new WaitForSeconds(2);
    }

    public void inicio() {
        SceneManager.LoadScene(0);

    }
}
