using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Camera cam;
    public PlayerController player;

    Vector3 firstpos, lastpos;
    Vector3 startpos;
    float starttime, endtime;
    float dragdistance;
    bool toKick;
    bool returned = true;
    private void Start()
    {
        dragdistance = Screen.height * 10 / 100;
        startpos = gameObject.transform.position;
        transform.localRotation = Quaternion.identity;
    }

    void Update()
    {
        Kick();
    }

    void Kick()
    {
        if (Input.GetMouseButtonDown(0) && returned)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.name == "Soccer Ball")
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

            Debug.Log("Before Clamp: " + force);

            force.x = Mathf.Clamp(force.x, -800, 800);
            force.y = Mathf.Clamp(force.y, 0, 850);
            force.z = 1350;

            Vector3 ballForce = transform.forward * force.z + transform.right * force.x + transform.up * force.y;
            Debug.Log("After Clamp: " + force);
            //this is changed by Joel Torres
            StartCoroutine(KickAnimation(ballForce));
            returned = false;
            StartCoroutine(ReturnBall());

        }
    }

    

    IEnumerator ReturnBall()
    {
        yield return new WaitForSeconds(3);
        transform.position = startpos;
        transform.localRotation = Quaternion.identity;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        returned = true;
    }

    IEnumerator KickAnimation(Vector3 ballForce)
    {
        player.KickBall();
        yield return new WaitForSeconds(0.55f);
        gameObject.GetComponent<Rigidbody>().AddForce(ballForce);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("GoalBound"))
        {
            GameManager.instance.MarkGoalToPlayer(player.id);
        }
    }
}
