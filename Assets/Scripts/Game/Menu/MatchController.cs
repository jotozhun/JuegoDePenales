using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using AccountModels;
using System;

public class MatchController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI fecha;
    [SerializeField]
    private TextMeshProUGUI rivalUser;
    [SerializeField]
    private TextMeshProUGUI myGoals;
    [SerializeField]
    private TextMeshProUGUI rivalGoals;
    // Start is called before the first frame update
    [SerializeField]
    private Image background;
    [SerializeField]
    private Sprite winSprite;
    [SerializeField]
    private Sprite loseSprite;
    public void Initialize(Duelo duelo, bool isWin)
    {
        if(isWin)
        {
            background.sprite = winSprite;
            myGoals.text = duelo.goles_ganador.ToString();
            rivalUser.text = duelo.perdedor.username;
            rivalGoals.text = duelo.goles_perdedor.ToString();
        }
        else
        {
            background.sprite = loseSprite;
            myGoals.text = duelo.goles_perdedor.ToString();
            rivalUser.text = duelo.ganador.username;
            rivalGoals.text = duelo.goles_ganador.ToString();
        }
        //fecha.text = duelo.fecha;
        DateTime tmpFecha = DateTime.Parse(duelo.fecha);
        fecha.text = tmpFecha.ToString("yyyy-MM-dd") + "\n" + tmpFecha.ToString("hh:mm tt");
    }
}
