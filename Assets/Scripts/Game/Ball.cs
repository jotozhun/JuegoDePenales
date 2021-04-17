using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ball : MonoBehaviourPun
{
    public PlayerController player;
    public bool doneMissedGoal;

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        if (other.CompareTag("GoalBound"))
        {
            //GameManager.instance.photonView.RPC("MarkGoalToPlayer", RpcTarget.AllBuffered, player.id - 1);
            GameManager.instance.photonView.RPC("MarkGoalToPlayer", RpcTarget.AllBuffered, player.photonPlayer);
        }
        else if (other.CompareTag("MissedGoalBound"))
        {
            //GameManager.instance.photonView.RPC("MarkGoalMissedToPlayer", RpcTarget.All, player.id - 1);
            GameManager.instance.photonView.RPC("MarkGoalMissedToPlayer", RpcTarget.All, player.photonPlayer);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if(collision.collider.CompareTag("Goalkeeper") && !doneMissedGoal)
        {
            this.GetComponent<Rigidbody>().velocity = Vector3.zero;
            doneMissedGoal = true;
            GameManager.instance.photonView.RPC("MarkSavedGoalToPlayer", RpcTarget.All, player.photonPlayer);
        }
    }
}
