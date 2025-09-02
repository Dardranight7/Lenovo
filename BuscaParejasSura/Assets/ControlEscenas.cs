using UnityEngine;
using UnityEngine.EventSystems;

public class ControlEscenas : MonoBehaviour, IPointerClickHandler
{

    [SerializeField] private AudioSource bocina;
    [SerializeField] private AudioClip sonido;
   

    private void Awake()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        bocina.PlayOneShot(sonido);
        CargadorEscenas.instancia.cargarEscena();
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
