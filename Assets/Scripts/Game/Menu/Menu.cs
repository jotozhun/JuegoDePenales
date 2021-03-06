﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using AccountModels;

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
    public GameObject spectateButton;

    [Header("Waiting Room")]
    public Button cancelButton;


    [Header("Player Screen")]
    public GameObject gameLogo;
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
    public NetworkManager networkManager;
    private void Awake()
    {
        if(NetworkManager.instance != null)
        {
            networkManager = NetworkManager.instance;
        }
    }

    private void Start()
    {
        if (networkManager.isConnected)
        {
            playername.text = "¿Listo para ganar?\n" + networkManager.userLogin.username;
            tournamentButton.interactable = true;
            ResetGoalProperties();
        }
        else
        {
            playername.text = "Bienvenido " + networkManager.userLogin.username;
            playGameButton.interactable = false;
            tournamentButton.interactable = false;
            estadisticsButton.interactable = false;
        }
        SetScreen(playerScreen);
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
        if (roomName.text != "")
        {

            networkManager.CreateRoom(roomName.text);
            playButton.interactable = false;
            
        }
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
    }

    public override void OnJoinedRoom()
    {
        int curPlayerCount = (int)PhotonNetwork.CurrentRoom.PlayerCount;
        if (curPlayerCount == (networkManager.maxPlayers - 1))
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
            networkManager.photonView.RPC("ChangeScene", RpcTarget.AllBuffered, "Game");
            gameLogo.SetActive(false);
        }
        
         
        else if(curPlayerCount > (networkManager.maxPlayers - 1))
        {
            networkManager.ChangeScene("Game");
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

}
