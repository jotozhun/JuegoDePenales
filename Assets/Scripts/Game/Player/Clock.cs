using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class Clock : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI timerText;
    public int secondsToKick;
    public int currTimeToText;
    public float currTime;
    public bool started;
    Player photonPlayer;
    GameUI gameUI;

    public static Clock instance;

    private void Awake()
    {
        instance = this;
        if ((bool)PhotonNetwork.CurrentRoom.CustomProperties["isTorneo"])
            secondsToKick = NetworkManager.instance.userLogin.duelo_agendado.tiempo_patear_segundos;
        else
            secondsToKick = NetworkManager.instance.secondsToKick;
        photonPlayer = PhotonNetwork.LocalPlayer;
    }
    
    private void Start()
    {
        currTime = secondsToKick;
        currTimeToText = (int)currTime;
        timerText.text = (currTimeToText).ToString();
        gameUI = GameUI.instance;
    }
    
    private void Update()
    {
        if (started && currTime > 0.00)
        {
            currTime -= Time.deltaTime;
            currTimeToText = (int)currTime;
            timerText.text = currTimeToText.ToString();
        } else if (currTime <= 0.00 && started)
        {
            StopTimer();
            if(!(bool)photonPlayer.CustomProperties["isGoalkeeper"])
            {
                GameManager.instance.photonView.RPC("MarkGoalMissedToPlayer", RpcTarget.All, photonPlayer);//PhotonNetwork.LocalPlayer);
                gameUI.players[photonPlayer.ActorNumber - 1].FailedGoalByClock();
            }
        }
    }
    
    [PunRPC]
    public void StopTimer()
    {
        started = false;
    }

    [PunRPC]
    public void RestartTime()
    {
        currTime = secondsToKick;
        currTimeToText = secondsToKick;
        timerText.text = currTimeToText.ToString();
        started = true;
    }
}
