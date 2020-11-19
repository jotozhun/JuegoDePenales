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
    public TextMeshProUGUI[] playersNickname;
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

    [Header("Timer")]
    public GameObject time;
    private Timer timScript;

    [Header("Kicks")]
    public GameObject kick;
    private CountKicks kickScipt;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        scores = new int[2];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
        timScript = time.GetComponent<Timer>();
        kickScipt = kick.GetComponent<CountKicks>();
    }

    [PunRPC]
    void ImInGame()
    {
        playersInGame++;

        if(PhotonNetwork.IsMasterClient && playersInGame == PhotonNetwork.PlayerList.Length)
        {
            photonView.RPC("SpawnPlayers", RpcTarget.All);
        }
    }

    [PunRPC]
    void SpawnPlayers()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, Vector3.one, Quaternion.identity);

        PlayerController playerScript = playerObj.GetComponent<PlayerController>();

        //initialize the player
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);

        Timer timerScript = playerObj.GetComponent<Timer>();

        timerScript.photonView.RPC("Start", RpcTarget.All, PhotonNetwork.LocalPlayer);

        CountKicks kcikScript = playerObj.GetComponent<CountKicks>();

        kcikScript.photonView.RPC("Start", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    //Goal and missed Goals
    [PunRPC]
    public void MarkGoalToPlayer(int id)
    {
        kickScipt.DecreaseKicks();
        scores[id]++;
        playerScoresUI[id].text = scores[id].ToString();
        celebrationGoalSound.Play();
        missedGoalSound.Stop();
        StartCoroutine(DeactivateGoalBounds());
    }

    [PunRPC]
    public void MarkGoalMissedToPlayer()
    {
        kickScipt.DecreaseKicks();
        celebrationGoalSound.Stop();
        missedGoalSound.Play();
        StartCoroutine(DeactivateMissedGoalBounds());
    }

    [PunRPC]
    public void SwitchPositions()
    {
        foreach(PlayerController player in players)
        {
            player.hasToChange = true;
            if(player.isGoalKeeper)
            {
                spawnAsKicker(player);
            }
            else
            {
                spawnAsGoalKeeper(player);
            }
        }
    }

    public void spawnAsGoalKeeper(PlayerController player)
    {
        player.isGoalKeeper = true;
        //player.anim.SetBool("isGoalKeeper", true);
        player.gameObject.transform.position = goalKeeperSpawn.position;
        player.gameObject.transform.rotation = goalKeeperSpawn.rotation;
        player.ball.SetActive(false);
        player.canCover = true;
        timScript.R();
        timScript.StartTime();
        kickScipt.RestartKicks();
        if (PhotonNetwork.IsMasterClient)
        {
            timScript.keeper = true;
        }
    }

    public void spawnAsKicker(PlayerController player)
    {
        player.isGoalKeeper = false;
        player.canCover = false;
        //player.anim.SetBool("isGoalKeeper", false);
        player.gameObject.transform.position = kickerSpawn.position;
        player.gameObject.transform.rotation = kickerSpawn.rotation;
        player.ball.SetActive(true);
        timScript.R();
        timScript.StartTime();
        kickScipt.RestartKicks();
        if (PhotonNetwork.IsMasterClient)
        {
            timScript.keeper = false;
        }
    }
    
    //Goal Bounds
    IEnumerator DeactivateMissedGoalBounds()
    {
        DeactivateBounds();
        missedGoalAnim.SetTrigger("missedgoal");
        yield return new WaitForSeconds(4);
        ActivateBounds();
    }

    IEnumerator DeactivateGoalBounds()
    {
        DeactivateBounds();
        goalAnim.SetTrigger("goal");
        yield return new WaitForSeconds(4.8f);
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

    public void ActivateBounds()
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

    [PunRPC]
    public void stopTime()
    {
        timScript.StopTime();
    }

    [PunRPC]
    public bool activateSwitch()
    {
        if (kickScipt.numKicks == 0)
        {
            return true;
        }
        return false;
    }

    [PunRPC]
    public void restartTime()
    {
        timScript.R();
        timScript.StartTime();
    }
}
