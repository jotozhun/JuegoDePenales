using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickForce : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameUIPractice.instance.playerObject.KickAnimation(GameUIPractice.instance.playerObject.ballForce);
        }
    }
}
