using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UIElements;

public class CountKicks : MonoBehaviourPunCallbacks
{
    [Header("Kicks")]
    public int numberKicksStatic;
    public int numKicks;
    public TextMeshProUGUI numberKicksGUI;
    // Start is called before the first frame update
    [PunRPC]
    public void Start()
    {
        numKicks = numberKicksStatic;
        string numberKicksStart;
        numberKicksStart = numKicks.ToString();
        numberKicksGUI.text = numberKicksStart;
    }

    [PunRPC]
    public void DecreaseKicks() {
        numKicks -= 1;
        string numberKicks;
        numberKicks = numKicks.ToString();
        numberKicksGUI.text = numberKicks;
    }

    [PunRPC]
    public void RestartKicks()
    {
        numKicks = numberKicksStatic;
        string numberKicks;
        numberKicks = numberKicksStatic.ToString();
        numberKicksGUI.text = numberKicks;
    }
}
