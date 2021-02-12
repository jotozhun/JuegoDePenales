using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviourPunCallbacks
{
    [Header("Screens")]
    public GameObject gameScreen;
    public GameObject waitingScreen;
    public GameObject loginScreen;
    public GameObject signUpScreen;
    public GameObject playerScreen;
    public GameObject estadisticsScreen;
    public GameObject tournamentScreen;
    public GameObject resultsScreen;

    [Header("Game Screen")]
    public TMP_InputField playerName;
    public TMP_InputField roomName;
    public Button playButton;

    [Header("Waiting Room")]
    public Button cancelButton;

    [Header("Login Screen")]
    public TMP_InputField userNameInput;
    public TMP_InputField playerPassword;
    public TextMeshProUGUI loginStatusText;
    public Button loginButton;
    public Button toRegisterScreenButton;

    [Header("SignUp Screen")]
    public TMP_InputField registerNameInput;
    public TMP_InputField registerUsernameInput;
    public TMP_InputField registerEmailInput;
    public TMP_InputField registerPasswordInput;
    public TextMeshProUGUI registerStatusText;
    public Button registerAccountButton;
    public Button toLoginScreenButton;

    [Header("Player Screen")]
    public TextMeshProUGUI playername;
    public Button playGameButton;
    public Button tournamentButton;
    public Button estadisticsButton;
    public Button exitButton;

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
    /*
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            gameObject.SetActive(false);
        }
        else
        {
            //Screen.orientation = ScreenOrientation.Portrait;
            //DontDestroyOnLoad(this);
            //SetScreen(playerScreen);
            //playername.text = "Bienvenido de vuelta " + NetworkManager.instance.userInfo.username + "!";
        }
    }*/
    private void Start()
    {
        if (NetworkManager.instance != null && NetworkManager.instance.isConnected)
        {
            SetScreen(playerScreen);
            //playername.text = "Bienvenido de vuelta " + NetworkManager.instance.userInfo.username + "!";
            //playername.text = "Bienvenido de vuelta " + PhotonNetwork.NickName + "!";
            playername.text = "Bienvenido de vuelta " + PhotonNetwork.LocalPlayer.NickName + "!";
            tournamentButton.interactable = true;
        }
    }

    public void SetScreen(GameObject screen)
    {
        gameScreen.SetActive(false);
        waitingScreen.SetActive(false);
        loginScreen.SetActive(false);
        signUpScreen.SetActive(false);
        playerScreen.SetActive(false);
        estadisticsScreen.SetActive(false);
        tournamentScreen.SetActive(false);
        resultsScreen.SetActive(false);

        screen.SetActive(true);
    }

    // MAIN SCREEN
    /*
    public void OnPlayerNameChanged()
    {
        PhotonNetwork.NickName = playerName.text;
    }
    */
    public void OnPlayButton()
    {
        if (roomName.text != "")
        {
            NetworkManager.instance.CreateRoom(roomName.text);
            playButton.interactable = false;
        }
    }

    public override void OnCreatedRoom()
    {
        playButton.interactable = true;
        gameScreen.SetActive(false);
        waitingScreen.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == NetworkManager.instance.maxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Game");
        }
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
        SetScreen(gameScreen);
    }

    /*public override void OnLeftRoom()
    {
        cancelButton.interactable = true;
        gameScreen.SetActive(true);
        waitingScreen.SetActive(false);
    }*/

    // PLAYER SCREEN
    public void OnEstadisticButtonUI()
    {
        NetworkManager.UserInfo info = NetworkManager.instance.userInfo;
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

    public void OnTournamentButton()
    {
        playerScreen.SetActive(false);
        tournamentScreen.SetActive(true);

        NetworkManager.TorneoInfo torneoInfo = NetworkManager.instance.torneoInfo;
        tournaName.text = torneoInfo.nombre_torneo;
        reglaUno.text = "Anotar " + torneoInfo.num_goles + " para ganar\n" + torneoInfo.tiempo_patear + " segundos para patear";
        torneoInicia.text = torneoInfo.fecha_inicio.ToString();
        torneoFin.text = torneoInfo.fecha_fin.ToString();
        registrados.text = torneoInfo.registrados + " / " + torneoInfo.num_participantes;

        backTournaButton.interactable = true;
    }
    public void OnExitButton()
    {
        playerScreen.SetActive(false);
        loginScreen.SetActive(true);
        loginButton.interactable = true;
    }

    public void OnLogoutButton()
    {
        Destroy(NetworkManager.instance.gameObject);
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

    public void OnSignTorneoButton()
    {
        NetworkManager.instance.SignTorneo(registerButton, backTournaButton, signStatus);
    }

    public void OnBackToPlayerScreen()
    {
        gameScreen.SetActive(false);
        playerScreen.SetActive(true);
    }

    //TESTING
    //Test Actualizar información de usuario
    public void TestUpdateUserInfo()
    {
        StartCoroutine(NetworkManager.instance.AddResultToUser(4, 3, 1, true));
    }


    /*
    public override void OnLeftRoom()
    {
        Debug.Log("Bye bye");
        PhotonNetwork.LoadLevel("Menu");
        //SceneManager.LoadScene("Menu");
    }
    */
}
