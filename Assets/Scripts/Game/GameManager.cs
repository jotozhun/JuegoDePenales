using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
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
    public GameObject[] goalContainers;
    public Image[] playerEmblemas;
    public int[] emblemasIndexs;

    [HideInInspector]
    public int playersInGame;

    [Header("Game Settings")]
    public GameObject[] goalBounds;
    public GameObject[] missedGoalBounds;
    
    

    [Header("Animators")]
    public Animator goalAnim;
    public Animator missedGoalAnim;


    public int[] scores;

    private Timer timScript;

    private CountKicks kickScipt;
    //public bool markGoal;
    //public bool missGoal;

    public int numberKicks;            //Change the number kicks of players
    [Header("EndGame")]
    public GameObject endGameCam;
    public bool temporalEndGame;
    private void Awake()
    {
        temporalEndGame = false;
        instance = this;
    }
    private void Start()
    {
        GameUI.instance.players = new PlayerController[PhotonNetwork.PlayerList.Length];
        scores = new int[2];
        photonView.RPC("ImInGame", RpcTarget.All);
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        /*
        if(temporalEndGame)
        {
            photonView.RPC("spawnAsEndMatch", RpcTarget.All);
        }
        */
    }

    [PunRPC]
    public void spawnAsEndMatch(Player winnerPlayer, Player loserPlayer, int winnerScore, int loseScore)
    {
        temporalEndGame = true;
        foreach (PlayerController player in GameUI.instance.players)
        {
            Transform tmpTransf = null;
            player.ball.SetActive(false);
            player.goalkeeper_obj.SetActive(false);
            player.goalkeeper_cam_obj.SetActive(false);
            player.kicker_cam_obj.SetActive(false);
            player.kicker_obj.SetActive(true);
            endGameCam.SetActive(true);
            if (player.id == winnerPlayer.ActorNumber)
            {
                tmpTransf = GameUI.instance.GetDidWinSpawn(true);
                player.kicker_anim.SetBool("DidWin", true);
            }
            else
            {
                tmpTransf = GameUI.instance.GetDidWinSpawn(false);
                player.kicker_anim.SetBool("DidLose", true);
            }
            player.transform.position = tmpTransf.position;
            player.transform.rotation = tmpTransf.rotation;
        }

        if(PhotonNetwork.LocalPlayer.ActorNumber == winnerPlayer.ActorNumber)
        {
            StartCoroutine(GameUI.instance.ActivateWinnerScreen(winnerScore, loseScore));
        }
        else
        {
            StartCoroutine(GameUI.instance.ActivateLoserScreen(winnerScore, loseScore));
        }
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
        if (PhotonNetwork.LocalPlayer.ActorNumber != 3)
        {
            //GameObject playerObj = PhotonNetwork.Instantiate(GameUI.instance.playerPrefabLocation, Vector3.one, Quaternion.identity);
            int indexOfKicker = NetworkManager.instance.kicker_index;
            GameObject playerObj = PhotonNetwork.Instantiate(GameUI.instance.playersPrefabLocation[indexOfKicker], Vector3.one, Quaternion.identity);
            PlayerController playerScript = playerObj.GetComponent<PlayerController>();

            //initialize the player

            playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);

            GameUI.instance.photonView.RPC("InitializeGoalContainers", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber - 1);
            //Timer timerScript = playerObj.GetComponent<Timer>();

            //CountKicks kcikScript = playerObj.GetComponent<CountKicks>();
        }
        else
        {
            GameUI.instance.spawnAsSpectator();
        }
    }

    //Goal and missed Goals
    [PunRPC]
    public void MarkGoalToPlayer(Player player)
    {
        //markGoal = true;
        int id = player.ActorNumber - 1;
        int goalNumber = (int)player.CustomProperties["Goals"] + 1;
        int kicksLeft = (int)player.CustomProperties["KicksLeft"];
        player.CustomProperties["Goals"] = goalNumber;
        player.CustomProperties["KicksLeft"] = kicksLeft - 1;
        numberKicks++;
        scores[id]++;
        playerScoresUI[id].text = scores[id].ToString();
        GameUI.instance.celebrationGoalSound.Play();
        GameUI.instance.missedGoalSound.Stop();
        GameUI.instance.MarkGoalUI(player, kicksLeft);
        //StartCoroutine(DeactivateGoalBounds());
        DeactivateBounds();
        goalAnim.SetTrigger("goal");
    }

    [PunRPC]
    public void MarkSavedGoalToPlayer(Player player)
    {
        int id = player.ActorNumber - 1;
        int savedGoals = (int)player.CustomProperties["SavedGoals"] + 1;
        int kicksLeft = (int)player.CustomProperties["KicksLeft"];
        player.CustomProperties["SavedGoals"] = savedGoals;
        player.CustomProperties["KicksLeft"] = kicksLeft - 1;
        numberKicks++;
        scores[id]++;
        playerScoresUI[id].text = scores[id].ToString();
        GameUI.instance.celebrationGoalSound.Stop();
        GameUI.instance.missedGoalSound.Play();
        GameUI.instance.MarkSavedGoalUI(player, kicksLeft);
    }

    [PunRPC]
    public void MarkGoalMissedToPlayer(Player player)
    {
        //missGoal = true;
        int id = player.ActorNumber - 1;
        int failedGoals = (int)player.CustomProperties["FailedGoals"] + 1;
        int kicksLeft = (int)player.CustomProperties["KicksLeft"];
        player.CustomProperties["FailedGoals"] = failedGoals;
        player.CustomProperties["KicksLeft"] = kicksLeft - 1;
        GameUI.instance.celebrationGoalSound.Stop();
        GameUI.instance.missedGoalSound.Play();
        GameUI.instance.MarkFailedGoalUI(player, kicksLeft);
        //StartCoroutine(DeactivateMissedGoalBounds());
        DeactivateBounds();
        missedGoalAnim.SetTrigger("missedgoal");
    }

    [PunRPC]
    public void SwitchPositions()
    {
        //markGoal = false;
        //missGoal = false;
        if (temporalEndGame == true)
            return;
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
        Clock.instance.RestartTime();
    }

    public void spawnAsGoalKeeper(PlayerController player)
    {
        player.isGoalKeeper = true;
        player.canCover = true;
        player.ChangeRol(true);
    }

    public void spawnAsKicker(PlayerController player)
    {
        player.playerCanCover = false;
        player.isGoalKeeper = false;
        player.canCover = false;
        player.ChangeRol(false);
    }

    
    
    //Goal Bounds
    /*
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

    */
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
    public void keeperCanCover(bool cover)
    {
        foreach (PlayerController player in GameUI.instance.players)
        {
            if (player.isGoalKeeper)
            {
                player.playerCanCover = cover;
            }
        }
        ActivateBounds();
    }

    
    /*
    [PunRPC]
    public void stopTime()
    {
        timScript.StopTime();
    }

    [PunRPC]
    public bool activateSwitch()
    {
        //Debug.Log("kicks:" + kickScipt.numKicks);
        if (kickScipt.numKicks == 0)
        {
            return true;
        }
        return false;
    }
    
    [PunRPC]
    public void decreaseKicksCount()
    {
        kickScipt.DecreaseKicks();
    }

    [PunRPC]
    public void restartKicksCount()
    {
        kickScipt.RestartKicks();
    }

    [PunRPC]
    public void restartTime()
    {
        timScript.R();
        timScript.StartTime();
    }
    */

    /*
    [PunRPC]
    public bool DrawGame()
    {
        //Debug.Log(scores[0]);
        //Debug.Log(scores[1]);
        if (scores[0] == scores[1])
            return true;
        return false;

    }

    [PunRPC]
    public bool WinGame()
    {
        int lastKicks = kickScipt.numKicks;
        int canWin1 = lastKicks + scores[0];
        int canWin2 = lastKicks + scores[1];
        //Debug.Log(canWin1);
        //Debug.Log(canWin2);
        //Debug.Log(lastKicks);
        if ( (scores[0] > lastKicks && scores[0] > canWin2) || (scores[1] > lastKicks && scores[1] > canWin1))
            return true;
        return false;

    }

    */
}
