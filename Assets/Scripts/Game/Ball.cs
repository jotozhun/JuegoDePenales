using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public PlayerController player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GoalBound"))
        {
            GameManager.instance.photonView.RPC("MarkGoalToPlayer", Photon.Pun.RpcTarget.AllBuffered, player.id - 1);
        }
        else if (other.CompareTag("MissedGoalBound"))
        {
            GameManager.instance.photonView.RPC("MarkGoalMissedToPlayer", Photon.Pun.RpcTarget.All);
        }
    }
}
