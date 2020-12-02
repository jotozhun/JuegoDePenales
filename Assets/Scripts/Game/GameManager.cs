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

    private int playersInGame;

    [Header("Game Settings")]
    public GameObject[] goalBounds;
    public GameObject[] missedGoalBounds;

    [Header("Animators")]
    public Animator goalAnim;
    public Animator missedGoalAnim;


    private int[] scores;

    private Timer timScript;

    private CountKicks kickScipt;
    public bool markGoal;
    public bool missGoal;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        markGoal = false;
        missGoal = false;
        GameUI.instance.players = new PlayerController[PhotonNetwork.PlayerList.Length];
        scores = new int[2];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
        timScript = GameUI.instance.time.GetComponent<Timer>();
        kickScipt = GameUI.instance.kick.GetComponent<CountKicks>();
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
        GameObject playerObj = PhotonNetwork.Instantiate(GameUI.instance.playerPrefabLocation, Vector3.one, Quaternion.identity);

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
        markGoal = true;
        //Physics2D.gravity = new Vector2(0f, -7.51f);
        kickScipt.DecreaseKicks();
        scores[id]++;
        playerScoresUI[id].text = scores[id].ToString();
        GameUI.instance.celebrationGoalSound.Play();
        GameUI.instance.missedGoalSound.Stop();
        StartCoroutine(DeactivateGoalBounds());
    }

    [PunRPC]
    public void MarkGoalMissedToPlayer()
    {
        missGoal = true;
        //Physics2D.gravity = new Vector2(0f, -7.51f);
        kickScipt.DecreaseKicks();
        GameUI.instance.celebrationGoalSound.Stop();
        GameUI.instance.missedGoalSound.Play();
        StartCoroutine(DeactivateMissedGoalBounds());
    }

    [PunRPC]
    public void SwitchPositions()
    {
        markGoal = false;
        missGoal = false;
        foreach (PlayerController player in GameUI.instance.players)
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
        player.gameObject.transform.position = GameUI.instance.goalKeeperSpawn.position;
        player.gameObject.transform.rotation = GameUI.instance.goalKeeperSpawn.rotation;
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
        player.playerCanCover = false;
        player.isGoalKeeper = false;
        player.canCover = false;
        //player.anim.SetBool("isGoalKeeper", false);
        player.gameObject.transform.position = GameUI.instance.kickerSpawn.position;
        player.gameObject.transform.rotation = GameUI.instance.kickerSpawn.rotation;
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

    [PunRPC]
    public void keeperCanCover(bool cover)
    {
        foreach (PlayerController player in GameUI.instance.players)
        {
            if (player.isGoalKeeper)
            {
                player.playerCanCover = cover;
            }
        }
    }
}
