using System.Collections;
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
    private bool ballReturned = true;
    public bool playerCanCover;

    public float forceCoeficient;
    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;
        Debug.Log("Player id: " + id);
        GameUI.instance.players[id - 1] = this;

        this.transform.position = GameUI.instance.playerSpawn.position;
        SelectKickerHaircut(NetworkManager.instance.kicker_haircutIndex);
        //GameManager.instance.playersNickname[id - 1].text = NetworkManager.instance.userInfo.username;
        GameManager.instance.playersNickname[id - 1].text = photonPlayer.NickName;
        GameManager.instance.playerEmblemas[id - 1].sprite = GameUI.instance.playerEmblemas[NetworkManager.instance.emblemaIndex];
        //GameUI.instance.playersName[id - 1].text = photonPlayer.NickName;
        if (id == 1)
            GameManager.instance.spawnAsKicker(this);
        else if (id == 2)
            GameManager.instance.spawnAsGoalKeeper(this);

        forceCoeficient = 1920f/Screen.height;
        camStartRotK = kicker_cam.gameObject.transform.localRotation;
    }

    private void Start()
    {
        hasToChange = true;
        startpos = new Vector3(0.4403152f, -0.8204808f, -2.119095f);
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
        if(isCamFollowingK)
        {
            kicker_cam.transform.LookAt(ball.transform);
        }else
        {
            kicker_cam.transform.localRotation = camStartRotK;
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
                float diferential = Mathf.Pow(force.y - baseForce, 0.95f);

                force.y = baseForce + diferential;
                Debug.Log("Now is: " + force.y);
            }

            if (force.x < 0 && force.x > -260)
            {
                force.x = -Mathf.Pow(Mathf.Abs(force.x), 0.92f);
            }
            else if (force.x <= -260 && force.x > -340)
            {
                force.x = -Mathf.Pow(Mathf.Abs(force.x), 0.95f);
            }
            else if (force.x <= -340)
            {
                force.x = -Mathf.Pow(Mathf.Abs(force.x), 0.98f);
            }

            force.x = Mathf.Clamp(force.x, -600, 600);
            force.y = Mathf.Clamp(force.y, 0, 600);
            force.z = Mathf.Clamp(force.z + 120, 520, 700);

            Vector3 ballForce = transform.forward * force.z + transform.right * force.x + transform.up * force.y;

            //touch_ball_col.enabled = false;
            //GameManager.instance.photonView.RPC("stopTime", RpcTarget.AllBuffered);
            GameManager.instance.photonView.RPC("keeperCanCover", RpcTarget.AllBuffered, true);
            StartCoroutine(KickAnimation(ballForce));
            ballReturned = false;
            StartCoroutine(ReturnBall());
            //StartCoroutine(PlayerCoverBall());
        }
       
    }

    IEnumerator KickAnimation(Vector3 ballForce)
    {

        //kicker_anim.SetTrigger("KickNow");
        kicker_anim.SetBool("KickNow1", true);
        yield return new WaitForSeconds(0.55f);
        GameUI.instance.kickSound.Play();
        ballRigBody.AddForce(ballForce);
        UncheckAnimBooleans();
        isCamFollowingK = true;
        //yield return new WaitForSeconds(0.50f);
    }

    IEnumerator ReturnBall()
    {
        yield return new WaitForSeconds(4.0f);
        isCamFollowingK = false;
        ball.transform.localPosition = startpos;
        ball.transform.localRotation = Quaternion.identity;
        ballRigBody.velocity = Vector3.zero;
        ballRigBody.angularVelocity = Vector3.zero;
        ballReturned = true;
        /*
        GameManager.instance.photonView.RPC("restartTime", RpcTarget.AllBuffered);
        if (!GameManager.instance.markGoal && !GameManager.instance.missGoal)
        {
            GameManager.instance.photonView.RPC("MarkGoalMissedToPlayer", RpcTarget.AllBuffered, id - 1);
        }
        GameManager.instance.markGoal = false;
        GameManager.instance.missGoal = false;
        checkRestartDecreaseKicks();
        */
        GameManager.instance.photonView.RPC("SwitchPositions", RpcTarget.AllBuffered);
    }
    /*
    IEnumerator PlayerCoverBall()
    {
        yield return new WaitForSeconds(1.1f);
        GameManager.instance.photonView.RPC("keeperCanCover", RpcTarget.AllBuffered, false);
    }
    */
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
        yield return new WaitForSeconds(0.5f);
        //if (this.isGoalKeeper)
          //  canCover = true;
        UncheckAnimBooleans();
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

    /*
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
    */
    /*
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
    /*}
    public void salirGame()
    {
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
        //PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Menu");
    }
    */
}
