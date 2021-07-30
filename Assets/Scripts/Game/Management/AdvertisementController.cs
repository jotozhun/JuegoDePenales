using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdvertisementType;
using TMPro;
using UnityEngine.UI;
using AccountModels;
using UnityEngine.SceneManagement;

public class AdvertisementController : MonoBehaviour
{
    [SerializeField]
    private Advertisement tipoPublicidad;
    [SerializeField]
    private TextMeshProUGUI descripcion;
    [SerializeField]
    private Image imagen;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    private List<PublicidadGame> publicidades;
    private int timeToChange;
    // Start is called before the first frame update

    public void Initialize(List<PublicidadGame> _publicidades, int _timeToChange)
    {
        publicidades = _publicidades;
        timeToChange = _timeToChange;
        StartCoroutine(CambiarPublicidad(timeToChange));
    }

    public void Initialize(List<PublicidadGame> _publicidades)
    {
        publicidades = _publicidades;
        CambiarPublicidadGol();
    }

    public void CambiarPublicidadGol()
    {
        if(publicidades.Count > 0)
        { 
            int randomPublicidad = Random.Range(0, publicidades.Count);
            List<ImageGame> tmpSprites = publicidades[randomPublicidad].imagenes;
            if(tmpSprites.Count > 0)
            { 
                int randomImagen = Random.Range(0, tmpSprites.Count);
                imagen.sprite = tmpSprites[randomImagen].sprite;
                string tmp_descripcion = publicidades[randomPublicidad].descripcion;
                descripcion.text = tmp_descripcion;
            }
        }
    }

    IEnumerator CambiarPublicidad(int _timeToChange)
    {
        if(publicidades.Count > 0) { 
            int randomPublicidad = Random.Range(0, publicidades.Count);
            List<ImageGame> tmpSprites = publicidades[randomPublicidad].imagenes;
            
            if(tmpSprites.Count > 0) {
                int randomImagen = Random.Range(0, tmpSprites.Count);
                spriteRenderer.sprite = tmpSprites[randomImagen].sprite;
                yield return new WaitForSeconds(_timeToChange);
                if (SceneManager.GetActiveScene().buildIndex == 2)
                    StartCoroutine(CambiarPublicidad(_timeToChange));
            }
        }
    }
}

