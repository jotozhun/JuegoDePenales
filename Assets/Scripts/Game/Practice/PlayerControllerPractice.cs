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
    public GameObject kickCollider;
    //public BoxCollider kickCo;

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
    public Vector3 ballForce;
    //KickCollider
    private KickForce kickScript;

    public void Initialize()
    {
        //Debug.Log("Player id: " + id);
        //GameUIPractice.instance.playerObject;
        //GameManager.instance.playersNickname[id - 1].text = photonPlayer.NickName;
        //GameUI.instance.playersName[id - 1].text = photonPlayer.NickName;
        GameUIPractice.instance.players[0] = this;
        this.transform.position = GameUIPractice.instance.kickerSpawn.position;
        GameManagerPractice.instance.spawnAsKicker(this);
        cam.gameObject.SetActive(true);
    }

    private void Start()
    {
        hasToChange = true;
        anim = playerModel.GetComponent<Animator>();
        ballScript = ball.GetComponent<BallPractice>();
        ballRigBody = ball.GetComponent<Rigidbody>();
        //kickCo = kickCollider.GetComponent<BoxCollider>();
        //startpos = new Vector3(0.88f, -1.565f, 1.79f);
        startpos = new Vector3(1.029999f, 0.21f, 3.519997f);
        playerModelStartPos = playerModel.transform.localPosition;
        playerModelStartRot = playerModel.transform.localRotation;
        playerRig = playerModel.GetComponent<Rigidbody>();
        Physics2D.gravity = new Vector2(0f, -6.71f);
        ballForce = new Vector3();
        kickScript = kickCollider.GetComponent<KickForce>();
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
                if (hit.transform.name == "Soccer Ball Practice" )
                {
                    //Debug.Log("soceeer");
                    starttime = Time.time;
                    firstpos = Input.mousePosition;
                    toKick = true;
                }
            }
            
        }
        if (Input.GetMouseButtonUp(0) && toKick)
        {
            //Debug.Log("terminar");
            endtime = Time.time;
            lastpos = Input.mousePosition;
            //Debug.Log("last" + lastpos);
            Vector3 distance = lastpos - firstpos;
            distance.z = distance.magnitude;
            //Vector3 force = new Vector3((distance.x / ((endtime - starttime) / 0.33f)), (distance.y / ((endtime - starttime) / 0.26f)), (distance.z / ((endtime - starttime) / 0.4f)));
            Vector3 force = new Vector3((distance.x / ((endtime - starttime) / 0.39f)), (distance.y / ((endtime - starttime) / 0.35f)), (distance.z / ((endtime - starttime) / 0.37f)));
            toKick = false;
    /*        Vector3 mousePos = Input.mousePosition;
            mousePos.z = cam.nearClipPlane;
            Vector3 worldPosition = cam.ScreenToWorldPoint(mousePos);
            Debug.Log("WorldpOs" + worldPosition);
            */

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
/*
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.transform.name == "KickLimit")
                {
                    Vector3 kikkkk = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                    Debug.Log("kik"+kikkkk);                    
                }

            }
*/
            //GameUIPractice.instance.kickCollider.SetActive(true);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.name == "KickLimit")
                {
                    //Vector3 kickLimitPosition = new Vector3(hit.point.x, hit.collider.transform.position.y, hit.collider.transform.position.z);
                    //Vector3 kickLimitPosition = new Vector3(lastpos.x, lastpos.y ,hit.collider.transform.position.z);
                    //Debug.Log("kick"+kickLimitPosition);
                    //Debug.Log("dd" + force);;
                    //force.y = Mathf.Clamp(force.y, 0, 650);
                    //force.z = Mathf.Clamp(force.z+120, 520, 700);
                    force.y = Mathf.Clamp(force.y, 0, 650);
                    force.z = Mathf.Clamp(force.z + 120, 540, 700);
                    ballForce = transform.forward * force.z + transform.right * force.x + transform.up * force.y;
                    //kickCollider.SetActive(true);
                    //Debug.Log("activar");
                    //Debug.Log("playercontrollerpractice"+ballForce);
                    //Debug.Log("real"+ballForce);
                    //GameManagerPractice.instance.stopTime();
                    //GameManagerPractice.instance.keeperCanCover(true);
                    //StartCoroutine(KickAnimation(ballForce));
                    
                    anim.SetTrigger("KickNow");
                    ballReturned = false;
                    //StartCoroutine(ReturnBall());
                }

            }
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
    public void ReturnBall()
    {
        //yield return new WaitForSeconds(4.2f);
        ball.transform.localPosition = startpos;
        ball.transform.localRotation = Quaternion.identity;
        ballRigBody.velocity = Vector3.zero;
        ballRigBody.angularVelocity = Vector3.zero;
        ballReturned = true;
        GameManagerPractice.instance.restartTime();
        //GameUIPractice.instance.kickCollider.SetActive(false);
        kickCollider.SetActive(false);
        /*if (!GameManagerPractice.instance.markGoal && !GameManagerPractice.instance.missGoal)
        {
            GameManagerPractice.instance.MarkGoalMissedToPlayer();
        }*/
        GameManagerPractice.instance.markGoal = false;
        GameManagerPractice.instance.missGoal = false;
        //checkRestartDecreaseKicks();
        //GameManager.instance.photonView.RPC("SwitchPositions", RpcTarget.AllBuffered);
    }

    public void KickAnimation(Vector3 ballForce)
    {
        //yield return new WaitForSeconds(4f);
        GameUIPractice.instance.kickSound.Play();
        ballRigBody.AddForce(ballForce);
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ShoeKick"))
        {
            //GameUIPractice.instance.kickSound.Play();
            ballRigBody.AddForce(ballForce);
        }
    }
    */
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
