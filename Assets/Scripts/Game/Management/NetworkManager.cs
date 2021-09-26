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
    [SerializeField]
    private Button reconnectButton;
    [SerializeField]
    private GameObject reconnectObj;

    public NetworkAPICalls apiCalls;
    [HideInInspector]
    public UserToken userToken;
    public UserLogin userLogin;
    public SavedAccount savedAccount;
    public Publicidades publicidades = new Publicidades();
    public PublicidadesGame publicidadesGame;
    public PublicidadesGame publicidadesGameTmp;

    public ExitGames.Client.Photon.Hashtable _playerCustomProperties = new ExitGames.Client.Photon.Hashtable();
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

    public void SavePublicidad()
    {
        PlayerPrefs.SetString("publicidades", Helper.Serialize<PublicidadesGame>(publicidadesGame));
    }

    public void SaveLogout()
    {
        savedAccount.isLoggedIn = false;
        Save();
    }

    public void LoadPublicidad()
    {
        if(PlayerPrefs.HasKey("publicidades"))
        {
            publicidadesGameTmp = Helper.Deserialize<PublicidadesGame>(PlayerPrefs.GetString("publicidades"));
        }
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
        /*
        if(PlayerPrefs.HasKey("publicidades"))
        {
            publicidadesGame = Helper.Deserialize<PublicidadesGame>(PlayerPrefs.GetString("publicidades"));
        }
        */
    }


    void Initialize()
    {
        Debug.Log(photonView.ViewID);
        maxPlayers = 2;
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
            //if (loginButton != null)
            //  loginButton.interactable = true;
        }
        if (SceneManager.GetActiveScene().name == "GameAccount")
        {
            Load(); //Carga informacion del usuario si es que esta existe
            reconnectObj.SetActive(false);
            ActivateButtons(true);
            loginStatusText.text = "Conexión exitosa al servidor!";
            loginStatusText.color = Color.green;
        } else if (SceneManager.GetActiveScene().name == "Menu")
        {
            StartCoroutine(Menu.instance.OnReconnectSuccess());
        }
    }

    public void OnOfflineButton()
    {
        isConnected = false;
        userLogin = offlineInfo;
        SceneManager.LoadScene("Menu");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom(userLogin.username);
    }

    public void CreateRoom(string roomName)
    {
        RoomOptions options = new RoomOptions();

        options.MaxPlayers = (byte)maxPlayers;

        PhotonNetwork.JoinOrCreateRoom(roomName, options, null, null);
    }



    void ActivateButtons(bool noConexButton)
    {
        loginButton.interactable = true;
        toLoginButton.interactable = true;
        registerButton.interactable = true;
        toRegisterButton.interactable = true;
        if (noConexButton)
            offlineButton.interactable = true;
    }

    void DeactivateButtons(bool noConexButton)
    {
        loginButton.interactable = false;
        toLoginButton.interactable = false;
        registerButton.interactable = false;
        toRegisterButton.interactable = false;
        if (noConexButton)
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
            DeactivateButtons(true);
            Register(registerNameInput.text, registerUsernameInput.text, registerEmailInput.text, registerPasswordInput.text,
                (string res) => {
                    Debug.Log(res);
                    registerStatusText.text = res;
                    registerStatusText.color = Color.green;
                    ActivateButtons(true);
                }, (int err) => {
                    if (err == 400)
                    {
                        registerStatusText.text = "Ya existe un usuario con este username o email";

                    }
                    else
                    {
                        registerStatusText.text = "Error inesperado, compruebe su conexión a internet";
                    }
                    registerStatusText.color = Color.red;
                    ActivateButtons(true);
                });
        }
    }

    public void Register(string name, string username, string email, string password, Action<string> res, Action<int> err)
    {
        StartCoroutine(apiCalls.RegisterUser(registerNameInput.text, registerUsernameInput.text, registerEmailInput.text, registerPasswordInput.text, res, err));
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
            DeactivateButtons(true);
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
                        //offlineButton.interactable = true;
                        ActivateButtons(true);
                    });
                }, (int err) => {
                    if (err == 403)
                    {
                        loginStatusText.text = "Usuario o contraseña incorrecta";
                    }
                    else
                    {
                        loginStatusText.text = "Error del servidor, intente más tarde.";
                    }
                    loginStatusText.color = Color.red;
                    //offlineButton.interactable = true;
                    ActivateButtons(true);
                });
        }
    }

    public int publicidadesDescargadas = 0;

    public IEnumerator OnLoginSuccess()
    {
        LoadPublicidad();
        yield return StartCoroutine(apiCalls.GetPublicidad(
            (string res) => {
                Debug.Log(res);                
                publicidades = JsonUtility.FromJson<Publicidades>(res);
                foreach (Publicidad _publicidad in publicidades.publicidades)
                {
                    Debug.Log("Publicidades descargadas: " + publicidadesDescargadas);
                    if (_publicidad.tipo_imagen.Equals("gol"))
                    {
                        if (!CheckPublicidadId(_publicidad.id, publicidadesGameTmp.gol))
                        {
                            DownloadPublicidad(publicidadesGame.gol, _publicidad);
                        }/*
                        else
                            DownloadImagesPublicidad(publicidadesGame.horizontal, _publicidad);
                        */
                    }
                    else if (_publicidad.tipo_imagen.Equals("banner horizontal"))
                    {
                        if (!CheckPublicidadId(_publicidad.id, publicidadesGameTmp.horizontal))
                            DownloadPublicidad(publicidadesGame.horizontal, _publicidad);
                        /*
                        else
                            DownloadImagesPublicidad(publicidadesGame.horizontal, _publicidad);
                        */
                    }
                    else
                    {
                        if (!CheckPublicidadId(_publicidad.id, publicidadesGameTmp.vertical))
                        {
                            DownloadPublicidad(publicidadesGame.vertical, _publicidad);
                        }/*
                        else
                            DownloadImagesPublicidad(publicidadesGame.horizontal, _publicidad);
                        */
                    }
                }
                SavePublicidad();
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

    private bool CheckPublicidadId(int id1, List<PublicidadGame> publicidadGame)
    {
        bool result = false;
        if (publicidadGame.Count > 0)
        {
            foreach (PublicidadGame _publicidadGame in publicidadGame)
            {
                if (_publicidadGame.id == id1)
                {
                    result = true;
                }
            }
        }
        return result;
    }

    private void DownloadImagesPublicidad(List<PublicidadGame> _publicidadGameList, Publicidad _publicidad)
    {
        foreach (PublicidadGame _publicidadGame in _publicidadGameList)
        {
            if (_publicidadGame.id == _publicidad.id)
            {

                foreach (Imagen _imagen in _publicidad.imagenes)
                {
                    bool downloadImage = true;
                    foreach (ImageGame _imageGame in _publicidadGame.imagenes)
                    {
                        if (_imageGame.id == _imagen.id)
                            downloadImage = false;
                    }
                    if (downloadImage)
                    {
                        StartCoroutine(apiCalls.GetTexture(_imagen.nombre,
                        (Texture2D texture) =>
                        {
                            Sprite tmpSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));
                            _publicidadGame.imagenes.Add(new ImageGame(_imagen.id, tmpSprite));
                        }, (int textError) =>
                        {
                            Debug.Log("No se pudo descargar la imagen!");
                        }));
                    }
                }
            }
        }
    }

    private void DownloadPublicidad(List<PublicidadGame> _publicidadGame, Publicidad _publicidad)
    {
        publicidadesDescargadas++;
        PublicidadGame tmpPublicidadGame = new PublicidadGame(
                                _publicidad.id,
                                _publicidad.marca,
                                _publicidad.descripcion
                                );
        _publicidadGame.Add(tmpPublicidadGame);
        foreach (Imagen imagen in _publicidad.imagenes)
        {
            StartCoroutine(apiCalls.GetTexture(imagen.nombre,
                (Texture2D texture) =>
                {
                    Sprite tmpSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));
                    tmpPublicidadGame.imagenes.Add(new ImageGame(imagen.id, tmpSprite));
                }, (int textError) =>
                {
                    Debug.Log("No se pudo descargar la imagen!");
                }));
        }
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

    public void CrearDueloNormal(int winnerId, int loserId, int isTorneo)
    {
        StartCoroutine(apiCalls.CreateDueloNormal(
            winnerId,
            loserId,
            1,
            0,
            isTorneo,
            0,
            0,
            0,
            0,
            (string res) => {
                if (isTorneo == 1)
                {
                    Duelo tmpDuelo = JsonUtility.FromJson<Duelo>(res);
                    StartCoroutine(apiCalls.SetPlayedDueloAgendado(
                                    tmpDuelo.id,
                                    tmpDuelo.ganador.id,
                                    tmpDuelo.perdedor.id,
                                    (int resCode) => {
                                        //GameUI.instance.SuccessOnDueloAgendadoRequest();
                                    }, (int errCode) => {
                                        Debug.Log("Ha ocurrido un error con el duelo agendado :C");
                                    }));
                }
            }, (int err) => {
                Debug.Log("Ha ocurrido un error al enviar el duelo");  
                }
            ));
    }

    public void CrearDueloNormal(Player winnerPlayer, Player loserPlayer, int isTorneo)
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
            isTorneo,
            goles_atajados_ganador,
            goles_atajados_perdedor,
            goles_recibidos_ganador,
            goles_recibidos_perdedor,
            (string res) =>
            {
                Debug.Log("Es torneo?: " + isTorneo);
                if (isTorneo == 1)
                {
                    Duelo tmpDuelo = JsonUtility.FromJson<Duelo>(res);
                    StartCoroutine(apiCalls.SetPlayedDueloAgendado(
                                    tmpDuelo.id,
                                    tmpDuelo.ganador.id,
                                    tmpDuelo.perdedor.id,
                                    (int resCode) => {
                                        //GameUI.instance.SuccessOnDueloAgendadoRequest();
                                    }, (int errCode) => {
                                        Debug.Log("Ha ocurrido un error con el duelo agendado :C");
                                    }));
                } else
                {
                    //GameUI.instance.SuccessOnDueloAgendadoRequest();
                }
                //userLogin.AddDuelo(tmpDuelo);
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


    public void SetDueloAgendadoResult(int duelo, int ganador, int perdedor)
    {
        StartCoroutine(apiCalls.SetPlayedDueloAgendado(duelo, ganador, perdedor,
            (int res) => {
                if (res == 200)
                    Debug.Log("Información actualizada correctamente!");
            },
            (int err) => {
                if (err == 600)
                    Debug.Log("Ha ocurrido un error en el servidor");
            }
        ));
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        this.isConnected = false;
        switch (SceneManager.GetActiveScene().name)
        {
            case "GameAccount":
                OnReconnect();
                break;
            case "Menu":
                Menu.instance.OnReconnect();
                break;
            case "Game":
                if(!(bool)PhotonNetwork.CurrentRoom.CustomProperties["gameEnded"])
                    GameUI.instance.OnLoseGameDisconnectedFromGame();
                break;
                
        }
    }

    private bool CanRecoverFromDisconnect(DisconnectCause cause)
    {
        switch(cause)
        {
            case DisconnectCause.Exception:
            case DisconnectCause.ServerTimeout:
            case DisconnectCause.ClientTimeout:
            case DisconnectCause.DisconnectByServerLogic:
            case DisconnectCause.DisconnectByServerReasonUnknown:
                return true;
        }
        return false;
    }

    private void Recover()
    {
        if (!PhotonNetwork.ReconnectAndRejoin())
        {
            
            Debug.LogError("ReconnectAndRejoin failed, trying Reconnect");
            if (!PhotonNetwork.Reconnect())
            {
                Debug.LogError("Reconnect failed, trying ConnectUsingSettings");
                if (!PhotonNetwork.ConnectUsingSettings())
                {
                    Debug.LogError("ConnectUsingSettings failed");
                }
            }
        }
    }


    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
        Screen.orientation = ScreenOrientation.Landscape;
    }

    public void ConnectToPhotonServer()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnReconnect()
    {
        loginStatusText.text = "No ha sido posible reconectarse";
        loginStatusText.color = Color.red;
        SetScreen(loginScreen);
        reconnectButton.interactable = true;
        reconnectObj.SetActive(true);
        DeactivateButtons(false);
    }

    public void OnReconnectButton()
    {
        ConnectToPhotonServer();
        reconnectButton.interactable = false;
    }

    public void SetTorneoGoals()
    {
        numberOfGoals = userLogin.duelo_agendado.numero_inicial_goles;
        PhotonNetwork.LocalPlayer.CustomProperties["KicksLeft"] = numberOfGoals;
    }

    public void SetNormalGoals()
    {
        numberOfGoals = 5;
        PhotonNetwork.LocalPlayer.CustomProperties["KicksLeft"] = numberOfGoals;
    }
    /*
    private void Update()
    {
        if(!PhotonNetwork.IsConnected && this.isConnected)
        {
            this.isConnected = false;
            switch (SceneManager.GetActiveScene().name)
            {
                case "GameAccount":
                    OnReconnect();
                    break;
                case "Menu":
                    Menu.instance.OnReconnect();
                    break;
            }
        }
    }
    */
}