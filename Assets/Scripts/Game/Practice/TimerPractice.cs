using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UIElements;

public class TimerPractice : MonoBehaviour
{
    public float seconds;
    public float sec;
    public TextMeshProUGUI textTime;
    //private PlayerControllerPractice playerScript;
    public bool keeper;
    public bool stopTime;

    // Update is called once per frame
    public void Start()
    {
        sec = seconds + 1;
        //playerScript = GameUI.instance.playerObject.GetComponent<PlayerControllerPractice>();
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
            /*
            if (PhotonNetwork.IsMasterClient && keeper == true)
            {
                //GameManager.instance.photonView.RPC("MarkGoalToPlayer", RpcTarget.AllBuffered, player.id + 1);
                GameManager.instance.photonView.RPC("MarkGoalMissedToPlayer", RpcTarget.AllBuffered);
                playerScript.checkRestartDecreaseKicks();
            }
            else if (PhotonNetwork.IsMasterClient && keeper == false)
            {
                //GameManager.instance.photonView.RPC("MarkGoalToPlayer", RpcTarget.AllBuffered, player.id);
                GameManager.instance.photonView.RPC("MarkGoalMissedToPlayer", RpcTarget.AllBuffered);
                playerScript.checkRestartDecreaseKicks();
            }*/
            R();
        }
    }

    public void R()
    {
        sec = seconds + 1;
    }

    public void StopTime()
    {
        stopTime = true;
    }

    public void StartTime()
    {
        stopTime = false;
    }

}
