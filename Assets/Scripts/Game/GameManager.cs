using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    [HideInInspector]
    public static GameManager instance;

    [Header("Players Info")]
    public TextMeshProUGUI player1Nickname;
    public TextMeshProUGUI player2Nickname;
    public TextMeshProUGUI[] playerScoresUI;

    [Header("Players")]
    public string playerPrefabLocation;
    public Transform goalKeeperSpawn;
    public Transform kickerSpawn;
    public PlayerController[] players;
    private int playersInGame;

    [Header("Game Settings")]
    public GameObject[] goalBounds;
    public GameObject[] missedGoalBounds;

    [Header("Animators")]
    public Animator goalAnim;
    public Animator missedGoalAnim;

    [Header("Audio Effects")]
    public AudioSource missedGoalSound;
    public AudioSource celebrationGoalSound;
    public AudioSource kickSound;

    private int[] scores;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        scores = new int[2];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void ImInGame()
    {
        playersInGame++;

        if(playersInGame == PhotonNetwork.PlayerList.Length)
        {
            SpawnPlayer();
        }
    }

    void SpawnPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, Vector3.one, Quaternion.identity);

        PlayerController playerScript = playerObj.GetComponent<PlayerController>();

        //initialize the player
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    //Goal and missed Goals
    [PunRPC]
    public void MarkGoalToPlayer(int id)
    {
        scores[id]++;
        playerScoresUI[id].text = scores[id].ToString();
        celebrationGoalSound.Play();
        missedGoalSound.Stop();
        StartCoroutine(DeactivateGoalBounds());
    }

    [PunRPC]
    public void MarkGoalMissedToPlayer()
    {
        celebrationGoalSound.Stop();
        missedGoalSound.Play();
        StartCoroutine(DeactivateMissedGoalBounds());
    }

    //[PunRPC]
    public void SwitchPositions()
    {
        foreach(PlayerController player in players)
        {
            if (!player.hasToChange)
                return;
            if(player.isGoalKeeper)
            {
                spawnAsKicker(player);
            }
            else
            {
                spawnAsGoalKeeper(player);
            }
            player.hasToChange = false;
        }
    }

    public void spawnAsGoalKeeper(PlayerController player)
    {
        player.isGoalKeeper = true;
        player.gameObject.transform.position = goalKeeperSpawn.position;
        player.gameObject.transform.rotation = goalKeeperSpawn.rotation;
        player.ball.SetActive(false);
    }

    public void spawnAsKicker(PlayerController player)
    {
        player.isGoalKeeper = false;
        player.gameObject.transform.position = kickerSpawn.position;
        player.gameObject.transform.rotation = kickerSpawn.rotation;
        player.ball.SetActive(true);
    }
    
    //Goal Bounds
    IEnumerator DeactivateMissedGoalBounds()
    {
        DeactivateBounds();
        missedGoalAnim.SetTrigger("missedgoal");
        yield return new WaitForSeconds(2);
        ActivateBounds();
    }

    IEnumerator DeactivateGoalBounds()
    {
        DeactivateBounds();
        goalAnim.SetTrigger("goal");
        yield return new WaitForSeconds(2.8f);
        ActivateBounds();
    }

    void DeactivateBounds()
    {
        foreach (GameObject missedGoalBound in missedGoalBounds)
        {
            missedGoalBound.SetActive(false);
        }
        foreach (GameObject goalBound in goalBounds)
        {
            goalBound.SetActive(false);
        }
    }

    void ActivateBounds()
    {
        foreach (GameObject missedGoalBound in missedGoalBounds)
        {
            missedGoalBound.SetActive(true);
        }
        foreach (GameObject goalBound in goalBounds)
        {
            goalBound.SetActive(true);
        }
    }
}
