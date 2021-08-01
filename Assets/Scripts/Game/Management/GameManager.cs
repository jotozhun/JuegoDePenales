using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("GameComponents")]
    public GameUI gameUI;

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
    public GameObject goalScreen;
    public GameObject failedGoalScreen;

    [Header("Animators")]
    public Animator goalAnim;
    public Animator missedGoalAnim;


    public int[] scores;

    private Timer timScript;

    private CountKicks kickScipt;

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
        gameUI.players = new PlayerController[PhotonNetwork.PlayerList.Length];
        scores = new int[2];

        photonView.RPC("ImInGame", RpcTarget.All);
    }

    [PunRPC]
    public void spawnAsEndMatch(Player winnerPlayer, Player loserPlayer, int winnerScore, int loseScore)
    {
        gameUI.surrenderButton.gameObject.SetActive(false);
        gameUI.surrenderScreen.SetActive(false);
        temporalEndGame = true;
        foreach (PlayerController player in gameUI.players)
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
                tmpTransf = gameUI.GetDidWinSpawn(true);
                player.kicker_anim.SetBool("DidWin", true);
            }
            else
            {
                tmpTransf = gameUI.GetDidWinSpawn(false);
                player.kicker_anim.SetBool("DidLose", true);
            }
            player.transform.position = tmpTransf.position;
            player.transform.rotation = tmpTransf.rotation;
        }
        
        if(PhotonNetwork.IsMasterClient)
        {
            bool isTorneo = (bool) PhotonNetwork.CurrentRoom.CustomProperties["isTorneo"];
            NetworkManager.instance.CrearDueloNormal(winnerPlayer, loserPlayer, Convert.ToInt32(isTorneo));
        }


        if(PhotonNetwork.LocalPlayer.ActorNumber == winnerPlayer.ActorNumber)
        {
            NetworkManager.instance.AddEstadisticasToLocal(true);
            StartCoroutine(gameUI.ActivateWinnerScreen(winnerScore, loseScore, winnerPlayer, loserPlayer));
        }
        else
        {
            NetworkManager.instance.AddEstadisticasToLocal(false);
            StartCoroutine(gameUI.ActivateLoserScreen(winnerScore, loseScore, winnerPlayer, loserPlayer));
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
            int indexOfKicker = NetworkManager.instance.userLogin.player;
            GameObject playerObj = PhotonNetwork.Instantiate(gameUI.playersPrefabLocation[indexOfKicker], Vector3.one, Quaternion.identity);
            PlayerController playerScript = playerObj.GetComponent<PlayerController>();

            //initialize the player

            playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);

            gameUI.photonView.RPC("InitializeGoalContainers", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber - 1);
            Clock.instance.started = true;
        }
        else
        {
            gameUI.spawnAsSpectator();
        }
    }
    

    //Goal and missed Goals
    [PunRPC]
    public void MarkGoalToPlayer(Player player)
    {
        if ((bool)player.CustomProperties["hasMarkedAResult"])
            return;

        if (!(bool)PhotonNetwork.LocalPlayer.CustomProperties["isGoalkeeper"])
            ActivateGoalScreen();

        player.CustomProperties["hasMarkedAResult"] = true;
        //markGoal = true;
        int id = player.ActorNumber - 1;
        bool isDeathmatchTime = (bool)player.CustomProperties["isDeathMatchTime"];
        int goalNumber = (int)player.CustomProperties["Goals"] + 1;
        int kicksLeft = (int)player.CustomProperties["KicksLeft"];
        player.CustomProperties["Goals"] = goalNumber;
        player.CustomProperties["KicksLeft"] = kicksLeft - 1;
        numberKicks++;
        scores[id]++;
        playerScoresUI[id].text = scores[id].ToString();
        gameUI.celebrationGoalSound.Play();
        gameUI.missedGoalSound.Stop();
        if (isDeathmatchTime)
            gameUI.CalculateWin(player);
        else
            gameUI.MarkGoalUI(player, kicksLeft);
        //StartCoroutine(DeactivateGoalBounds());
        DeactivateBounds();
        //goalAnim.SetTrigger("goal");
    }

    [PunRPC]
    public void MarkSavedGoalToPlayer(Player player)
    {
        if ((bool)player.CustomProperties["hasMarkedAResult"])
            return;

        if (!(bool)PhotonNetwork.LocalPlayer.CustomProperties["isGoalkeeper"])
            ActivateFailedGoalScreen();

        player.CustomProperties["hasMarkedAResult"] = true;
        int id = player.ActorNumber - 1;
        bool isDeathmatchTime = (bool)player.CustomProperties["isDeathMatchTime"];
        int savedGoals = (int)player.CustomProperties["SavedGoals"] + 1;
        int kicksLeft = (int)player.CustomProperties["KicksLeft"];
        player.CustomProperties["SavedGoals"] = savedGoals;
        player.CustomProperties["KicksLeft"] = kicksLeft - 1;
        numberKicks++;
        //scores[id]++;
        //playerScoresUI[id].text = scores[id].ToString();
        gameUI.celebrationGoalSound.Stop();
        gameUI.missedGoalSound.Play();
        if (isDeathmatchTime)
            gameUI.CalculateWin(player);
        else
            gameUI.MarkSavedGoalUI(player, kicksLeft);
        DeactivateBounds();
    }

    [PunRPC]
    public void MarkGoalMissedToPlayer(Player player)
    {
        if ((bool)player.CustomProperties["hasMarkedAResult"])
            return;

        if (!(bool)PhotonNetwork.LocalPlayer.CustomProperties["isGoalkeeper"])
            ActivateFailedGoalScreen();

        player.CustomProperties["hasMarkedAResult"] = true;
        //missGoal = true;
        int id = player.ActorNumber - 1;
        bool isDeathmatchTime = (bool)player.CustomProperties["isDeathMatchTime"];
        int failedGoals = (int)player.CustomProperties["FailedGoals"] + 1;
        int kicksLeft = (int)player.CustomProperties["KicksLeft"];
        player.CustomProperties["FailedGoals"] = failedGoals;
        player.CustomProperties["KicksLeft"] = kicksLeft - 1;
        gameUI.celebrationGoalSound.Stop();
        gameUI.missedGoalSound.Play();
        if (isDeathmatchTime)
            gameUI.CalculateWin(player);
        else
            gameUI.MarkFailedGoalUI(player, kicksLeft);
        //StartCoroutine(DeactivateMissedGoalBounds());
        DeactivateBounds();
        //missedGoalAnim.SetTrigger("missedgoal");
    }

    [PunRPC]
    public void SwitchPositions()
    {
        //markGoal = false;
        //missGoal = false;
        DeactivateGoalScreens();
        if (temporalEndGame == true)
            return;
        foreach (PlayerController player in gameUI.players)
        {
            player.hasToChange = true;
            
            if (player.isGoalKeeper)
            {
                spawnAsKicker(player);
            }
            else
            {
                spawnAsGoalKeeper(player);
            }
        }
        Clock.instance.photonView.RPC("RestartTime", RpcTarget.All);
    }

    public void spawnAsGoalKeeper(PlayerController player)
    {
        player.isGoalKeeper = true;
        player.canCover = true;
        player.ChangeRol(true);
        player.photonPlayer.CustomProperties["isGoalkeeper"] = true;
    }

    public void spawnAsKicker(PlayerController player)
    {
        player.playerCanCover = false;
        player.isGoalKeeper = false;
        player.canCover = false;
        player.ChangeRol(false);
        player.ballReturned = true;
        player.triggerForKick.enabled = true;
        player.photonPlayer.CustomProperties["hasMarkedAResult"] = false;

        player.photonPlayer.CustomProperties["isGoalkeeper"] = false;
    }

    public void ActivateGoalScreen()
    {
        goalScreen.SetActive(true);
    }

    public void ActivateFailedGoalScreen()
    {
        failedGoalScreen.SetActive(true);
    }

    public void DeactivateGoalScreens()
    {
        goalScreen.SetActive(false);
        failedGoalScreen.SetActive(false);
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
    public void keeperCanCover(bool cover)
    {
        foreach (PlayerController player in gameUI.players)
        {
            if (player.isGoalKeeper)
            {
                player.playerCanCover = cover;
            }
        }
        ActivateBounds();
    }

}
