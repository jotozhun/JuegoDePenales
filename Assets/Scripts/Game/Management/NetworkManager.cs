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
using AccountModels;

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
    public MCharacter characterInfo;

    [Header("Game")]
    public int numberOfGoals;
    public int secondsToKick;

    [Header("Torneo")]
    //public Button signInTournaButton;
    public TorneoInfo torneoInfo;

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

    public NetworkAPICalls apiCalls;
    [HideInInspector]
    public UserToken userToken;
    public UserLogin userLogin;
    public SavedAccount savedAccount;
    public StatusCodes responses = new StatusCodes();

    private ExitGames.Client.Photon.Hashtable _playerCustomProperties = new ExitGames.Client.Photon.Hashtable();
    //public float resolutionCoeficient;
    public static NetworkManager instance;
    public int maxPlayers;


    [Header("Connected Settings")]
    public bool isConnected;
    public UserLogin offlineInfo;
    private void Awake()
    {
        if (instance == null)
        {
            Initialize();
            offlineInfo = new UserLogin();
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Save()
    {
        PlayerPrefs.SetString("saved", Helper.Serialize<SavedAccount>(savedAccount));
    }

    public void SaveLogout()
    {
        savedAccount.isLoggedIn = false;
        Save();
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("saved"))
        {
            savedAccount = Helper.Deserialize<SavedAccount>(PlayerPrefs.GetString("saved"));
            if (savedAccount.isLoggedIn)
            {
                loginButton.interactable = false;
                registerButton.interactable = false;
                userNameInput.text = savedAccount.username;
                playerPassword.text = savedAccount.password;
                StartCoroutine(LoginAuth(savedAccount.username, savedAccount.password, loginStatusText));
            }
        }
    }


    void Initialize()
    {
        Debug.Log(photonView.ViewID);
        maxPlayers = 3;
        instance = this;
        Screen.orientation = ScreenOrientation.Portrait;
        DontDestroyOnLoad(this);
        photonView.ViewID = 1;
        //default
        numberOfGoals = 5;
        secondsToKick = 7;
    }

    void SetPlayerCustomPropierties()
    {
        int emblemaIndex = userLogin.emblema;
        int kicker_haircutIndex = userLogin.haircut_player;

        _playerCustomProperties["EmblemaIndex"] = emblemaIndex;
        _playerCustomProperties["KickerHaircutIndex"] = kicker_haircutIndex;
        ResetPlayerGameProperties();
    }

    public void ResetPlayerGameProperties()
    {
        _playerCustomProperties["Goals"] = 0;
        _playerCustomProperties["SavedGoals"] = 0;
        _playerCustomProperties["FailedGoals"] = 0;
        _playerCustomProperties["KicksLeft"] = numberOfGoals;
        _playerCustomProperties["isGoalkeeper"] = false;
        _playerCustomProperties["isDeathMatchTime"] = false;
        _playerCustomProperties["hasMarkedAResult"] = false;
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
        if (SceneManager.GetActiveScene().name == "GameAccount")
            Load(); //Carga informacion del usuario si es que esta existe
    }

    public void OnOfflineButton()
    {
        isConnected = false;
        userLogin = offlineInfo;
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
            StartCoroutine(Register(registerNameInput.text, registerUsernameInput.text, registerEmailInput.text, registerPasswordInput.text, registerStatusText));
        }
    }

    public IEnumerator Register(string name, string username, string email, string password, TextMeshProUGUI status)
    {
        DeactivateButtons();
        yield return StartCoroutine(apiCalls.RegisterUser(registerNameInput.text, registerUsernameInput.text, registerEmailInput.text, registerPasswordInput.text));

        Debug.Log("Code: " + responses.registerCode);

        if(responses.registerCode == 200)
        {
            status.text = "Usuario Registrado Correctamente!";
            status.color = Color.green;
            yield return new WaitForSeconds(2);
            SetScreen(loginScreen);
        }
        else
        {
            status.text = "Error en el servidor, intente más tarde por favor";
            status.color = Color.red;
        }
        ActivateButtons();
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
            StartCoroutine(LoginAuth(userNameInput.text, playerPassword.text, loginStatusText));
        }
    }

    public IEnumerator LoginAuth(string username, string password, TextMeshProUGUI status)
    {
        DeactivateButtons();
        yield return StartCoroutine(apiCalls.CallUserToken(username, password));

        if (userToken.statusCode == 200)
        {
            int userId = userToken.id;
            string tmpToken = userToken.token;

            yield return StartCoroutine(apiCalls.GetLoginInfo(userId, tmpToken));
            
            if (userLogin.statusCode == 200)
            {
                status.text = "Bienvenido " + userLogin.username + "!";
                status.color = Color.green;
                savedAccount = new SavedAccount() {
                    username = username,
                    password = password,
                    isLoggedIn = true};
                Save();
                SetPlayerCustomPropierties();
                yield return new WaitForSeconds(2);
                isConnected = true;
                SceneManager.LoadScene("Menu");
            }
            else
            {
                status.text = "Error inesperado, token no válido";
                status.color = Color.red;
                offlineButton.interactable = true;
                ActivateButtons();
            }
        }
        else
        {
            if (userToken.statusCode == 403)
            {
                status.text = "Usuario o contraseña incorrecta";
            }
            else
            {
                status.text = "Error del servidor, intente más tarde";
            }
            status.color = Color.red;
            offlineButton.interactable = true;
            ActivateButtons();
        }
    }

    public void AddLocalMatchResult(bool isWin, int goles_anotados, int goles_atajados, int goles_recibidos)
    {
        userLogin.total_partidos += 1;
        if(isWin)
        {
            userLogin.partidos_ganados += 1;
        }
        else
        {
            userLogin.partidos_perdidos += 1;
        }

        userLogin.goles_anotados += goles_anotados;
        userLogin.goles_atajados += goles_atajados;
        userLogin.goles_recibidos += goles_recibidos;
    }

    public IEnumerator AddMatchResultsToServer(bool isWin)
    {
        Player tmpPlayer = PhotonNetwork.LocalPlayer;

        int id = userToken.id;
        string token = userToken.token;
        int goles_anotados = (int)tmpPlayer.CustomProperties["Goals"];
        int goles_atajados = (int)tmpPlayer.CustomProperties["SavedGoals"];
        int goles_recibidos = (int)tmpPlayer.CustomProperties["FailedGoals"];

        yield return StartCoroutine(apiCalls.AddMatchResultsToPlayer(
            isWin,
            id,
            goles_anotados,
            goles_atajados,
            goles_recibidos,
            token
            ));
    }



    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
    }

    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
        Screen.orientation = ScreenOrientation.Landscape;
    }

}