using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using System;



public class IniciarSesionController : MonoBehaviour
{

    public Button btnIniciarSesion;
    public Button btnOlvidoPassword;
    public Button btnRecuperarPassword;
    public Button btnCancelar;
    public Button btnRegresarLogin;

    public InputField usuario;
    public InputField password;
    public Text errorIniciarSesion;
   
    public GameObject loginPage;
    public GameObject recoverPasswordPage;
    public GameObject recoverPasswordDonePage;

    public GameObject succesAnswer;
    public GameObject failAnswer;


    public TMP_InputField correoTxtmp;
    public TMP_Text correoTxtmpError;

    public GameObject loadingImage;


    // Start is called before the first frame update
    void Start()
    {
        btnOlvidoPassword.onClick.AddListener(olvidoPasswordOnClick);
        btnRecuperarPassword.onClick.AddListener(recuperarPasswordOnClick);
        btnCancelar.onClick.AddListener(cancelarOnClick);
        btnRegresarLogin.onClick.AddListener(regresarLoginOnClick);
        btnIniciarSesion.onClick.AddListener(recuperarDatosUsuario);
    }

    void Update()
    {
        
    }

    void recuperarDatosUsuario(){
        string usuarioInput = usuario.text;
        string passwordInput = password.text;
        WWWForm form = new WWWForm();

        if(!String.IsNullOrEmpty(usuarioInput) && !String.IsNullOrEmpty(passwordInput)){
            form.AddField("username", usuarioInput);
            form.AddField("password", passwordInput);
            StartCoroutine(autenticarUsuario(form)); 
        }else{
            errorIniciarSesion.text = "Complete todos los campos";
        }
          

    }

    IEnumerator autenticarUsuario(WWWForm form)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post("https://oscarp.pythonanywhere.com/api/autenticar/", form))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log("Resultado: " + ": Error: " + webRequest.downloadHandler.text);
                errorIniciarSesion.text = "Usuario o contraseña incorrectos";
            }
            else
            {
               SceneManager.LoadScene("Radio/scenes/RadioScene");
               
            }
        }
    }

    void olvidoPasswordOnClick(){
        loginPage.SetActive(false);
        recoverPasswordPage.SetActive(true);

	}

    void regresarLoginOnClick(){
        recoverPasswordDonePage.SetActive(false);
        correoTxtmp.text = "";
        succesAnswer.SetActive(false);
        failAnswer.SetActive(false);
        loginPage.SetActive(true);

	}

    void cancelarOnClick(){
        recoverPasswordPage.SetActive(false);
        correoTxtmp.text = "";
        correoTxtmpError.gameObject.SetActive(false);
        loginPage.SetActive(true);
	}

    void recuperarPasswordOnClick(){
        bool isValidEmail = checkCorreo();
        if ( isValidEmail)
        {
            string correoInput = correoTxtmp.text;

            WWWForm form = new WWWForm();
            form.AddField("email", correoInput);

            loadingImage.SetActive(true);
            StartCoroutine(RecuperarPassword(form));         
        }


	}
    //Registro de una nueva cuenta
    IEnumerator RecuperarPassword(WWWForm form)
    {
        // using (UnityWebRequest webRequest = UnityWebRequest.Post("http://127.0.0.1:8000/api/rest-auth/password_reset/", form))
        using (UnityWebRequest webRequest = UnityWebRequest.Post("https://oscarp.pythonanywhere.com/api/rest-auth/password_reset/", form))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            loadingImage.SetActive(false);
            recoverPasswordPage.SetActive(false);
            recoverPasswordDonePage.SetActive(true);
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log("Resultado: " + ": Error: " + webRequest.error);
                Debug.Log("Resultado: " + ": Error: " + webRequest.downloadHandler.text);
                failAnswer.SetActive(true);
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                Debug.Log("Recuperación Exitosa");
                succesAnswer.SetActive(true);
            }
        }
    }


    private bool checkCorreo(){
        if (string.IsNullOrEmpty(correoTxtmp.text))
        {
            correoTxtmpError.text = "Campo Correo no puede estar vacío";
            correoTxtmpError.gameObject.SetActive(true);
            return false;
        }
        if (!Regex.IsMatch(correoTxtmp.text, "^(([\\w-]+\\.)+[\\w-]+|([a-zA-Z]{1}|[\\w-]{2,}))@(([a-zA-Z]+[\\w-]+\\.){1,2}[a-zA-Z]{2,4})$"))
        {
            correoTxtmpError.text = "Correo inválido";
            correoTxtmpError.gameObject.SetActive(true);
            return false;
        }
        correoTxtmpError.gameObject.SetActive(false);
        return true;
    }



   

}
