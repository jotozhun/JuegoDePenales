using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System;
using Newtonsoft.Json;
// https://www.youtube.com/watch?v=GRn49ehm_pI
// https://www.newtonsoft.com/json/help/html/DeserializeDictionary.htm

public class RegistroController : MonoBehaviour
{

    public Button btnSiguiente;
    public Button btnRegistrar;
    public Button btnCancelar;
    public Button btnAtras;

    public GameObject firstPage;
    public GameObject secondPage;

    public TMP_InputField nombresTxtmp;
    public TMP_InputField apellidosTxtmp;
    public TMP_InputField correoTxtmp;
    public TMP_InputField usuarioTxtmp;
    public TMP_InputField contrasenaTxtmp;
    public TMP_InputField contrasena2Txtmp;
    public TMP_InputField fechNacDiaTxtmp;
    public TMP_InputField fechNacMesTxtmp;
    public TMP_InputField fechNacAnioTxtmp;
    public TMP_InputField numCelularTxtmp;

    public TMP_Text nombresTxtmpError;
    public TMP_Text apellidosTxtmpError;
    public TMP_Text correoTxtmpError;
    public TMP_Text usuarioTxtmpError;
    public TMP_Text contrasenaTxtmpError;
    public TMP_Text contrasena2TxtmpError;
    public TMP_Text fechNacTxtmpError;
    public TMP_Text numCelularTxtmpError;

    public TMP_Text mensajeTxtmp;
    public GameObject loadingImage;

    // Start is called before the first frame update
    void Start()
    {
        btnSiguiente.onClick.AddListener(siguienteOnClick);
        btnRegistrar.onClick.AddListener(registroOnClick);
        btnCancelar.onClick.AddListener(cancelarOnClick);
        btnAtras.onClick.AddListener(showFirstPage);
        // nombresTxtmp.onValueChanged.AddListener(delegate {ValueChangeCheck(nombresTxtmpError); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void registroOnClick(){
        mensajeTxtmp.text = "";
        bool isValidUser = checkUser();
        bool isValidLastPassword = checkPassword();
        bool isValidPassword2 = checkPassword2();
        bool isValidFechNac = checkFechNac();
        if ( isValidUser && isValidLastPassword && isValidPassword2 && isValidFechNac)
        {
            string nombresInput = nombresTxtmp.text;
            string apellidosInput = apellidosTxtmp.text;
            string correoInput = correoTxtmp.text;
            string usuarioInput = usuarioTxtmp.text;
            string contrasenaInput = contrasenaTxtmp.text;
            string numCelularInput = numCelularTxtmp.text;
            string fechNacInput = fechNacAnioTxtmp.text + "-" + fechNacMesTxtmp.text + "-" + fechNacDiaTxtmp.text;

            WWWForm form = new WWWForm();
            form.AddField("username", usuarioInput);
            form.AddField("password", contrasenaInput);
            form.AddField("email", correoInput);
            form.AddField("telefono", numCelularInput);
            form.AddField("first_name", nombresInput);
            form.AddField("last_name", apellidosInput);
            form.AddField("fecha_nac", fechNacInput);

            loadingImage.SetActive(true);
            StartCoroutine(RegisterClient(form));         
        }


	}

