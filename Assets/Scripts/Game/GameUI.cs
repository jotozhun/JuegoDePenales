using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.UI;

public class GameUI : MonoBehaviourPunCallbacks
{
    [HideInInspector]
    public static GameUI instance;

    [Header("Players")]
    public string playerPrefabLocation;
    //public Transform goalKeeperSpawn;
    //public Transform kickerSpawn;
    public Transform playerSpawn;
    public PlayerController[] players;
    public PlayerController playerObject;
    //public int playersInGame;

    [Header("Audio Effects")]
    public AudioSource missedGoalSound;
    public AudioSource celebrationGoalSound;
    public AudioSource kickSound;

    [Header("Timer")]
    public GameObject time;

    [Header("Kicks")]
    public GameObject kick;

    [Header("Buttons")]
    public Button surrenderButton;

    [Header("Screens")]
    public GameObject surrenderScreen;

    private void Awake()
    {
        instance = this;
    }

    public void OpenSurrenderScreen(bool state)
    {
        surrenderScreen.SetActive(state);
    }

    
    public void OnAcceptExitButton()
    {
        photonView.RPC("OnEndGame", RpcTarget.All);
    }

    [PunRPC]
    public void OnEndGame()
    {
        StartCoroutine(LeaveGameRoom());
    }
    
    IEnumerator LeaveGameRoom()
    {
        PhotonNetwork.LeaveRoom();
        while(PhotonNetwork.InRoom)
            yield return null;
        Screen.orientation = ScreenOrientation.Portrait;
        PhotonNetwork.LoadLevel("Menu");
    }
}
