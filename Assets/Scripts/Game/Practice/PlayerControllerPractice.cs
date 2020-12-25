using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UIElements;

public class PlayerControllerPractice : MonoBehaviour
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
    //public Player photonPlayer;
    public bool hasToChange;
    //GoalKeeper settings
    private Vector3 playerModelStartPos;
    private Quaternion playerModelStartRot;
    private Vector3 firstCoverPos, lastCoverPos;
    private Rigidbody playerRig;
    public bool canCover;
    public bool canJump;
    //Ball components
    private BallPractice ballScript;
    private Rigidbody ballRigBody;
    private Vector3 firstpos, lastpos;
    private Vector3 startpos;
    private float starttime, endtime;
    private bool toKick; 
    private bool ballReturned = true;
    public bool playerCanCover;
    

    public void Initialize()
    {
        //Debug.Log("Player id: " + id);
        //GameUIPractice.instance.playerObject;
        //GameManager.instance.playersNickname[id - 1].text = photonPlayer.NickName;
        //GameUI.instance.playersName[id - 1].text = photonPlayer.NickName;
        GameUIPractice.instance.players[0] = this;
        GameManagerPractice.instance.spawnAsKicker();
        cam.gameObject.SetActive(true);
        Debug.Log("CamaraAtiva");

    }

    private void Start()
    {
        hasToChange = true;
        anim = playerModel.GetComponent<Animator>();
        ballScript = ball.GetComponent<BallPractice>();
        ballRigBody = ball.GetComponent<Rigidbody>();
        startpos = new Vector3(0.88f, -1.565f, 1.79f);
        playerModelStartPos = playerModel.transform.localPosition;
        playerModelStartRot = playerModel.transform.localRotation;
        playerRig = playerModel.GetComponent<Rigidbody>();
        Physics2D.gravity = new Vector2(0f, -7.51f);
    }
    

    private void Update()
    {
        Kick();
        /*if (!isGoalKeeper)
        {
            Kick();
        }
        if(isGoalKeeper && playerCanCover)
        {
            TryCover();
        }*/
    }
    // Playability
    public void Kick()
    {
        if (Input.GetMouseButtonDown(0) && ballReturned)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.name == "Soccer Ball" && hit.transform.GetComponent<BallPractice>().player.id == id)
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
            force.y = Mathf.Clamp(force.y, 0, 890);
            //force.z = 1350;
            force.z = 830;

            Vector3 ballForce = transform.forward * force.z + transform.right * force.x + transform.up * force.y;

            GameManagerPractice.instance.stopTime();
            //GameManagerPractice.instance.keeperCanCover(true);
            StartCoroutine(KickAnimation(ballForce));
            ballReturned = false;
            StartCoroutine(ReturnBall());
            //StartCoroutine(PlayerCoverBall());
        }
    }

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
        yield return new WaitForSeconds(3.2f);
        ball.transform.localPosition = startpos;
        ball.transform.localRotation = Quaternion.identity;
        ballRigBody.velocity = Vector3.zero;
        ballRigBody.angularVelocity = Vector3.zero;
        ballReturned = true;
        GameManagerPractice.instance.restartTime();

        if (!GameManagerPractice.instance.markGoal && !GameManagerPractice.instance.missGoal)
        {
            GameManagerPractice.instance.MarkGoalMissedToPlayer();
        }
        GameManagerPractice.instance.markGoal = false;
        GameManagerPractice.instance.missGoal = false;
        //checkRestartDecreaseKicks();
        //GameManager.instance.photonView.RPC("SwitchPositions", RpcTarget.AllBuffered);
    }

    IEnumerator KickAnimation(Vector3 ballForce)
    {
        
        anim.SetTrigger("kick");
        yield return new WaitForSeconds(0.55f);
        GameUIPractice.instance.kickSound.Play();
        ballRigBody.AddForce(ballForce);
    }

    IEnumerator PlayerCoverBall()
    {
        yield return new WaitForSeconds(1.1f);
        GameManagerPractice.instance.keeperCanCover(false);
    }

    public void checkRestartDecreaseKicks()
    {
        if (GameManagerPractice.instance.numberKicks % 2 == 0)
            GameManagerPractice.instance.decreaseKicksCount();
        bool zeroKicks = GameManagerPractice.instance.activateSwitch();
        if (zeroKicks == true)
            GameManagerPractice.instance.restartKicksCount();
        //NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Menu");
        
    }

    public void changeToGoalKeeperButtom()
    {
        GameManagerPractice.instance.spawnAsGoalKeeper(this);
    }
}
