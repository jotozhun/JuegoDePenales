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

    public static Clock instance;

    private void Awake()
    {
        instance = this;
        secondsToKick = NetworkManager.instance.secondsToKick;
        started = true;
    }

    private void Start()
    {
        currTime = secondsToKick;
        currTimeToText = (int)currTime;
        timerText.text = (currTimeToText).ToString();
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
            GameManager.instance.photonView.RPC("MarkGoalMissedToPlayer", RpcTarget.All, PhotonNetwork.LocalPlayer);
        }
    }

    public void StopTimer()
    {
        started = false;
    }

    public void RestartTime()
    {
        currTime = secondsToKick;
        currTimeToText = secondsToKick;
        timerText.text = currTimeToText.ToString();
        started = true;
    }
}
