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
    private PlayerController playerScript;
    public bool keeper;
    public bool stopTime;

    // Update is called once per frame
    [PunRPC]
    public void Start()
    {
        sec = seconds + 1;
        playerScript = GameUI.instance.playerObject.GetComponent<PlayerController>();
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


        if (sec < 0.0 && PhotonNetwork.IsMasterClient)
        {
            if (keeper == true)
            {
                GameManager.instance.photonView.RPC("MarkGoalMissedToPlayer", RpcTarget.AllBuffered, playerScript.id - 1);
            }
            else if (keeper == false)
            {
                GameManager.instance.photonView.RPC("MarkGoalMissedToPlayer", RpcTarget.AllBuffered, playerScript.id - 1);
            }
            GameManager.instance.photonView.RPC("SwitchPositions", RpcTarget.AllBuffered);
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
        stopTime = true;
    }

    [PunRPC]
    public void StartTime()
    {
        stopTime = false;
    }

}
