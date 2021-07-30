using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AccountModels;
using TMPro;

public class GoalScreen : MonoBehaviour
{
    public Publicidad publicidad;
    public TextMeshProUGUI description;
    public Image publicity;
    // Start is called before the first frame update
    private void Awake()
    {
        publicidad = NetworkManager.instance.publicidades.publicidades[0];
    }
    void Start()
    {
        description.text = publicidad.descripcion;
        //publicity.sprite = publicidad.imagenes[0].sprite;
    }

}
