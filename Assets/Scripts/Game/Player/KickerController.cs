using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class KickerController : MonoBehaviourPunCallbacks
{
    [Header("Player parts")]
    public Camera cam;
    public GameObject ball;

    [Header("Haircuts")]
    public GameObject[] haircuts;

    [Header("Animator")]
    public GameObject[] anim;

    [Header("Ball Components")]
    public Ball ballScript;
    public Rigidbody ballRigidbody;
    private Vector3 firstpos, lastpos;
    private Vector3 startpos;
    private float starttime, endtime;
    private bool toKick;
    private bool ballReturned = true;
    public bool playerCanCover;
    public float forceCoeficient;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

            GameManager.instance.photonView.RPC("keeperCanCover", RpcTarget.All, true);
            //StartCoroutine(KickAnimation(ballForce));
            ballReturned = false;
            //StartCoroutine(ReturnBall());
        }
    }
}
