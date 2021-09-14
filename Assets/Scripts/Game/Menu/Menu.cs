using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using AccountModels;
using System;

public class Menu : MonoBehaviourPunCallbacks
{
    [Header("Screens")]
    public GameObject gameScreen;
    public GameObject waitingScreen;
    public GameObject playerScreen;
    public GameObject estadisticsScreen;
    public GameObject tournamentScreen;
    public GameObject interfazScreen;
    public GameObject historialScreen;

    [Header("Game Screen")]
    public TMP_InputField roomName;
    public Button playButton;
    public Button playAgendadoButton;
    public GameObject spectateButton;

    [Header("Waiting Room")]
    public WaitingTorneo waitingTorneoScript;
    public Button cancelButton;


    [Header("Player Screen")]
    public GameObject gameLogo;
    public TextMeshProUGUI playername;
    public Button playGameButton;
    public Button tournamentButton;
    public Button estadisticsButton;
    public Button exitButton;
    public GameObject reconnectScreen;
    public Button reconnectButton;
    public Button exitButtonReconnect;
    public TextMeshProUGUI reconnectText;

    [Header("Estadistics Screen")]
    public Button backEstadisticsButton;
    public TextMeshProUGUI total_partidos;
    public TextMeshProUGUI partidos_ganados;
    public TextMeshProUGUI partidos_perdidos;
    public TextMeshProUGUI goles_anotados;
    public TextMeshProUGUI goles_recibidos;
    public TextMeshProUGUI goles_atajados;

    [Header("Tournament Screen")]
    public Button registerButton;
    public Button backTournaButton;
    public TextMeshProUGUI tournaName;
    public TextMeshProUGUI reglaUno;
    public TextMeshProUGUI torneoInicia;
    public TextMeshProUGUI torneoFin;
    public TextMeshProUGUI registrados;
    public TextMeshProUGUI signStatus;

    public static Menu instance;
    public MenuTutorial tutorialManager;
    public NetworkManager networkManager;
    private DateTime fecha_inicio;
    private DateTime fecha_fin;
    private bool isTimeForAgenda;
    private bool blockedForTorneo;
    private GameObject startScreen;
    private DueloAgendado duelo_agendado;
    private bool isTorneo = false;
    private void Awake()
    {
        if (NetworkManager.instance != null)
        {
            networkManager = NetworkManager.instance;
            duelo_agendado = networkManager.userLogin.duelo_agendado;
        }
        instance = this;
    }

