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
    public Publicidades publicidades = new Publicidades();

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
                //StartCoroutine(LoginAuth(savedAccount.username, savedAccount.password, loginStatusText));
                OnLoginButtonUI();
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
        _playerCustomProperties["id"] = userLogin.id;

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
            DeactivateButtons();
            Register(registerNameInput.text, registerUsernameInput.text, registerEmailInput.text, registerPasswordInput.text, 
                (string res) => {
                    Debug.Log(res);
                    registerStatusText.text = res;
                    registerStatusText.color = Color.green;
                    ActivateButtons();
                }, (int err) => { 
                if(err == 400)
                {
                    registerStatusText.text = "Ya existe un usuario con este username o email";
                    
                }
                else
                {
                    registerStatusText.text = "Error inesperado, compruebe su conexión a internet";
                }
                registerStatusText.color = Color.red;
                ActivateButtons();
            });
        }
    }

    public void Register(string name, string username, string email, string password, Action<string> res, Action<int> err)
    {
        apiCalls.RegisterUser(registerNameInput.text, registerUsernameInput.text, registerEmailInput.text, registerPasswordInput.text, res, err);
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
            //StartCoroutine(LoginAuth(userNameInput.text, playerPassword.text, loginStatusText));
            DeactivateButtons();
            LoginAuth(userNameInput.text, playerPassword.text, 
                (string res) => {
                    userToken = JsonUtility.FromJson<UserToken>(res);
                    Login(userToken.id, userToken.token,
                        (string loginRes) =>
                    {
                        Debug.Log(loginRes);
                        userLogin = JsonUtility.FromJson<UserLogin>(loginRes);

                        loginStatusText.text = "Bienvenido " + userLogin.username + "!";
                        loginStatusText.color = Color.green;

                        _playerCustomProperties["username"] = userLogin.username;

                        savedAccount = new SavedAccount()
                        {
                            username = userNameInput.text,
                            password = playerPassword.text,
                            isLoggedIn = true
                        };
                        Save();
                        SetPlayerCustomPropierties();
                        StartCoroutine(OnLoginSuccess());
                    }, (int loginErr) =>
                    {
                        loginStatusText.text = "Error inesperado, token no válido";
                        loginStatusText.color = Color.red;
                        offlineButton.interactable = true;
                        ActivateButtons();
                    });
            },  (int err) => {
                if(err == 403)
                {
                    loginStatusText.text = "Usuario o contraseña incorrecta";
                }
                else
                {
                    loginStatusText.text = "Error del servidor, intente más tarde.";
                }
                loginStatusText.color = Color.red;
                offlineButton.interactable = true;
                ActivateButtons();
            });
        }
    }

    public IEnumerator OnLoginSuccess()
    {
        yield return StartCoroutine(apiCalls.GetPublicidad(
            (string res) => {
                Debug.Log(res);
                publicidades = JsonUtility.FromJson<Publicidades>(res);
                StartCoroutine(apiCalls.GetTexture(publicidades.publicidades[0].imagenes[0].nombre, 
                    (Texture2D texture) => {
                        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));
                        Debug.Log("Imagen descargada correctamente!");

                        
                        publicidades.publicidades[0].imagenes[0].sprite = sprite;
                    }, (int textError) => {
                        Debug.Log("No se pudo descargar la imagen!");
                    }));
                
                
            }, (int err) => {
                if (err == 600)
                    Debug.Log("Error en la red, intente más tarde!");
                else
                {
                    Debug.Log("Error inesperado, no se obtuvo respuesta!");
                }
            }));

        yield return new WaitForSeconds(2);
        isConnected = true;
        SceneManager.LoadScene("Menu");
    }

    public void LoginAuth(string username, string password, Action<string> res, Action<int> err)
    {
        StartCoroutine(apiCalls.CallUserToken(username, password, res, err));
    }

    public void Login(int id, string token, Action<string> res, Action<int> err)
    {
        StartCoroutine(apiCalls.GetLoginInfo(id, token, res, err));
    }
    

    public void AddEstadisticasToLocal(bool isWin)
    {
        Player tmpPlayer = PhotonNetwork.LocalPlayer;

        int goles_anotados = (int)tmpPlayer.CustomProperties["Goals"];
        int goles_atajados = (int)tmpPlayer.CustomProperties["SavedGoals"];
        int goles_recibidos = (int)tmpPlayer.CustomProperties["FailedGoals"];

        userLogin.total_partidos += 1;
        if (isWin)
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

    public void CargarPublicidad(Action<string> res, Action<int> err)
    {
        StartCoroutine(apiCalls.GetPublicidad(res, err));
    }

    public void CrearDueloNormal(Player winnerPlayer, Player loserPlayer)
    {
        int id_w = (int)winnerPlayer.CustomProperties["id"];
        int id_l = (int)loserPlayer.CustomProperties["id"];

        int goles_w = (int)winnerPlayer.CustomProperties["Goals"];
        int goles_l = (int)loserPlayer.CustomProperties["Goals"];

        int goles_atajados_ganador = (int)winnerPlayer.CustomProperties["SavedGoals"];
        int goles_atajados_perdedor = (int)loserPlayer.CustomProperties["SavedGoals"];

        int goles_recibidos_ganador = (int)winnerPlayer.CustomProperties["FailedGoals"];
        int goles_recibidos_perdedor = (int)loserPlayer.CustomProperties["FailedGoals"];

        StartCoroutine(apiCalls.CreateDueloNormal(
            id_w, 
            id_l, 
            goles_w, 
            goles_l, 
            goles_atajados_ganador,
            goles_atajados_perdedor,
            goles_recibidos_ganador,
            goles_recibidos_perdedor,
            (string res) => 
            {
                Debug.Log("Duelo creado correctamente!");        
            }, (int err) => 
            {
                Debug.Log("Duelo no se pudo crear!");                                  
            })
        );
    }
    
    public void AddMatchResultToLocal(Player winner, Player loser)
    {
        int id_w = (int)winner.CustomProperties["id"];
        int id_l = (int)loser.CustomProperties["id"];

        string username_w = (string)winner.CustomProperties["username"];
        string username_l = (string)loser.CustomProperties["username"];

        Jugador player_ganador = new Jugador()
        {
            id = id_w,
            username = username_w
        };

        Jugador player_perdedor = new Jugador()
        {
            id = id_l,
            username = username_l
        };

        int goles_w = (int)winner.CustomProperties["Goals"];
        int goles_l = (int)loser.CustomProperties["Goals"];

        DateTime localDate = DateTime.Now;
        var fecha = localDate.ToString("yyyy-MM-dd");

        Duelo tmpDuelo = new Duelo() {
            id = 999,
            ganador = player_ganador,
            perdedor = player_perdedor,
            goles_ganador = goles_w,
            goles_perdedor = goles_l,
            fecha = fecha
        };

        userLogin.AddDuelo(tmpDuelo);

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