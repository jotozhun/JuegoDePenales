using System.Collections;
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
    public bool keeper;
    public bool stopTime;

    // Update is called once per frame
    [PunRPC]
    public void Start()
    {
        sec = seconds + 1;
    }

    void Update()
    {
        if (!stopTime)
        {
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
                //GameManager.instance.photonView.RPC("MarkGoalToPlayer", RpcTarget.AllBuffered, player.id + 1);
                GameManager.instance.photonView.RPC("MarkGoalMissedToPlayer", RpcTarget.AllBuffered);
                if (GameManager.instance.numberKicks % 2 == 0)
                {
                    GameManager.instance.photonView.RPC("decreaseKicksCount", RpcTarget.AllBuffered);
                    //GameManager.instance.decreaseKicksCount();
                }
                GameManager.instance.photonView.RPC("SwitchPositions", RpcTarget.AllBuffered);
            }
            else if (PhotonNetwork.IsMasterClient && keeper == false)
            {
                //Debug.Log("Not Master player");
                //GameManager.instance.photonView.RPC("MarkGoalToPlayer", RpcTarget.AllBuffered, player.id);
                GameManager.instance.photonView.RPC("MarkGoalMissedToPlayer", RpcTarget.AllBuffered);
                if (GameManager.instance.numberKicks % 2 == 0)
                {
                    GameManager.instance.photonView.RPC("decreaseKicksCount", RpcTarget.AllBuffered);
                    //GameManager.instance.decreaseKicksCount();
                }
                GameManager.instance.photonView.RPC("SwitchPositions", RpcTarget.AllBuffered);
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
