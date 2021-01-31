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
        if (targetScipt.isTargetPractice)
        {
            if (other.CompareTag("Target"))
            {
                GameManagerPractice.instance.MarkTargetToPlayer();
            }
        }
        else
        {
            if (other.CompareTag("GoalBound"))
            {
                GameManagerPractice.instance.MarkGoalToPlayer();
            }
        }
        if (other.CompareTag("MissedGoalBound"))
        {
            GameManagerPractice.instance.MarkGoalMissedToPlayer();
        }
    }
}
