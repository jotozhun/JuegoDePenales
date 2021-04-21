using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Screens")]
    public GameObject loginScreen;
    public GameObject registerScreen;

    [Header("Buttons")]
    public Button loginButton;
    public Button registerButton;
    public Button toRegisterButton;
    public Button toLoginButton;
    public Button offlineButton;

    [Header("Estadisticas")]
    public UserInfo userInfo;
    public MCharacter characterInfo;
    public int kicker_index;
    public int kicker_haircutIndex;
    public int emblemaIndex;

    [Header("Game")]
    public int numberOfGoals;
    public int secondsToKick;

    [Header("Torneo")]
    //public Button signInTournaButton;
    public TorneoInfo torneoInfo;
    public PremioInfoList premiosInfo;

    [Header("Register")]
    public TMP_InputField registerNameInput;
    public TMP_InputField registerUsernameInput;
    public TMP_InputField registerEmailInput;
    public TMP_InputField registerPasswordInput;
    public TextMeshProUGUI registerStatusText;

    [Header("Login")]
    public TMP_InputField userNameInput;
    public TMP_InputField playerPassword;
    public TextMeshProUGUI loginStatusText; 

    private ExitGames.Client.Photon.Hashtable _playerCustomProperties = new ExitGames.Client.Photon.Hashtable(); 
    //public float resolutionCoeficient;
    public static NetworkManager instance;
    public int maxPlayers;
    
    public bool isTorneo;
    string regUri = "https://juego-penales.herokuapp.com/unity/register.php";
    //string logUri = "http://localhost/JuegoPenales/loginUser.php";
    string logUri = "https://juego-penales.herokuapp.com/unity/login.php";
    string torneoInfoUri = "https://juego-penales.herokuapp.com/unity/isTorneo.php";
    string resultToUserUri = "https://juego-penales.herokuapp.com/unity/addMatchResults.php";
    //string resultToUserUri = "http://localhost/WebJuegoEnLinea/unity/addMatchResults.php";
    string signTorneoUri = "http://localhost/WebJuegoEnLinea/unity/addParticipante.php";

    [Header("Connected Settings")]
    public bool isConnected;
    public UserInfo offlineInfo;
    private void Awake()
    {
        if(instance == null)
        {
            Initialize();
            offlineInfo = new UserInfo();
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Save()
    {
        PlayerPrefs.SetString("save", Helper.Serialize<UserInfo>(userInfo));
    }

    public void SaveLogout()
    {
        userInfo.isLoggedIn = false;
        Save();
    }

    public void Load()
    {
        if(PlayerPrefs.HasKey("save"))
        {
            userInfo = Helper.Deserialize<UserInfo>(PlayerPrefs.GetString("save"));
            if(userInfo.isLoggedIn)
            {
                loginButton.interactable = false;
                registerButton.interactable = false;
                userNameInput.text = userInfo.username;
                playerPassword.text = userInfo.password;
                StartCoroutine(LoginUser(userInfo.username, userInfo.password, loginStatusText));
            }
        }
    }


    void Initialize()
    {
        Debug.Log(photonView.ViewID);
        maxPlayers = 3;
        kicker_index = 0;
        instance = this;
        Screen.orientation = ScreenOrientation.Portrait;
        DontDestroyOnLoad(this);
        photonView.ViewID = 1;
        //default
        numberOfGoals = 5;
        secondsToKick = 7;
        //Custom propierties
        _playerCustomProperties["EmblemaIndex"] = emblemaIndex;
        _playerCustomProperties["KickerHaircutIndex"] = kicker_haircutIndex;
        _playerCustomProperties["Goals"] = 0;
        _playerCustomProperties["SavedGoals"] = 0;
        _playerCustomProperties["FailedGoals"] = 0;
        _playerCustomProperties["KicksLeft"] = numberOfGoals;
        _playerCustomProperties["isGoalkeeper"] = false;
        PhotonNetwork.LocalPlayer.CustomProperties = _playerCustomProperties;
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
            if (loginButton != null)
                loginButton.interactable = true;
        }
        if(SceneManager.GetActiveScene().name == "GameAccount")
            Load(); //Carga informacion del usuario si es que esta existe
    }

    public void OnOfflineButton()
    {
        isConnected = false;
        userInfo = offlineInfo;
        SceneManager.LoadScene("Menu");
    }

    public void CreateRoom(string roomName)
    {
        RoomOptions options = new RoomOptions();
        
        options.MaxPlayers = (byte)maxPlayers;

        PhotonNetwork.JoinOrCreateRoom(roomName, options, null, null);
    }

    

    void ActivateButtons()
    {
        loginButton.interactable = true;
        toLoginButton.interactable = true;
        registerButton.interactable = true;
        toRegisterButton.interactable = true;
    }

    void DeactivateButtons()
    {
        loginButton.interactable = false;
        toLoginButton.interactable = false;
        registerButton.interactable = false;
        toRegisterButton.interactable = false;
        offlineButton.interactable = false;
    }

    public void OnPLayerNameChanged()
    {
        PhotonNetwork.NickName = userNameInput.text;
    }

    public IEnumerator RegisterUser(string name, string username, string email, string password, TextMeshProUGUI status)
    {
        DeactivateButtons();
        WWWForm form = new WWWForm();
        form.AddField("regName", name);
        form.AddField("regUser", username);
        form.AddField("regEmail", email);
        form.AddField("regPass", password);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(regUri, form))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                status.text = webRequest.error;
                status.color = Color.red;
                offlineButton.interactable = true;
            }
            else
            {
                status.text = webRequest.downloadHandler.text;
                status.color = Color.green;
            }
            ActivateButtons();
        }
    }

    public void OnRegisterButtonUI()
    {
        bool regName = isFieldEmpty(registerNameInput);
        bool regUsername = isFieldEmpty(registerUsernameInput);
        bool regEmail = isFieldEmpty(registerEmailInput);
        bool regPass = isFieldEmpty(registerPasswordInput);
        bool areFieldsEmpty = regName || regUsername || regEmail || regPass;

        if (areFieldsEmpty)
        {
            registerStatusText.text = "Por favor, complete todos los campos!";
            registerStatusText.color = Color.red;
        }
        else
        {
            StartCoroutine(RegisterUser(registerNameInput.text, registerUsernameInput.text, registerEmailInput.text, registerPasswordInput.text, registerStatusText));
        }
    }

    bool isFieldEmpty(TMP_InputField field)
    {
        return field.text == "";
    }

    public void trimInputs(TMP_InputField input)
    {
        input.text = input.text.Trim();
    }

    public void SetScreen(GameObject screen)
    {
        loginScreen.SetActive(false);
        registerScreen.SetActive(false);
        screen.SetActive(true);
    }

    public IEnumerator LoginUser(string username, string password, TextMeshProUGUI status)
    {
        DeactivateButtons();
        WWWForm form = new WWWForm();
        form.AddField("logUser", username);
        form.AddField("logPass", password);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(logUri, form))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            ActivateButtons();
            if (webRequest.isNetworkError)
            {
                status.text = webRequest.error;
                status.color = Color.red;
                offlineButton.interactable = true;
            }
            else
            {
                string resp = webRequest.downloadHandler.text;
                if (!resp.Contains("Error"))
                {
                    userInfo = JsonUtility.FromJson<UserInfo>(resp);
                    userInfo.isLoggedIn = true;
                    Save();
                    status.text = "Bienvenido " + userInfo.username + "!";
                    status.color = Color.green;
                    yield return new WaitForSeconds(2);
                    isConnected = true;
                    loginScreen.SetActive(false);
                    status.text = "";
                    StartCoroutine(GetTorneoInfo());
                    SceneManager.LoadScene("Menu");
                }
                else
                {
                    status.text = resp;
                    status.color = Color.red;
                    offlineButton.interactable = true;
                }
            }
        }
    }

    public void OnLoginButtonUI()
    {
        bool logName = isFieldEmpty(userNameInput);
        bool logPass = isFieldEmpty(playerPassword);
        bool areFieldsEmpty = logName || logPass;

        if (areFieldsEmpty)
        {
            loginStatusText.text = "Por favor, complete todos los campos!";
            loginStatusText.color = Color.red;
        }
        else
        {
            StartCoroutine(LoginUser(userNameInput.text, playerPassword.text, loginStatusText));
        }
    }


    public IEnumerator GetTorneoInfo()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(torneoInfoUri))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                string resp = webRequest.downloadHandler.text;
                if (!resp.Contains("Error"))
                {
                    torneoInfo = JsonUtility.FromJson<TorneoInfo>(resp);
                    isTorneo = true;
                }
            }
        }
    }

    public IEnumerator AddResultToUser(int golesAnotados, int golesRecibidos, int golesAtajados, bool isWin)
    {
        userInfo.total_partidos += 1;
        if (isWin)
        {
            userInfo.partidos_ganados += 1;
        }
        else
        {
            userInfo.partidos_perdidos += 1;
        }
        userInfo.goles_anotados += golesAnotados;
        userInfo.goles_atajados += golesAtajados;
        userInfo.goles_recibidos += golesRecibidos;

        WWWForm form = new WWWForm();
        form.AddField("id_user", userInfo.id_user);
        form.AddField("total_partidos", userInfo.total_partidos);
        form.AddField("partidos_ganados", userInfo.partidos_ganados);
        form.AddField("partidos_perdidos", userInfo.partidos_perdidos);
        form.AddField("goles_anotados", userInfo.goles_anotados);
        form.AddField("goles_atajados", userInfo.goles_atajados);
        form.AddField("goles_recibidos", userInfo.goles_recibidos);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(resultToUserUri, form))
        {
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                string resp = webRequest.downloadHandler.text;
                Debug.Log(resp);
            }
        }
    }

    public IEnumerator SignTorneo(Button registerButton, Button backButton, TextMeshProUGUI signStatus)
    {
        registerButton.interactable = false;
        backButton.interactable = false;

        WWWForm form = new WWWForm();
        form.AddField("id_usuario", userInfo.id_user);
        form.AddField("id_torneo", torneoInfo.id_torneo);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(signTorneoUri, form))
        {
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {
                Debug.Log(webRequest.error);
                signStatus.text = webRequest.error;
                signStatus.color = Color.red;
            }
            else
            {
                string resp = webRequest.downloadHandler.text;
                Debug.Log(resp);
                signStatus.text = resp;
                if (!resp.Contains("Error"))
                {
                    signStatus.color = Color.green;
                }
                else
                {
                    signStatus.color = Color.red;
                }


            }
        }
        registerButton.interactable = true;
        backButton.interactable = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
    }

    public void OnUpdateUserInfo(int golesAnotados, int golesRecibidos, int golesAtajados, bool isWin)
    {
        StartCoroutine(AddResultToUser(golesAnotados, golesRecibidos, golesAtajados, isWin));
    }

    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
        Screen.orientation = ScreenOrientation.Landscape;
    }

    public class UserInfo
    {
        public int id_user;
        public int id_torneo;
        public bool isadmin;
        public string nombre;
        public string username;
        public string email;
        public bool email_verified;
        public string password;
        public int total_partidos;
        public int partidos_ganados;
        public int partidos_perdidos;
        public int goles_anotados;
        public int goles_atajados;
        public int goles_recibidos;
        public int posicion_ranking;
        public bool isLoggedIn;

        public UserInfo()
        {
            nombre = "Harta demencia offline";
            username = "Harta demencia offline";
            email = "";
            password = "";
        }
    }

    public class TorneoInfo
    {
        public int id_torneo;
        public string nombre_torneo;
        public DateTime fecha_inicio;
        public DateTime fecha_fin;
        public int num_participantes;
        public int num_goles;
        public int tiempo_espera;
        public int tiempo_patear;
        public int num_grupos;
        public int registrados;
    }

    public class PremioInfo
    {
        public int id_premio;
        public int id_torneo;
        public string titulo;
        public string sponsor;
        public string descripcion;
        public int posicion;
    }

    public class PremioInfoList
    {
        public List<PremioInfo> premiosInfo;
    }
}