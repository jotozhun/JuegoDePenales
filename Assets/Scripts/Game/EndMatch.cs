using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndMatch : MonoBehaviour
{
    [HideInInspector]
    public static EndMatch instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    public void ShowEndMatchScreen()
    {

    }
}