    IEnumerator RegisterClient(WWWForm form)
    {
        // using (UnityWebRequest webRequest = UnityWebRequest.Post("http://127.0.0.1:8000/api/rest-auth/register/", form))
        using (UnityWebRequest webRequest = UnityWebRequest.Post("http://oscarp.pythonanywhere.com/api/rest-auth/register/", form))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log("Resultado: " + ": Error: " + webRequest.error);
                Debug.Log("Resultado: " + ": Error: " + webRequest.downloadHandler.text);
                Dictionary<string, List<string>> errores = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(webRequest.downloadHandler.text);
                if (errores != null)
                {
                    string unionErrores = "";
                    foreach( KeyValuePair<string, List<string>> kvp in errores )
                    {
                        unionErrores+=kvp.Key+": "+ kvp.Value[0]+"; ";
                    }
                    if (unionErrores.Length > 0)
                    {
                        unionErrores = unionErrores.Substring(0, unionErrores.Length - 2);   
                    }
                    Debug.Log("errores: "+unionErrores);
                    mensajeTxtmp.text = "Error: " + unionErrores;
                }else
                {
                    mensajeTxtmp.text = "Error: " + webRequest.error;
                }
                loadingImage.SetActive(false);
                mensajeTxtmp.gameObject.SetActive(true);

            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
                Debug.Log("Registro Exitoso");
                mensajeTxtmp.text = "Registro exitoso" ;
                loadingImage.SetActive(false);
                mensajeTxtmp.gameObject.SetActive(true);
                 SceneManager.LoadScene("Radio/scenes/RadioScene");
                //TODO redireccionar a otra pagina cuando se haya creado la cuenta
            }
        }
    }

    void cancelarOnClick(){
        limpiarTexts();
        ocultarErrores();
        showFirstPage();
    }

    void limpiarTexts(){
        nombresTxtmp.text = "";
        apellidosTxtmp.text = "";
        correoTxtmp.text = "";
        usuarioTxtmp.text = "";
        contrasenaTxtmp.text = "";
        contrasena2Txtmp.text = "";
        fechNacDiaTxtmp.text = "";
        fechNacMesTxtmp.text = "";
        fechNacAnioTxtmp.text = "";
        numCelularTxtmp.text = "";
    }

    void ocultarErrores(){
        nombresTxtmpError.gameObject.SetActive(false);
        apellidosTxtmpError.gameObject.SetActive(false);
        correoTxtmpError.gameObject.SetActive(false);
        usuarioTxtmpError.gameObject.SetActive(false);
        contrasenaTxtmpError.gameObject.SetActive(false);
        contrasena2TxtmpError.gameObject.SetActive(false);
        fechNacTxtmpError.gameObject.SetActive(false);
        numCelularTxtmpError.gameObject.SetActive(false);
        
        mensajeTxtmp.gameObject.SetActive(false);
    }

    void siguienteOnClick(){
        bool isValidName = checkNames();
        bool isValidLastName = checkApellidos();
        bool isValidEmail = checkCorreo();
        if ( isValidName && isValidLastName && isValidEmail)
        {
            showSecondPage();            
        }
    }

    //TODO saber a que escena se debe cambiar
    void atrasOnClick(){
        // Scene scene = SceneManager.GetActiveScene();
        // Debug.Log("Active Scene is '" + scene.name + "'.");
    }

    private void showFirstPage(){
        firstPage.SetActive(true);
        btnSiguiente.gameObject.SetActive(true);
        secondPage.SetActive(false);
        btnRegistrar.gameObject.SetActive(false);
    }
    private void showSecondPage(){
        firstPage.SetActive(false);
        btnSiguiente.gameObject.SetActive(false);
        secondPage.SetActive(true);
        btnRegistrar.gameObject.SetActive(true);
    }


    //TODO detectar fin de ingreso de datos
    public void ValueChangeCheck(TMP_Text textError){
        Debug.Log("Value Changed");
    }

    private bool checkNames(){
        if (string.IsNullOrEmpty(nombresTxtmp.text))
        {
            nombresTxtmpError.text = "Campo Nombres no puede estar vacío";
            nombresTxtmpError.gameObject.SetActive(true);
            return false;
        }    
        nombresTxtmpError.gameObject.SetActive(false);
        return true;
    }

    private bool checkApellidos(){
        if (string.IsNullOrEmpty(apellidosTxtmp.text))
        {
            apellidosTxtmpError.text = "Campo Apellidos no puede estar vacío";
            apellidosTxtmpError.gameObject.SetActive(true);
            return false;
        }
        apellidosTxtmpError.gameObject.SetActive(false);
        return true;
    }

    private bool checkCorreo(){
        // if (nombresTxtmp.text.Length == 0)
        if (string.IsNullOrEmpty(correoTxtmp.text))
        {
            correoTxtmpError.text = "Campo Correo no puede estar vacío";
            correoTxtmpError.gameObject.SetActive(true);
            return false;
        }
        // return correoTxtmp.text != null && Regex.IsMatch(correoTxtmp.text, "/^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/");
        // return correoTxtmp.text != null && Regex.IsMatch(correoTxtmp.text, "/^[^\s@]+@[^\s@]+\.[^\s@]+$/");
        if (!Regex.IsMatch(correoTxtmp.text, "^(([\\w-]+\\.)+[\\w-]+|([a-zA-Z]{1}|[\\w-]{2,}))@(([a-zA-Z]+[\\w-]+\\.){1,2}[a-zA-Z]{2,4})$"))
        {
            correoTxtmpError.text = "Correo inválido";
            correoTxtmpError.gameObject.SetActive(true);
            return false;
        }
        correoTxtmpError.gameObject.SetActive(false);
        return true;
    }

    private bool checkUser(){
        // if (nombresTxtmp.text.Length == 0)
        if (string.IsNullOrEmpty(usuarioTxtmp.text))
        {
            usuarioTxtmpError.text = "Campo Usuario no puede estar vacío";
            usuarioTxtmpError.gameObject.SetActive(true);
            return false;
        }    
        usuarioTxtmpError.gameObject.SetActive(false);
        return true;
    }

    private bool checkPassword(){
        // if (nombresTxtmp.text.Length == 0)
        if (string.IsNullOrEmpty(contrasenaTxtmp.text))
        {
            contrasenaTxtmpError.text = "Campo Contraseña no puede estar vacío";
            contrasenaTxtmpError.gameObject.SetActive(true);
            return false;
        }
        var hasNumber = new Regex(@"[0-9]+");
        var hasUpperChar = new Regex(@"[A-Z]+");
        var hasMinimum8Chars = new Regex(@".{8,}");
        if (!hasNumber.IsMatch(contrasenaTxtmp.text))
        {
            contrasenaTxtmpError.text = "Contraseña debe contener al menos un número";
            contrasenaTxtmpError.gameObject.SetActive(true);
            return false;
        }
        if (!hasUpperChar.IsMatch(contrasenaTxtmp.text))
        {
            contrasenaTxtmpError.text = "Contraseña debe contener al menos una mayúscula";
            contrasenaTxtmpError.gameObject.SetActive(true);
            return false;
        }
        if (!hasMinimum8Chars.IsMatch(contrasenaTxtmp.text))
        {
            contrasenaTxtmpError.text = "Contraseña debe contener al menos 8 caracteres";
            contrasenaTxtmpError.gameObject.SetActive(true);
            return false;
        }
        contrasenaTxtmpError.gameObject.SetActive(false);
        return true;
    }

    private bool checkPassword2(){
        if (string.IsNullOrEmpty(contrasena2Txtmp.text))
        {
            contrasena2TxtmpError.text = "Campo Confirmar contraseña no puede estar vacío";
            contrasena2TxtmpError.gameObject.SetActive(true);
            return false;
        }
        if (!(contrasena2Txtmp.text == contrasenaTxtmp.text))
        {
            contrasena2TxtmpError.text = "Contraseñas no coinciden";
            contrasena2TxtmpError.gameObject.SetActive(true);
            return false;            
        }
        contrasena2TxtmpError.gameObject.SetActive(false);
        return true;
    }

    private bool checkFechNac(){
        string dia = fechNacDiaTxtmp.text;
        string mes = fechNacMesTxtmp.text;
        string anio = fechNacAnioTxtmp.text;
        if (string.IsNullOrEmpty(dia))
        {
            fechNacTxtmpError.text = "Campo Día no puede estar vacío";
            fechNacTxtmpError.gameObject.SetActive(true);
            return false;
        }    
        if (string.IsNullOrEmpty(mes))
        {
            fechNacTxtmpError.text = "Campo Mes no puede estar vacío";
            fechNacTxtmpError.gameObject.SetActive(true);
            return false;
        }    
        if (string.IsNullOrEmpty(anio))
        {
            fechNacTxtmpError.text = "Campo Año no puede estar vacío";
            fechNacTxtmpError.gameObject.SetActive(true);
            return false;
        }
        if (!(int.Parse(dia)>=1 && int.Parse(dia)<=31))
        {
            fechNacTxtmpError.text = "Campo Día inválido";
            fechNacTxtmpError.gameObject.SetActive(true);
            return false;
        }
        if (!(int.Parse(mes)>=1 && int.Parse(mes)<=12))
        {
            fechNacTxtmpError.text = "Campo Mes inválido";
            fechNacTxtmpError.gameObject.SetActive(true);
            return false;
        }
        DateTime currentTime = System.DateTime.Now;
        DateTime fechNacTime = new DateTime(int.Parse(anio), int.Parse(mes), int.Parse(dia));
        if (!(int.Parse(anio)>=1900 && int.Parse(anio)<=currentTime.Year))
        {
            fechNacTxtmpError.text = "Campo Año inválido";
            fechNacTxtmpError.gameObject.SetActive(true);
            return false;
        }
        int comparacion = DateTime.Compare(fechNacTime, currentTime);    //<0 -> fechNacTime<currentTime
        if (!(comparacion<0))
        {
            fechNacTxtmpError.text = "Fecha no puede ser mayor que la fecha actual";
            fechNacTxtmpError.gameObject.SetActive(true);
            return false;
        }
        fechNacTxtmpError.gameObject.SetActive(false);
        return true;
    }
}
