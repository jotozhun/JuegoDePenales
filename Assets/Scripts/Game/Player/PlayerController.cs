using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [HideInInspector]
    private GameManager gameManager;
    private GameUI gameUI;

    [Header("Stats")]
    public int id;
    public bool isGoalKeeper;
    public bool isSpectator;

    [Header("Player parts")]
    public Camera kicker_cam;
    public Camera goalkeeper_cam;
    public GameObject ball;

    [Header("Haircuts")]
    public GameObject[] kicker_haircuts;

    //public SphereCollider touch_ball_col;
    [Header("Animators")]
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

    //Cam settings
    private Quaternion camStartRotK;
    private bool isCamFollowingK;
    //GoalKeeper settings
    private Vector3 firstCoverPos, lastCoverPos;
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
    public bool ballReturned = true;
    public bool playerCanCover;
    public CapsuleCollider triggerForKick;

    public float forceCoeficient;
    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;
        Debug.Log("Player id: " + id);
        gameUI.players[id - 1] = this;

        this.transform.position = gameUI.playerSpawn.position;
        gameManager.playerEmblemas[id - 1].sprite = gameUI.playerEmblemas[(int)photonPlayer.CustomProperties["EmblemaIndex"]];
        SelectKickerHaircut((int)photonPlayer.CustomProperties["KickerHaircutIndex"]);
        gameManager.playersNickname[id - 1].text = photonPlayer.NickName;

        if (id == 1)
        {
            gameManager.spawnAsKicker(this);
        }
        else if (id == 2)
        {
            gameManager.spawnAsGoalKeeper(this);
        }
        forceCoeficient = 1920f/Screen.height;
        camStartRotK = kicker_cam.gameObject.transform.localRotation;
        if (PhotonNetwork.LocalPlayer.ActorNumber != id)
            this.enabled = false;
    }

    private void Awake()
    {
        gameManager = GameManager.instance;
        gameUI = GameUI.instance;
    }

    private void Start()
    {
        hasToChange = true;
        startpos = new Vector3(0.4403152f, -0.8204808f, -2.119095f);
        Physics2D.gravity = new Vector2(0f, -6.71f);
    }
    

    private void Update()
    {
        if (gameManager.temporalEndGame == true)
            return;
        if (!isSpectator)
        {
            if (!isGoalKeeper)
            {
                photonView.RPC("Kick", RpcTarget.All);
            }
            else if (isGoalKeeper && playerCanCover)
            {
                photonView.RPC("TryCover", RpcTarget.All);
            }
            if (isCamFollowingK)
            {
                kicker_cam.transform.LookAt(ball.transform);
            }
            else
            {
                kicker_cam.transform.localRotation = camStartRotK;
            }
        }
    }

    public void SelectKickerHaircut(int index)
    {
        foreach (GameObject haircut in kicker_haircuts)
        {
            haircut.SetActive(false);
        }
        kicker_haircuts[index].SetActive(true);
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
                if (hit.collider.CompareTag("Ball"))//hit.transform.name == "Soccer_Ball" && hit.transform.GetComponent<Ball>().player.id == id)
                {
                    starttime = Time.time;
                    firstpos = Input.mousePosition;
                    toKick = true;
                    triggerForKick.enabled = false;
                }
            }
            
        }
        if (Input.GetMouseButtonUp(0) && toKick)
        {
            endtime = Time.time;
            lastpos = Input.mousePosition;

            Vector3 distance = lastpos - firstpos;
            distance.z = distance.magnitude;
            Vector3 force = new Vector3((distance.x / ((endtime - starttime) / 0.33f)), (distance.y / ((endtime - starttime) / 0.26f)), (distance.z / ((endtime - starttime) / 0.4f)));

            force *= forceCoeficient;
            toKick = false;

            if (force.y >= 400)
            {

                float baseForce = 400f;
                float diferential = Mathf.Pow(force.y - baseForce, 0.95f); //0.95

                force.y = baseForce + diferential;
                Debug.Log("Now is: " + force.y);
            }

            if (force.x < 0 && force.x > -260)
            {
                force.x = -Mathf.Pow(Mathf.Abs(force.x), 0.92f); //0.92
            }
            else if (force.x <= -260 && force.x > -340)
            {
                force.x = -Mathf.Pow(Mathf.Abs(force.x), 0.95f); //0.95
            }
            else if (force.x <= -340)
            {
                force.x = -Mathf.Pow(Mathf.Abs(force.x), 0.98f); //0.98
            }

            force.x = Mathf.Clamp(force.x, -550, 550);
            force.y = Mathf.Clamp(force.y, 0, 600);
            force.z = Mathf.Clamp(force.z + 120, 520, 700);

            Vector3 ballForce = transform.forward * force.z + transform.right * force.x + transform.up * force.y;

            gameManager.photonView.RPC("keeperCanCover", RpcTarget.All, true);
            StartCoroutine(KickAnimation(ballForce));
            ballReturned = false;
            StartCoroutine(ReturnBall());
        }
       
    }

    IEnumerator KickAnimation(Vector3 ballForce)
    {
        Clock.instance.photonView.RPC("StopTimer", RpcTarget.All);
        //kicker_anim.SetTrigger("KickNow");
        kicker_anim.SetBool("KickNow1", true);
        yield return new WaitForSeconds(0.55f);
        gameUI.kickSound.Play();
        ballRigBody.AddForce(ballForce);
        UncheckAnimBooleans();
        isCamFollowingK = true;
        //yield return new WaitForSeconds(3);
        //UncheckAnimBooleans();
        //isCamFollowingK = true;
    }

    IEnumerator ReturnBall()
    {
        yield return new WaitForSeconds(5.0f);
        
        if(!ballScript.alreadyAGoalResult)
        {
            if(ballScript.touchedByGoalkeeper)
            {
                gameManager.photonView.RPC("MarkSavedGoalToPlayer", RpcTarget.All, photonPlayer);
            }
            else
            {
                gameManager.photonView.RPC("MarkGoalMissedToPlayer", RpcTarget.All, photonPlayer);
            }
        }
        ballScript.alreadyAGoalResult = false;
        ballScript.touchedByGoalkeeper = false;

        yield return new WaitForSeconds(2.0f);
        isCamFollowingK = false;
        ballScript.touchedByGoalkeeper = false;
        ball.transform.localPosition = startpos;
        ball.transform.localRotation = Quaternion.identity;
        ballRigBody.velocity = Vector3.zero;
        ballRigBody.angularVelocity = Vector3.zero;
        ballReturned = true;
        gameManager.photonView.RPC("SwitchPositions", RpcTarget.All);
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
            //goalkeeper_anim.SetBool("ExitAnim", false);
            StartCoroutine(GetPlayerSet(distance));
         }
        
    }

    IEnumerator GetPlayerSet(Vector3 distance)
    {
        bool needToRepeatCover = false;

        if (Mathf.Abs(distance.x) < 50 && distance.y > 0)
            goalkeeper_anim.SetBool("Jump1", true);
        else if (distance.x < 0 && distance.y > 0)
            goalkeeper_anim.SetBool("DivingLeft1", true);
        else if (distance.x > 0 && distance.y > 0)
            goalkeeper_anim.SetBool("DivingRight1", true);
        else if (distance.x < 0 && distance.y < 0)
            goalkeeper_anim.SetBool("BodyLeft1", true);
        else if (distance.x > 0 && distance.y < 0)
            goalkeeper_anim.SetBool("BodyRight1", true);
        else
        {
            canCover = true;
            needToRepeatCover = true;
        }


        //if (this.isGoalKeeper)
        //  canCover = true;
        if(!needToRepeatCover)
        { 
            yield return new WaitForSeconds(0.5f);
            UncheckAnimBooleans();
        }
    }

    public void UncheckAnimBooleans()
    {
        goalkeeper_anim.SetBool("Jump1", false);
        goalkeeper_anim.SetBool("DivingLeft1", false);
        goalkeeper_anim.SetBool("DivingRight1", false);
        goalkeeper_anim.SetBool("BodyLeft1", false);
        goalkeeper_anim.SetBool("BodyRight1", false);
        kicker_anim.SetBool("KickNow1", false);
    }

    
    public void FailedGoalByClock()
    {
        photonView.RPC("FailedGoalRPC", RpcTarget.All);
    }

    [PunRPC]
    public void FailedGoalRPC()
    {
        ballReturned = false;
        toKick = false;
        kicker_anim.SetBool("DidLose", true);

        StartCoroutine(switchPositionsClock());
        
    }

    IEnumerator switchPositionsClock()
    {
        yield return new WaitForSeconds(3);
        kicker_anim.SetBool("DidLose", false);
        if(PhotonNetwork.IsMasterClient)
            gameManager.photonView.RPC("SwitchPositions", RpcTarget.All);
    }
}