    private void Start()
    {
        if(Screen.orientation != ScreenOrientation.Portrait)
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }
        startScreen = playerScreen;
        if (networkManager.isConnected)
        {
            playername.text = "¿Listo para ganar?\n" + networkManager.userLogin.username;
            tournamentButton.interactable = true;
            ResetGoalProperties();

            if (duelo_agendado.id != 0)
            {
                fecha_inicio = DateTime.Parse(duelo_agendado.fecha_hora_inicio);
                fecha_fin = DateTime.Parse(duelo_agendado.fecha_hora_inicio).AddSeconds(duelo_agendado.tiempo_prorroga);
                //fecha_fin = DateTime.Parse(duelo_agendado.fecha_hora_fin);
                Debug.Log("Initialize a las: " + fecha_fin);
                InitializeAgendado();
            }
            /*
            if (networkManager.userLogin.isFirstLogin())
            {
                startScreen = interfazScreen;
                //tutorialManager.Initialize();
            }
            */
        }
        else
        {
            playername.text = "Bienvenido " + networkManager.userLogin.username;
            playGameButton.interactable = false;
            tournamentButton.interactable = false;
            estadisticsButton.interactable = false;
        }
        SetScreen(startScreen);
        //TestingPublicidad.instance.Initialize();
    }

    private void Update()
    {
        if (blockedForTorneo)
        {
            if (playAgendadoButton.interactable)
            {
                playAgendadoButton.interactable = false;
            }
            return;
        }
        isTimeForAgenda = checkForAgendamiento();
        if (isTimeForAgenda)
        {
            if (!playAgendadoButton.interactable)
            {
                playAgendadoButton.interactable = true;
            }
        }
        else
        {
            if (DateTime.Now >= fecha_fin)
            {
                Debug.Log("Se acabo la hora para entrar!");
                blockedForTorneo = true;
            }
        }
    }

    private void InitializeAgendado()
    {
        if (DateTime.Now >= fecha_fin)
        {
            blockedForTorneo = true;
        }
        isTimeForAgenda = checkForAgendamiento();

    }

    private bool checkForAgendamiento()
    {
        return DateTime.Now >= fecha_inicio && DateTime.Now <= fecha_fin;
    }

    public void SetScreen(GameObject screen)
    {
        gameScreen.SetActive(false);
        waitingScreen.SetActive(false);
        playerScreen.SetActive(false);
        estadisticsScreen.SetActive(false);
        tournamentScreen.SetActive(false);
        interfazScreen.SetActive(false);
        historialScreen.SetActive(false);

        screen.SetActive(true);
    }

    public void OnPlayButton()
    {
        /*
        if (roomName.text != "")
        {
            //networkManager.CreateRoom(roomName.text);
            networkManager.CreateRoom(PhotonNetwork.LocalPlayer.NickName);
            playButton.interactable = false;
            
        }
        */
        //networkManager.CreateRoom(PhotonNetwork.LocalPlayer.NickName);
        networkManager.SetNormalGoals();
        PhotonNetwork.JoinRandomRoom();
        playButton.interactable = false;
        isTorneo = false;
    }

    public void OnPlayTorneoButton()
    {
        networkManager.SetTorneoGoals();
        networkManager.CreateRoom("torneo" + duelo_agendado.jugador1.username + duelo_agendado.jugador2.username);
        isTorneo = true;
        playAgendadoButton.interactable = false;
    }

    public void ResetGoalProperties()
    {
        Player player = PhotonNetwork.LocalPlayer;
        player.CustomProperties["Goals"] = 0;
        player.CustomProperties["SavedGoals"] = 0;
        player.CustomProperties["FailedGoals"] = 0;
        player.CustomProperties["KicksLeft"] = networkManager.numberOfGoals;
    }

    public override void OnCreatedRoom()
    {
        playButton.interactable = true;
        gameScreen.SetActive(false);
        waitingScreen.SetActive(true);
        gameLogo.SetActive(false);
        if (PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable _roomCustomProperties = networkManager._playerCustomProperties;//new ExitGames.Client.Photon.Hashtable();
            if (isTorneo)
            {
                DueloAgendado agendados = networkManager.userLogin.duelo_agendado;
                string[] expectedUsers = {agendados.jugador1.username, agendados.jugador2.username};
                PhotonNetwork.CurrentRoom.SetExpectedUsers(expectedUsers);
            }
            _roomCustomProperties.Add("isTorneo", isTorneo);
            PhotonNetwork.CurrentRoom.SetCustomProperties(_roomCustomProperties);
        }
    }

    public override void OnJoinedRoom()
    {
        int curPlayerCount = (int)PhotonNetwork.CurrentRoom.PlayerCount;
        
        if (curPlayerCount == (networkManager.maxPlayers))
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
            networkManager.photonView.RPC("ChangeScene", RpcTarget.AllBuffered, "Game");
            gameLogo.SetActive(false);
        }

        /*
       else if(curPlayerCount > (networkManager.maxPlayers - 1))
       {
           networkManager.ChangeScene("Game");
       }
       */
    }

    // WAITING SCREEN
    public void OnCancelButton_WR()
    {
        StartCoroutine(LeaveWaitingRoom());
    }

    IEnumerator LeaveWaitingRoom()
    {
        cancelButton.interactable = false;
        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom)
            yield return null;
        cancelButton.interactable = true;
        if (waitingTorneoScript.enabled)
        {
            waitingTorneoScript.enabled = false;
        }
        SetScreen(gameScreen);

    }

    // PLAYER SCREEN
    public void OnEstadisticButtonUI()
    {
        UserLogin info = networkManager.userLogin;
        total_partidos.text = info.total_partidos.ToString();
        partidos_ganados.text = info.partidos_ganados.ToString();
        partidos_perdidos.text = info.partidos_perdidos.ToString();
        goles_anotados.text = info.goles_anotados.ToString();
        goles_recibidos.text = info.goles_recibidos.ToString();
        goles_atajados.text = info.goles_atajados.ToString();
        SetScreen(estadisticsScreen);
    }

    public void OnPlayGameButton()
    {

        playerScreen.SetActive(false);
        gameScreen.SetActive(true);
        playButton.interactable = true;
    }

    public void OnPracticarButton()
    {
        Screen.orientation = ScreenOrientation.Landscape;
        SceneManager.LoadScene("GamePractice");
    }

    public void OnExitButton()
    {
        playerScreen.SetActive(false);
    }

    public void OnLogoutButton()
    {
        networkManager.SaveLogout();
        Destroy(networkManager.gameObject);
        StartCoroutine(DisconnectAndLoad());
    }

    IEnumerator DisconnectAndLoad()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
            yield return null;
        SceneManager.LoadScene("GameAccount");
    }

    // ESTADISTICS SCREEN

    // TOURNAMENT SCREEN
    public void OnBackTournamentButton()
    {
        backTournaButton.interactable = false;
        tournamentScreen.SetActive(false);
        playerScreen.SetActive(true);
    }

    public void OnBackToPlayerScreen()
    {
        gameScreen.SetActive(false);
        playerScreen.SetActive(true);
    }

    public void OnSpectateButton()
    {

    }


    public void OnReconnect()
    {
        reconnectScreen.SetActive(true);
        reconnectButton.interactable = true;
        exitButtonReconnect.interactable = true;
        reconnectText.text = "Se ha perdido la conexión con el servidor";
        reconnectText.color = Color.white;
    }

    public void OnReconnectButton()
    {
        networkManager.ConnectToPhotonServer();
        reconnectButton.interactable = false;
        exitButtonReconnect.interactable = false;
    }

    public IEnumerator OnReconnectSuccess()
    {
        reconnectText.text = "Conexión establecida correctamente";
        reconnectText.color = Color.green;
        yield return new WaitForSeconds(1.5f);
        reconnectScreen.SetActive(false);
        reconnectText.text = "Se ha perdido la conexión con el servidor";
        reconnectText.color = Color.white;
        reconnectButton.interactable = true;
        exitButtonReconnect.interactable = true;
    }

    public void TestingCoroutine()
    {
        int id1 = 9;
        int id2 = 10;

        int goles_w = 5;
        int goles_l = 3;

        int isTorneo = 1;

        int atajados_w = 1;
        int atajados_l = 0;

        int recibidos_w = 0;
        int recibidos_l = 1;

        StartCoroutine(NetworkAPICalls.instance.CreateDueloNormal(
            id1,
            id2,
            goles_w,
            goles_l,
            isTorneo,
            atajados_w,
            atajados_l,
            recibidos_w,
            recibidos_l,
            (string res) =>
            {
                DueloTorneo dueloTorneo = JsonUtility.FromJson<DueloTorneo>(res);
                int idDuelo = dueloTorneo.id;
                int idGanador = dueloTorneo.ganador.id;
                int idPerdedor = dueloTorneo.perdedor.id;

                StartCoroutine(NetworkAPICalls.instance.SetPlayedDueloAgendado(idDuelo,
                    idGanador,
                    idPerdedor,
                    (int resp) =>
                    {
                        Debug.Log("CORRECTO MI PEX " + resp);
                    }, (int error) => { 
                    
                    }
                    ));

            }, (int err) => 
            { 
            
            }));
    }

    public void OnBannerButton()
    {
        Application.OpenURL("https://frontedpenales2021.herokuapp.com/login");
    }
}
