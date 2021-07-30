using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AccountModels;
public class TestingPublicidad : MonoBehaviour
{
    public Image[] horizontales;
    public Image[] verticales;
    public Image[] gol;
    public PublicidadesGame publicidadesGame;
    public static TestingPublicidad instance;
    private void Awake()
    {
        instance = this;
        publicidadesGame = NetworkManager.instance.publicidadesGame;
    }
    public void Initialize()
    {
        horizontales[0].sprite = publicidadesGame.horizontal[0].imagenes[0].sprite;
        horizontales[1].sprite = publicidadesGame.horizontal[1].imagenes[0].sprite;
        horizontales[2].sprite = publicidadesGame.horizontal[2].imagenes[0].sprite;

        verticales[0].sprite = publicidadesGame.vertical[0].imagenes[0].sprite;

        gol[0].sprite = publicidadesGame.gol[0].imagenes[0].sprite;
    }
}
