using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ball : MonoBehaviourPun
{
    public PlayerController player;

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (other.CompareTag("GoalBound"))
        {
            GameManager.instance.photonView.RPC("MarkGoalToPlayer", RpcTarget.AllBuffered, player.id - 1);
        }
        else if (other.CompareTag("MissedGoalBound"))
        {
            GameManager.instance.photonView.RPC("MarkGoalMissedToPlayer", RpcTarget.All);
        }
    }
}
