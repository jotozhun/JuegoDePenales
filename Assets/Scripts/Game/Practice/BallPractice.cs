using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallPractice : MonoBehaviour
{
    public PlayerControllerPractice player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GoalBound"))
        {
            GameManagerPractice.instance.MarkGoalToPlayer();
        }
        else if (other.CompareTag("MissedGoalBound"))
        {
            GameManagerPractice.instance.MarkGoalMissedToPlayer();
        }
    }
}
