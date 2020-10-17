using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager instance;

    [Header("PlayersInfo")]
    public PlayerController[] players;
    public TextMeshProUGUI player1Nickname;
    public TextMeshProUGUI player2Nickname;
    public TextMeshProUGUI[] playerScoresUI;

    private int[] scores;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        scores = new int[2];
    }
    public void MarkGoalToPlayer(int id)
    {
        scores[id]++;
        playerScoresUI[id].text = scores[id].ToString();
    }
}
