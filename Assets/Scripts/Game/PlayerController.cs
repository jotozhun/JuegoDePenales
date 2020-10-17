using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    public int id;

    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void KickBall()
    {
        anim.SetTrigger("kick");
    }
}
