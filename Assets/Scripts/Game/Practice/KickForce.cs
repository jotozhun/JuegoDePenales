using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickForce : MonoBehaviour
{
    public PlayerControllerPractice player;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("entraTrigger");
        if (other.CompareTag("ShoeKick"))
        {
            Debug.Log("patear");
            Debug.Log("fuerzaBall" + GameUIPractice.instance.playerObject.ballForce);
            GameUIPractice.instance.playerObject.KickAnimation(GameUIPractice.instance.playerObject.ballForce);
        }
        /*if (other.CompareTag("Player"))
        {
            Debug.Log("patear");
            Debug.Log("fuerzaBall" + player.ballForce);
            player.KickAnimation(player.ballForce);
        }*/
    }
}
