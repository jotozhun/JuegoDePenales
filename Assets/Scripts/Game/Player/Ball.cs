using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ball : MonoBehaviourPun
{
    public PlayerController player;
    public bool touchedByGoalkeeper;
    public bool alreadyAGoalResult;
    GameUI gameUI;

    private void Start()
    {
        gameUI = GameUI.instance;
    }


    private void OnTriggerEnter(Collider other)
    {
        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["isGoalkeeper"])
          return;

        if (other.CompareTag("GoalBound") && !alreadyAGoalResult)
        {
            alreadyAGoalResult = true;
            GameManager.instance.photonView.RPC("MarkGoalToPlayer", RpcTarget.AllBuffered, player.photonPlayer);
        }
        else if (other.CompareTag("MissedGoalBound") && !alreadyAGoalResult)
        {
            alreadyAGoalResult = true;
            GameManager.instance.photonView.RPC("MarkGoalMissedToPlayer", RpcTarget.All, player.photonPlayer);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["isGoalkeeper"])
            return;

        if (collision.collider.CompareTag("Goalkeeper") && !touchedByGoalkeeper && !alreadyAGoalResult)
        {
            this.GetComponent<Rigidbody>().velocity = Vector3.zero;
            touchedByGoalkeeper = true;
            //GameManager.instance.photonView.RPC("MarkSavedGoalToPlayer", RpcTarget.All, player.photonPlayer);
        }
    }
}
