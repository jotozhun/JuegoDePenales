using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
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
    public TMP_InputField playerPassword;
    public Button loginButton;

    [Header("SignUp Screen")]
    public TMP_InputField registerNameInput;
    public TMP_InputField registerUsernameInput;
    public TMP_InputField registerEmailInput;
    public TMP_InputField registerPasswordInput;
    public TextMeshProUGUI registerStatusText;
    public Button registerAccountButton;
    public Button toLoginScreenButton;

    [Header("Player Screen")]
    public Button playGameButton;
    public Button tournamentButton;
    public Button estadisticsButton;
    public Button exitButton;

    [Header("Estadistics Screen")]
    public Button backEstadisticsButton;

    [Header("Tournament Screen")]
    public Button registerButton;
    public Button backTournaButton;

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
    public void OnPlayerNameChanged()
    {
        PhotonNetwork.NickName = playerName.text;
    }
    public void OnPlayButton()
    {
        playButton.interactable = false;
        if(roomName.text != "")
            NetworkManager.instance.CreateRoom(roomName.text);
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
    public void OnCancelButton()
    {
        cancelButton.interactable = false;
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        cancelButton.interactable = true;
        gameScreen.SetActive(true);
        waitingScreen.SetActive(false);
    }

    // LOGIN SCREEN
    public void OnLoginButton()
    {
        loginButton.interactable = false;
        loginScreen.SetActive(false);
        playerScreen.SetActive(true);

        ActivateAllButtonPlayerScreen();
    }

    // SIGN UP SCREEN

    public void OnRegisterButtonUI()
    {
        bool regName = isFieldEmpty(registerNameInput);
        bool regUsername = isFieldEmpty(registerUsernameInput);
        bool regEmail = isFieldEmpty(registerEmailInput);
        bool regPass = isFieldEmpty(registerPasswordInput);
        bool areFieldsEmpty = regName && regUsername && regEmail && regPass;

        if (areFieldsEmpty)
        {
            registerStatusText.text = "Por favor, complete todos los campos!";
            registerStatusText.color = Color.red;
        }
        else
        {
            StartCoroutine(NetworkManager.instance.RegisterUser(registerNameInput.text, registerUsernameInput.text, registerEmailInput.text, registerPasswordInput.text, registerStatusText));
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
    // PLAYER SCREEN
    public void OnPlayGameButton()
    {
        DesactivateAllButtonPlayerScreen();

        playerScreen.SetActive(false);
        gameScreen.SetActive(true);
        playButton.interactable = true;
    }
    public void OnTournamentButton()
    {
        DesactivateAllButtonPlayerScreen();

        playerScreen.SetActive(false);
        tournamentScreen.SetActive(true);
        backTournaButton.interactable = true;
    }
    public void OnEstadisticButton()
    {
        DesactivateAllButtonPlayerScreen();

        playerScreen.SetActive(false);
        estadisticsScreen.SetActive(true);
        backEstadisticsButton.interactable = true;
    }
    public void OnExitButton()
    {
        DesactivateAllButtonPlayerScreen();

        playerScreen.SetActive(false);
        loginScreen.SetActive(true);
        loginButton.interactable = true;
    }

    // ESTADISTICS SCREEN
    public void OnBackEstadisticButton()
    {
        backEstadisticsButton.interactable = false;
        estadisticsScreen.SetActive(false);
        playerScreen.SetActive(true);

        ActivateAllButtonPlayerScreen();
    }

    // TOURNAMENT SCREEN
    public void OnBackTournamentButton()
    {
        backTournaButton.interactable = false;
        tournamentScreen.SetActive(false);
        playerScreen.SetActive(true);

        ActivateAllButtonPlayerScreen();
    }

    public void ActivateAllButtonPlayerScreen()
    {
        playGameButton.interactable = true;
        tournamentButton.interactable = true;
        estadisticsButton.interactable = true;
        exitButton.interactable = true;
    }

    public void DesactivateAllButtonPlayerScreen()
    {
        playGameButton.interactable = false;
        tournamentButton.interactable = false;
        estadisticsButton.interactable = false;
        exitButton.interactable = false;
    }

    public void OnBackToPlayerScreen()
    {
        gameScreen.SetActive(false);
        playerScreen.SetActive(true);
        ActivateAllButtonPlayerScreen();
    }


}
