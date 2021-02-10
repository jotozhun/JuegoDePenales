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

        }
    }
}
