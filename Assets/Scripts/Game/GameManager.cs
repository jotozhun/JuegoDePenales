using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public static GameManager instance;

    [Header("Players Info")]
    public PlayerController[] players;
    public TextMeshProUGUI player1Nickname;
    public TextMeshProUGUI player2Nickname;
    public TextMeshProUGUI[] playerScoresUI;

    [Header("Game Settings")]
    public GameObject[] goalBounds;
    public GameObject[] missedGoalBounds;

    [Header("Animators")]
    public Animator goalAnim;
    public Animator missedGoalAnim;

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
        StartCoroutine(DeactivateGoalBounds());
    }

    public void MarkGoalMissedToPlayer()
    {
        StartCoroutine(DeactivateMissedGoalBounds());
    }

    IEnumerator DeactivateMissedGoalBounds()
    {
        foreach (GameObject missedGoalBound in missedGoalBounds)
        {
            missedGoalBound.SetActive(false);
        }
        missedGoalAnim.SetTrigger("missedgoal");
        yield return new WaitForSeconds(2);
        foreach (GameObject missedGoalBound in missedGoalBounds)
        {
            missedGoalBound.SetActive(true);
        }
    }

    IEnumerator DeactivateGoalBounds()
    {
        foreach(GameObject goalBound in goalBounds)
        {
            goalBound.SetActive(false);
        }
        goalAnim.SetTrigger("goal");
        yield return new WaitForSeconds(2.8f);
        foreach (GameObject goalBound in goalBounds)
        {
            goalBound.SetActive(true);
        }
    }
}
