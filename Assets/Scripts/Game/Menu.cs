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
    public GameObject mainScreen;
    public GameObject waitingScreen;

    [Header("Main Screen")]
    public TMP_InputField playerName;
    public Button playButton;

    [Header("Waiting Room")]
    public Button cancelButton;


    // MAIN SCREEN
    public void OnPlayerNameChanged()
    {
        PhotonNetwork.NickName = playerName.text;
    }
    public void OnPlayButton()
    {
        playButton.interactable = false;
        NetworkManager.instance.CreateRoom();
    }

    public override void OnCreatedRoom()
    {
        playButton.interactable = true;
        mainScreen.SetActive(false);
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
        mainScreen.SetActive(true);
        waitingScreen.SetActive(false);
    }
}
