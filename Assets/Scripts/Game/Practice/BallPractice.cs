using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallPractice : MonoBehaviour
{
    public PlayerControllerPractice player;
    private TargetPractice targetScipt;

    private void Start()
    {
        targetScipt = GameUIPractice.instance.target.GetComponent<TargetPractice>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ShoeKick"))
        {
            Debug.Log("patear");
            Debug.Log("fuerzaBall" + player.ballForce);
            player.KickAnimation(player.ballForce);
        }
        if (targetScipt.isTargetPractice)
        {
            if (other.CompareTag("Target"))
            {
                GameManagerPractice.instance.MarkTargetToPlayer();
                player.ReturnBall();
            }
            else if (other.CompareTag("GoalBound"))
            {
                player.ReturnBall();
            }
        }
        else
        {
            if (other.CompareTag("GoalBound"))
            {
                GameManagerPractice.instance.MarkGoalToPlayer();
                player.ReturnBall();
            }
        }
        if (other.CompareTag("MissedGoalBound"))
        {
            GameManagerPractice.instance.MarkGoalMissedToPlayer();
            player.ReturnBall();
        }
    }
}
