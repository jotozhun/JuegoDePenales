﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [Header("Stats")]
    public int id;
    public bool isGoalKeeper;
    public bool isSpectator;
    private bool changePosition;

    [Header("Player parts")]
    public GameObject playerModel;
    public Camera cam;
    public GameObject ball;

    //Player components
    private Animator anim;
    [HideInInspector]
    public Player photonPlayer;
    public bool hasToChange;
    //Ball components
    private Ball ballScript;
    private Rigidbody ballRigBody;
    private Vector3 firstpos, lastpos;
    private Vector3 startpos;
    private float starttime, endtime;
    private bool toKick; 
    private bool ballReturned = true;

    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;
        Debug.Log("Player id: " + id);
        GameManager.instance.players[id - 1] = this;

        // give the kicker the ball
        if(PhotonNetwork.IsMasterClient)
        {
            GameManager.instance.spawnAsKicker(this);
            GameManager.instance.player1Nickname.text = photonPlayer.NickName;
        }else
        {
            GameManager.instance.spawnAsGoalKeeper(this);
            GameManager.instance.player2Nickname.text = photonPlayer.NickName;
        }

        if(!photonView.IsMine)
        {
            cam.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        anim = playerModel.GetComponent<Animator>();
        ballScript = ball.GetComponent<Ball>();
        ballRigBody = ball.GetComponent<Rigidbody>();
        startpos = ball.transform.position;
    }

    private void Update()
    {
        if(!isGoalKeeper)
        {
            photonView.RPC("Kick", RpcTarget.AllBuffered);
        }
        if(isGoalKeeper)
        {

        }
    }

    [PunRPC]
    public void Kick()
    {
        if (Input.GetMouseButtonDown(0) && ballReturned)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.name == "Soccer Ball" && hit.transform.GetComponent<Ball>().player.id == id)
                {
                    starttime = Time.time;
                    firstpos = Input.mousePosition;
                    toKick = true;
                }
            }
        }
        if (Input.GetMouseButtonUp(0) && toKick)
        {
            endtime = Time.time;
            lastpos = Input.mousePosition;

            Vector3 distance = lastpos - firstpos;
            distance.z = distance.magnitude;
            Vector3 force = (distance / ((endtime - starttime) / 0.3f));
            toKick = false;

            force.x = Mathf.Clamp(force.x, -800, 800);
            force.y = Mathf.Clamp(force.y, 0, 850);
            force.z = 1350;

            Vector3 ballForce = transform.forward * force.z + transform.right * force.x + transform.up * force.y;
            
            StartCoroutine(KickAnimation(ballForce));
            ballReturned = false;
            StartCoroutine(ReturnBall());

        }
    }

    //Testing coroutines
    IEnumerator ReturnBall()
    {
        yield return new WaitForSeconds(3);
        ball.transform.position = startpos;
        ball.transform.localRotation = Quaternion.identity;
        ballRigBody.velocity = Vector3.zero;
        ballRigBody.angularVelocity = Vector3.zero;
        ballReturned = true;
        //GameManager.instance.photonView.RPC("SwitchPositions", RpcTarget.AllBuffered);
        this.hasToChange = true;
        GameManager.instance.SwitchPositions();
    }

    IEnumerator KickAnimation(Vector3 ballForce)
    {
        anim.SetTrigger("kick");
        yield return new WaitForSeconds(0.55f);
        GameManager.instance.kickSound.Play();
        ballRigBody.AddForce(ballForce);
    }
}
