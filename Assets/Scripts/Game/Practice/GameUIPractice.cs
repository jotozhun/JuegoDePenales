﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.UI;

public class GameUIPractice : MonoBehaviour
{
    [HideInInspector]
    public static GameUIPractice instance;

    [Header("Players")]
    public string playerPrefabLocation;
    public Transform goalKeeperSpawn;
    public Transform kickerSpawn;
    public PlayerControllerPractice[] players;
    public PlayerControllerPractice playerObject;
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
