using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UIElements;

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
    public Animator anim;
    [HideInInspector]
    public Player photonPlayer;
    public bool hasToChange;
    //GoalKeeper settings
    private Vector3 playerModelStartPos;
    private Quaternion playerModelStartRot;
    private Vector3 firstCoverPos, lastCoverPos;
    private Rigidbody playerRig;
    public bool canCover;
    public bool canJump;
    //Ball components
    private Ball ballScript;
    private Rigidbody ballRigBody;
    private Vector3 firstpos, lastpos;
    private Vector3 startpos;
    private float starttime, endtime;
    private bool toKick; 
    private bool ballReturned = true;
    private Vector3 WindSpeed;
    public bool playerCanCover;

    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;
        Debug.Log("Player id: " + id);
        GameUI.instance.players[id - 1] = this;
        GameManager.instance.playersNickname[id - 1].text = photonPlayer.NickName;

        if (id == 1)
            GameManager.instance.spawnAsKicker(this);
        else if (id == 2)
            GameManager.instance.spawnAsGoalKeeper(this);

        if(photonView.IsMine)
        {
            cam.gameObject.SetActive(true);
        }
    }

    private void Start()
    {
        hasToChange = true;
        anim = playerModel.GetComponent<Animator>();
        ballScript = ball.GetComponent<Ball>();
        ballRigBody = ball.GetComponent<Rigidbody>();
        startpos = new Vector3(0.88f, -1.565f, 1.79f);
        playerModelStartPos = playerModel.transform.localPosition;
        playerModelStartRot = playerModel.transform.localRotation;
        playerRig = playerModel.GetComponent<Rigidbody>();
        Physics2D.gravity = new Vector2(0f, -7.51f);
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
    // Playability
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
            force.x = Mathf.Clamp(force.x, -880, 880);
            force.y = Mathf.Clamp(force.y, 0, 900);
            //force.z = 1350;
            force.z = 830;

            Vector3 ballForce = transform.forward * force.z + transform.right * force.x + transform.up * force.y;

            GameManager.instance.photonView.RPC("stopTime", RpcTarget.AllBuffered);
            GameManager.instance.photonView.RPC("keeperCover", RpcTarget.AllBuffered, true);
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
            anim.SetTrigger("jumpLeft");
        else if (distance.x > 0 && distance.y > 0)
            anim.SetTrigger("jumpRight");
        else if (distance.x < 0 && distance.y < 0)
            anim.SetTrigger("throwLeft");
        else if (distance.x > 0 && distance.y < 0)
            anim.SetTrigger("throwRight");
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
        yield return new WaitForSeconds(3);
        ball.transform.localPosition = startpos;
        ball.transform.localRotation = Quaternion.identity;
        ballRigBody.velocity = Vector3.zero;
        ballRigBody.angularVelocity = Vector3.zero;
        ballReturned = true;
        GameManager.instance.photonView.RPC("restartTime", RpcTarget.AllBuffered);
        //GameManager.instance.restartTime();
        bool change = GameManager.instance.activateSwitch();
        if (change == true)
        {
            GameManager.instance.photonView.RPC("SwitchPositions", RpcTarget.AllBuffered);
        }
        WindSpeed = new Vector3(0, 0, 0);
    }

    IEnumerator KickAnimation(Vector3 ballForce)
    {
        
        anim.SetTrigger("kick");
        yield return new WaitForSeconds(0.55f);
        GameUI.instance.kickSound.Play();
        ballRigBody.AddForce(ballForce);
        yield return new WaitForSeconds(0.50f);
        Debug.Log(WindSpeed);
        ballRigBody.AddRelativeForce(WindSpeed);
    }

    IEnumerator PlayerCoverBall()
    {
        yield return new WaitForSeconds(1.5f);
        GameManager.instance.photonView.RPC("keeperCover", RpcTarget.AllBuffered, false);
    }
}
