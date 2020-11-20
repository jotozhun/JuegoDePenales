using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameUI : MonoBehaviourPunCallbacks
{
    [HideInInspector]
    public static GameUI instance;

    [Header("Players")]
    public string playerPrefabLocation;
    public Transform goalKeeperSpawn;
    public Transform kickerSpawn;
    public PlayerController[] players;
    //public int playersInGame;

    [Header("Audio Effects")]
    public AudioSource missedGoalSound;
    public AudioSource celebrationGoalSound;
    public AudioSource kickSound;

    [Header("Timer")]
    public GameObject time;

    [Header("Kicks")]
    public GameObject kick;

    private void Awake()
    {
        instance = this;
    }
}
