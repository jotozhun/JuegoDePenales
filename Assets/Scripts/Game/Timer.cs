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
    }

    void Update()
    {
        sec -= Time.deltaTime;
        int secondsInt = (int)sec;
        string time;
        time = secondsInt.ToString();
        textTime.text = time;

        if (sec < 0.0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GameManager.instance.photonView.RPC("MarkGoalToPlayer", RpcTarget.All, player.id + 1);
                //photonView.RPC("SpawnPlayers", RpcTarget.All);
            }
            
            /*scoreScript.numGo -= 1;
            string numberSco;
            numberSco = scoreScript.numGo.ToString();
            numberScoredGUI.text = numberSco;
            if (scoreScript.numGo == 0)
            {
                //Debug.Log("Restart Kicks");
                scoreScript.R();
                StartCoroutine(scoreScript.Wait());
            }
            //Debug.Log("0seconds");
            */
            R();
        }
    }

    [PunRPC]
    public void R()
    {
        sec = seconds + 1;
    }

    public void StopTime()
    {
        enabled = false;
    }

    public void StartTime()
    {
        enabled = true;
    }
}
