﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [Header("Stats")]
    public int id;
    public bool isGoalKeeper;
    public bool isSpectator;
    //private bool changePosition;

    [Header("Player parts")]
    //public GameObject kickerModel;
    public Camera kicker_cam;
    public Camera goalkeeper_cam;
    public GameObject ball;
    public Animator kicker_anim;
    public Animator goalkeeper_anim;
    [Header("Player objects")]
    public GameObject kicker_obj;
    public GameObject goalkeeper_obj;
    public GameObject kicker_cam_obj;
    public GameObject goalkeeper_cam_obj;
    //Player components

    [HideInInspector]
    public Player photonPlayer;
    public bool hasToChange;
    //GoalKeeper settings
    private Vector3 firstCoverPos, lastCoverPos;
    //private Rigidbody playerRig;
    public bool canCover;
    public bool canJump;
    //Ball components
    [Header("Ball Components")]
    public Ball ballScript;
    public Rigidbody ballRigBody;
    private Vector3 firstpos, lastpos;
    private Vector3 startpos;
    private float starttime, endtime;
    private bool toKick; 
    private bool ballReturned = true;
    public bool playerCanCover;
    

    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;
        Debug.Log("Player id: " + id);
        GameUI.instance.players[id - 1] = this;

        this.transform.position = GameUI.instance.playerSpawn.position;
        //GameManager.instance.playersNickname[id - 1].text = NetworkManager.instance.userInfo.username;
        //GameManager.instance.playersNickname[id - 1].text = photonPlayer.NickName;
        //GameUI.instance.playersName[id - 1].text = photonPlayer.NickName;
        if (id == 1)
            GameManager.instance.spawnAsKicker(this);
        else if (id == 2)
            GameManager.instance.spawnAsGoalKeeper(this);

        if(photonView.IsMine)
        {
            kicker_cam.gameObject.SetActive(true);
        }
    }

    private void Start()
    {
        hasToChange = true;
        //anim = kickerModel.GetComponent<Animator>();
        //ballScript = ball.GetComponent<Ball>();
        //ballRigBody = ball.GetComponent<Rigidbody>();
        startpos = new Vector3(0.4403152f, -0.8204808f, -2.119095f);
        //playerRig = kickerModel.GetComponent<Rigidbody>();
        Physics2D.gravity = new Vector2(0f, -6.71f);
    }
    

    private void Update()
    {
        if(!isGoalKeeper)
        {
            photonView.RPC("Kick", RpcTarget.All);
        }
        if(isGoalKeeper && playerCanCover)
        {
            photonView.RPC("TryCover", RpcTarget.All);
        }
    }

    public void ChangeRol(bool isGoalkeeper)
    {
        goalkeeper_obj.SetActive(isGoalkeeper);

        kicker_obj.SetActive(!isGoalkeeper);
        
        ball.SetActive(!isGoalkeeper);

        //Cams
        if (photonView.IsMine)
        {
            kicker_cam_obj.SetActive(!isGoalkeeper);
            goalkeeper_cam_obj.SetActive(isGoalkeeper);
        }

    }
    // Playability
    [PunRPC]
    public void Kick()
    {
        if (Input.GetMouseButtonDown(0) && ballReturned)
        {
            Ray ray = kicker_cam.ScreenPointToRay(Input.mousePosition);
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
            //Vector3 force = (distance / ((endtime - starttime) / 0.3f));
            Vector3 force = new Vector3((distance.x / ((endtime - starttime) / 0.33f)), (distance.y / ((endtime - starttime) / 0.26f)), (distance.z / ((endtime - starttime) / 0.4f)));
            toKick = false;
            /*
            if (distance.x < 0 && distance.y > 300)
            {
                //anim.SetTrigger("jumpLeft");
                WindSpeed = new Vector3(0, 0, -600);

                //Physics2D.gravity = new Vector2(15f, -7.51f);
                Debug.Log("jumpleft");
                Debug.Log(WindSpeed);
            }
            else if (distance.x > 0 && distance.y > 300)
            {
                //anim.SetTrigger("jumpRight");
                WindSpeed = new Vector3(0, 0, 600);
                //Physics2D.gravity = new Vector2(-15f, -7.51f);
                Debug.Log("jumpRight");
                Debug.Log(WindSpeed);
            }
            else if (distance.x < 0 && distance.y < 300)
                WindSpeed = new Vector3(0, 0, -600);
            else if (distance.x > 0 && distance.y < 300)
                WindSpeed = new Vector3(0, 0, 600);

            */
            //force.x = Mathf.Clamp(force.x, -880, 880);
            //force.y = Mathf.Clamp(force.y, 0, 890);
            //force.z = 830;

            force.y = Mathf.Clamp(force.y, 0, 650);
            force.z = Mathf.Clamp(force.z + 120, 520, 700);

            Vector3 ballForce = transform.forward * force.z + transform.right * force.x + transform.up * force.y;

            GameManager.instance.photonView.RPC("stopTime", RpcTarget.AllBuffered);
            GameManager.instance.photonView.RPC("keeperCanCover", RpcTarget.AllBuffered, true);
            StartCoroutine(KickAnimation(ballForce));
            ballReturned = false;
            StartCoroutine(ReturnBall());
            StartCoroutine(PlayerCoverBall());
        }
    }

    [PunRPC]
    public void TryCover()
    {
        if (Input.GetMouseButtonDown(0) && canCover)
        {
            firstCoverPos = Input.mousePosition;
            canJump = true;

        }
        if (Input.GetMouseButtonUp(0) && canJump)
        {
            canCover = false;
            canJump = false;
            lastCoverPos = Input.mousePosition;

            Vector3 distance = lastCoverPos - firstCoverPos;
            StartCoroutine(GetPlayerSet(distance));
         }
        
    }

    IEnumerator GetPlayerSet(Vector3 distance)
    {
        if (distance.x < 0 && distance.y > 0)
            kicker_anim.SetTrigger("jumpLeft");
        else if (distance.x > 0 && distance.y > 0)
            kicker_anim.SetTrigger("jumpRight");
        else if (distance.x < 0 && distance.y < 0)
            kicker_anim.SetTrigger("throwLeft");
        else if (distance.x > 0 && distance.y < 0)
            kicker_anim.SetTrigger("throwRight");
        yield return new WaitForSeconds(0.5f);
        if (this.isGoalKeeper)
            canCover = true;
        //playerRig.velocity = Vector3.zero;
        //playerModel.transform.localPosition = playerModelStartPos;
        //playerModel.transform.localRotation = playerModelStartRot;
    }
    //Testing coroutines
    IEnumerator ReturnBall()
    {
        yield return new WaitForSeconds(3.2f);
        ball.transform.localPosition = startpos;
        ball.transform.localRotation = Quaternion.identity;
        ballRigBody.velocity = Vector3.zero;
        ballRigBody.angularVelocity = Vector3.zero;
        ballReturned = true;
        GameManager.instance.photonView.RPC("restartTime", RpcTarget.AllBuffered);
        //Debug.Log(GameManager.instance.numberKicks);
        //GameManager.instance.restartTime();
        if (!GameManager.instance.markGoal && !GameManager.instance.missGoal)
        {
            GameManager.instance.photonView.RPC("MarkGoalMissedToPlayer", RpcTarget.AllBuffered);
        }
        GameManager.instance.markGoal = false;
        GameManager.instance.missGoal = false;
        //bool change = GameManager.instance.activateSwitch();
        /*if (change == true)
        {
            GameManager.instance.photonView.RPC("SwitchPositions", RpcTarget.AllBuffered);
        }*/
        /*if (GameManager.instance.numberKicks%2 == 0)
        {
            //Debug.Log("DecrementarKicks");
            GameManager.instance.photonView.RPC("decreaseKicksCount", RpcTarget.AllBuffered);
            //GameManager.instance.decreaseKicksCount();
        }
        bool change = GameManager.instance.activateSwitch();
        bool draw = GameManager.instance.DrawGame();
        if (change == true && draw == true)
        {
            GameManager.instance.photonView.RPC("restartKicksCount", RpcTarget.AllBuffered);
        }*/
        checkRestartDecreaseKicks();

        GameManager.instance.photonView.RPC("SwitchPositions", RpcTarget.AllBuffered);
    }

    IEnumerator KickAnimation(Vector3 ballForce)
    {
        
        kicker_anim.SetTrigger("kick");
        yield return new WaitForSeconds(0.55f);
        GameUI.instance.kickSound.Play();
        ballRigBody.AddForce(ballForce);
        yield return new WaitForSeconds(0.50f);
    }

    IEnumerator PlayerCoverBall()
    {
        yield return new WaitForSeconds(1.1f);
        GameManager.instance.photonView.RPC("keeperCanCover", RpcTarget.AllBuffered, false);
    }

    [PunRPC]
    public void checkRestartDecreaseKicks()
    {
        if (GameManager.instance.numberKicks % 2 == 0) {
            GameManager.instance.photonView.RPC("decreaseKicksCount", RpcTarget.AllBuffered);
            bool winGame = GameManager.instance.WinGame();
            if (winGame == true)
            {
                //PhotonNetwork.LeaveRoom();
                //NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Menu");
                //GameManager.instance.playersInGame--;
                StartCoroutine(LeaveAndLoad());
            }
        }
        bool zeroKicks = GameManager.instance.activateSwitch();
        bool draw = GameManager.instance.DrawGame();
        if (zeroKicks == true && draw == true)
            GameManager.instance.photonView.RPC("restartKicksCount", RpcTarget.AllBuffered);
        else if (zeroKicks == true && draw == false)
            StartCoroutine(LeaveAndLoad()); 




    }

    [PunRPC]
    IEnumerator LeaveAndLoad()
    {
        //yield return new WaitForSeconds(0.01f);
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.InRoom)
            yield return null;
        //NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Menu");
        PhotonNetwork.LoadLevel("Menu");
        /*Debug.Log("Salir");
        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom)
            yield return null;
        SceneManager.LoadScene("Menu");*/
        //NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.AllBuffered, "Menu");
    }
    public void salirGame()
    {
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
        //PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Menu");
    }
}
