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

    [Header("Player parts")]
    public GameObject playerModel;
    public Camera cam;
    public GameObject ball;

    [Header("Haircuts")]
    public GameObject[] kicker_haircuts;

    [Header("Cam settings")]
    private Quaternion camStartRot;
    private bool isCamFollowing;
    //Player components
    public Animator anim;

    public float forceCoeficient;
    private Vector3 firstCoverPos, lastCoverPos;
    //Ball components
    private Rigidbody ballRigBody;
    private Vector3 firstpos, lastpos;
    private Vector3 startpos;
    private float starttime, endtime;
    private bool toKick; 
    private bool ballReturned = true;


    public void Initialize()
    {
        GameUIPractice.instance.players[0] = this;
        this.transform.position = GameUIPractice.instance.kickerSpawn.position;
        SelectKickerHaircut(NetworkManager.instance.userLogin.haircut_player);
        GameManagerPractice.instance.spawnAsKicker(this);
        cam.gameObject.SetActive(true);
        forceCoeficient = 1920f / Screen.height;
        camStartRot = cam.gameObject.transform.localRotation;
    }

    private void Start()
    {
        ballRigBody = ball.GetComponent<Rigidbody>();
        startpos = new Vector3(1.029999f, 0.21f, 3.519997f);
        Physics2D.gravity = new Vector2(0f, -6.71f);
    }


    private void Update()
    {
        Kick();
        if (isCamFollowing)
        {
            cam.transform.LookAt(ball.transform);
        }
        else
            cam.transform.localRotation = camStartRot;
    }

    public void SelectKickerHaircut(int index)
    {
        foreach(GameObject haircut in kicker_haircuts)
        {
            haircut.SetActive(false);
        }
        kicker_haircuts[index].SetActive(true);
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
            //Debug.Log("last" + lastpos);
            Vector3 distance = lastpos - firstpos;
            distance.z = distance.magnitude;
            //Vector3 force = new Vector3((distance.x / ((endtime - starttime) / 0.33f)), (distance.y / ((endtime - starttime) / 0.26f)), (distance.z / ((endtime - starttime) / 0.4f)));
            float fixedTime = 0f; 
            
            fixedTime = Mathf.Clamp((endtime - starttime), 0.35f, 5f);
            Vector3 force = new Vector3((distance.x / (fixedTime / 0.39f)), (distance.y / (fixedTime / 0.35f)), (distance.z / (fixedTime / 0.37f)));
            
            force *= forceCoeficient;
            toKick = false;
            float baseForce = 0f;
            float diferential = 0f;
            if (force.y >= 400)
            {
                baseForce = 400f;
                diferential = Mathf.Pow(force.y - baseForce, 0.95f);

                force.y = baseForce + diferential;
            }

            if (force.x < 0 && force.x > -260)
            {
                force.x = -Mathf.Pow(Mathf.Abs(force.x), 0.92f);
            }else if(force.x <= -260 && force.x > -340)
            {
                force.x = -Mathf.Pow(Mathf.Abs(force.x), 0.95f);
            }else if(force.x <= -340)
            {
                force.x = -Mathf.Pow(Mathf.Abs(force.x), 0.98f);
            }

            force.x = Mathf.Clamp(force.x, -600, 600);
            force.y = Mathf.Clamp(force.y, 0, 600);
            force.z = Mathf.Clamp(force.z + 120, 540, 700);
            Vector3 ballForce = transform.forward * force.z + transform.right * force.x + transform.up * force.y;

            StartCoroutine(KickAnimation(ballForce));
            ballReturned = false;
            StartCoroutine(ReturnBall());
        }
    }

    IEnumerator KickAnimation(Vector3 ballForce)
    {

        //kicker_anim.SetTrigger("KickNow");
        anim.SetBool("KickNow1", true);
        yield return new WaitForSeconds(0.75f);
        GameUIPractice.instance.kickSound.Play();
        ballRigBody.AddForce(ballForce);
        isCamFollowing = true;
        //cam.gameObject.transform.LookAt(ball.transform);
        anim.SetBool("KickNow1", false);
    }

    //Testing coroutines
    IEnumerator ReturnBall()
    {
        yield return new WaitForSeconds(4.0f);
        isCamFollowing = false;
        ball.transform.localPosition = startpos;
        ball.transform.localRotation = Quaternion.identity;
        ballRigBody.velocity = Vector3.zero;
        ballRigBody.angularVelocity = Vector3.zero;
        ballReturned = true;
        cam.transform.localRotation = camStartRot;
    }

}
