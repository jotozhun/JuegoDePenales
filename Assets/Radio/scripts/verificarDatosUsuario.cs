using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class verificarDatosUsuario : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       verificarLogin();
    }

    public void verificarLogin(){
        string inicioSesion = PlayerPrefs.GetString("inicioSesion");
        if( String.Equals(inicioSesion,"true") ){
                SceneManager.LoadScene("Radio/scenes/RadioScene");
        }
    }
}
