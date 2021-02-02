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
            if (other.CompareTag("ShoeKick"))
            {
                //Debug.Log("patear");
                //Debug.Log("fuerzaBall" + player.ballForce);
                player.KickAnimation(player.ballForce);
            }
            if (other.CompareTag("Target"))
            {
                player.ReturnBall();
            }
            else if (other.CompareTag("GoalBound"))
            {
                //Debug.Log("noObjetivoSIarco");
                targetScipt.intentosJugador++;
                targetScipt.actualizarUIText();
                player.ReturnBall();
            }
            else if (other.CompareTag("MissedGoalBound"))
            {
                //Debug.Log("noObjetivoSIfallo");
                targetScipt.intentosJugador++;
                targetScipt.actualizarUIText();
                player.ReturnBall();
            }
        }
        else
        {
            if (other.CompareTag("ShoeKick"))
            {
                //Debug.Log("patear2");
                //Debug.Log("fuerzaBall2" + player.ballForce);
                player.KickAnimation(player.ballForce);
            }
            if (other.CompareTag("GoalBound"))
            {
                GameManagerPractice.instance.MarkGoalToPlayer();
                player.ReturnBall();
            }
            else if (other.CompareTag("MissedGoalBound"))
            {
                GameManagerPractice.instance.MarkGoalMissedToPlayer();
                player.ReturnBall();
            }
        }

    }
}
