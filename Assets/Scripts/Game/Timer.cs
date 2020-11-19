﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UIElements;

public class Timer : MonoBehaviourPunCallbacks
{
    public float seconds;
    public float sec;
    public TextMeshProUGUI textTime;
    public PlayerController player;
    //public PlayerController[] players;
    public bool keeper;
    public bool stopTime;
    /*[Header("ChangeScoreForTime")]
    public GameObject ball;
    private Ball ballScript;*/
    //public TextMeshProUGUI numberScoredGUI;
    //public GameObject score;
    //private ScoreTrigger scoreScript;
    // Update is called once per frame
    [PunRPC]
    public void Start()
    {
        sec = seconds + 1;
        //ballScript = ball.GetComponent<Ball>();
        //scoreScript = score.GetComponent<ScoreTrigger>();
        
        //players = new PlayerController[PhotonNetwork.PlayerList.Length];
    }

    void Update()
    {
        //players = GameManager.instance.players;
        if(!stopTime){
            sec -= Time.deltaTime;
            int secondsInt = (int)sec;
            string time;
            time = secondsInt.ToString();
            textTime.text = time;
        }
        

        if (sec < 0.0)
        {

                if (PhotonNetwork.IsMasterClient && keeper == true)
                {
                    GameManager.instance.photonView.RPC("MarkGoalToPlayer", RpcTarget.AllBuffered, player.id + 1);
                    //photonView.RPC("SpawnPlayers", RpcTarget.All);
                }
                else if (PhotonNetwork.IsMasterClient && keeper == false)
                {
                    Debug.Log("Not Master player");
                    GameManager.instance.photonView.RPC("MarkGoalToPlayer", RpcTarget.AllBuffered, player.id);
                }

            bool change = GameManager.instance.activateSwitch();
            if (change == true)
            {
                GameManager.instance.photonView.RPC("SwitchPositions", RpcTarget.AllBuffered);
            }
            R();
        }
    }

    [PunRPC]
    public void R()
    {
        sec = seconds + 1;
    }

    [PunRPC]
    public void StopTime()
    {
        //Debug.Log("StopTime");
        stopTime = true;
        //enabled = false;
    }

    [PunRPC]
    public void StartTime()
    {
        stopTime = false;
        //enabled = true;
    }
}
