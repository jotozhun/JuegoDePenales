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
    public TextMeshProUGUI player1ScoreUI;
    public TextMeshProUGUI player2ScoreUI;

    private int player1Score;
    private int player2Score;

    private void Awake()
    {
        instance = this;
    }

    public void MarkGoalToPlayer(int id)
    {

    }
}
