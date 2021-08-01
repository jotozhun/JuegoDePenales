using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.UI;
using AccountModels;

public class GameUI : MonoBehaviourPunCallbacks
{
    [HideInInspector]
    public static GameUI instance;

    [Header("Players")]
    public string playerPrefabLocation;
    public string[] playersPrefabLocation;
    public Sprite[] playerEmblemas;
    //public Transform goalKeeperSpawn;
    //public Transform kickerSpawn;
    public Transform playerSpawn;
    public Transform playerWinSpawn;
    public Transform playerLoseSpawn;
    public PlayerController[] players;
    public PlayerController playerObject;
    public Transform[] spectatorCameras;
    public GameObject spectator_cam_obj;
    //public int playersInGame;

    [Header("Audio Effects")]
    public AudioSource missedGoalSound;
    public AudioSource celebrationGoalSound;
    public AudioSource kickSound;


    [Header("UI Scores")]
    public string goalTypeRoute;
    public GameObject goalTypePrefab;
    public GameObject[] goalContainers;
    public Image[] goalTypeSpritesCont1;
    public Image[] goalTypeSpritesCont2;
    public Sprite goalSprite;
    public Sprite savedSprite;
    public Sprite failedSprite;
    public TextMeshProUGUI winnerMsg;
    public GameObject spectatorCanvas;

    [Header("Win Game Screen")]
    public GameObject winScreen;
    public TextMeshProUGUI winScoreText;
    public Button winExitButton;

    [Header("Lose Game Screen")]
    public GameObject loseScreen;
    public TextMeshProUGUI LoseScoreText;
    public Button loseExitButton;
    
    //Score logic
    private int[] goalScores = { 0, 0};
    private int[] savedScores = { 0, 0 };
    private int[] failedScores = { 0, 0 };

    private int[] kicksLeft = new int[2];
    private int[] indexOfEmblema = new int[2];
    private int[] indexOfPeinado = new int[2];
    private int numberOfGoals;
    public bool isSuddenDeath = false;
    /*[Header("Timer")]
    public GameObject time;
    */
    [Header("Buttons")]
    public Button surrenderButton;

    [Header("Screens")]
    public GameObject surrenderScreen;

    private void Awake()
    {
        instance = this;
    }

    [PunRPC]
    public void InitializeGoalContainers(int id)
    {
        if((bool)PhotonNetwork.CurrentRoom.CustomProperties["isTorneo"])
        {
            numberOfGoals = NetworkManager.instance.userLogin.duelo_agendado.numero_inicial_goles;
        }
        else
        {
            numberOfGoals = NetworkManager.instance.numberOfGoals;
        }
        
        if (id == 0)
        {
            kicksLeft[0] = numberOfGoals;
            goalTypeSpritesCont1 = new Image[numberOfGoals];
        }
        else if(id == 1)
        {
            kicksLeft[1] = numberOfGoals;
            goalTypeSpritesCont2 = new Image[numberOfGoals];
        }
        for (int i = 0; i < numberOfGoals; i++)
        {
            //Instantiation
            GameObject tmpGoalTypeObj = Instantiate(goalTypePrefab);
            if(id == 0)
            {
                tmpGoalTypeObj.transform.SetParent(goalContainers[id].transform);
                goalTypeSpritesCont1[i] = tmpGoalTypeObj.GetComponent<Image>();
            }
            else if(id == 1)
            {
                tmpGoalTypeObj.transform.SetParent(goalContainers[id].transform);
                goalTypeSpritesCont2[i] = tmpGoalTypeObj.GetComponent<Image>();
            }
        }   
    }

    public void MarkGoalUI(Player player, int kicksLeft)
    {
        //goalScores[idPlayer] += 1;
        if (player.ActorNumber == 1)
        {
            goalTypeSpritesCont1[numberOfGoals-kicksLeft].sprite = goalSprite;
        }
        else if(player.ActorNumber == 2)
        {
            goalTypeSpritesCont2[numberOfGoals - kicksLeft].sprite = goalSprite;
        }
        CalculateWin(player);
    }

    public void MarkSavedGoalUI(Player player, int kicksLeft)
    {
        if (player.ActorNumber == 1)
        {
            goalTypeSpritesCont1[numberOfGoals - kicksLeft].sprite = savedSprite;
        }
        else if (player.ActorNumber == 2)
        {
            goalTypeSpritesCont2[numberOfGoals - kicksLeft].sprite = savedSprite;
        }
        CalculateWin(player);
    }

    public void MarkFailedGoalUI(Player player, int kicksLeft)
    {
        if (player.ActorNumber == 1)
        {
            goalTypeSpritesCont1[numberOfGoals - kicksLeft].sprite = failedSprite;
        }
        else if (player.ActorNumber == 2)
        {
            goalTypeSpritesCont2[numberOfGoals - kicksLeft].sprite = failedSprite;
        }
        CalculateWin(player);
    }

    public void CalculateWin(Player player)
    {
        Player otherPlayer = null;
        foreach(PlayerController controller in players)
        {
            if (controller.photonPlayer.ActorNumber != player.ActorNumber)
                otherPlayer = controller.photonPlayer;
        }

        //Player who achieved, missed or failed a goal
        int actualGoals = (int)player.CustomProperties["Goals"];
        int actualKicksLeft = (int)player.CustomProperties["KicksLeft"];
        //The player who was the goalkeeper
        int otherGoals = (int)otherPlayer.CustomProperties["Goals"];
        int otherKicksLeft = (int)otherPlayer.CustomProperties["KicksLeft"];

        bool isDeathmatchTime = calculateDeathMatch(actualGoals, actualKicksLeft, otherGoals, otherKicksLeft);

        bool actualPlayerWon = actualGoals > otherGoals + otherKicksLeft;
        bool otherPlayerWon = otherGoals > actualGoals + actualKicksLeft;

        if (isDeathmatchTime)
        {
            player.CustomProperties["isDeathMatchTime"] = true;
            otherPlayer.CustomProperties["isDeathMatchTime"] = true;
            player.CustomProperties["KicksLeft"] = 1;
            otherPlayer.CustomProperties["KicksLeft"] = 1;
            actualPlayerWon = false;
            otherPlayerWon = false;
        }
        else
        {
            if (actualPlayerWon)
            {
                //GameManager.instance.spawnAsEndMatch(player, otherPlayer, actualGoals, otherGoals);
                StartCoroutine(SpawnEndMatchCoroutine(player, otherPlayer, actualGoals, otherGoals));
            }
            else if (otherPlayerWon)
            {
                StartCoroutine(SpawnEndMatchCoroutine(otherPlayer, player, otherGoals, actualGoals));
                //GameManager.instance.spawnAsEndMatch(otherPlayer, player, otherGoals, actualGoals);
            }
        }
    }

    IEnumerator SpawnEndMatchCoroutine(Player winnerPlayer, Player loserPlayer, int winnerScore, int loseScore)
    {
        yield return new WaitForSeconds(3);

        GameManager.instance.spawnAsEndMatch(winnerPlayer, loserPlayer, winnerScore, loseScore);
    }

    bool calculateDeathMatch(int actualGoals, int actualKicksLeft, int otherGoals, int otherKicksLeft)
    {
        return (actualKicksLeft == 0 && otherKicksLeft == 0 && actualGoals == otherGoals);
    }


    public void OpenSurrenderScreen(bool state)
    {
        surrenderScreen.SetActive(state);
    }

    public void spawnAsSpectator()
    {
        spectator_cam_obj.SetActive(true);
        spectatorCanvas.SetActive(true);
        surrenderButton.gameObject.SetActive(false);
    }

    public void OnCameraPosition(int pos)
    {
        spectator_cam_obj.transform.position = spectatorCameras[pos].localPosition;
        spectator_cam_obj.transform.rotation = spectatorCameras[pos].localRotation;
    }
    
    public void OnAcceptExitButton()
    {
        photonView.RPC("OnEndGame", RpcTarget.All);
    }

    [PunRPC]
    public void OnShowWinner(Player player)
    {
        StartCoroutine(ShowWinner(player));
    }

    IEnumerator ShowWinner(Player player)
    {
        winnerMsg.text = "El ganador es " + player.NickName;
        yield return new WaitForSeconds(4);
        StartCoroutine(LeaveGameRoom());
    }

    [PunRPC]
    public void OnEndGame()
    {
        StartCoroutine(LeaveGameRoom());
    }
    
    IEnumerator LeaveGameRoom()
    {
        PhotonNetwork.LeaveRoom();
        while(PhotonNetwork.InRoom)
            yield return null;

        //This is for testing purposes


        Screen.orientation = ScreenOrientation.Portrait;
        PhotonNetwork.LoadLevel("Menu");
    }

    public Transform GetDidWinSpawn(bool didWin)
    {
        if (didWin)
            return playerWinSpawn;
        return playerLoseSpawn;
    }

    public IEnumerator ActivateWinnerScreen(int winnerScore, int loserScore, Player winnerPlayer, Player losePlayer)
    {
        //yield return StartCoroutine(NetworkManager.instance.AddMatchResultsToServer(true));
        NetworkManager.instance.AddMatchResultToLocal(winnerPlayer, losePlayer);

        NetworkManager.instance.ResetPlayerGameProperties();
        yield return new WaitForSeconds(2);
        winScreen.SetActive(true);
        winScoreText.text = winnerScore + " - " + loserScore;
        yield return new WaitForSeconds(3);
        winExitButton.interactable = true;
        if ((bool)PhotonNetwork.CurrentRoom.CustomProperties["isTorneo"])
        {
            NetworkManager.instance.userLogin.duelo_agendado.id = 0;
        }
    }

    public IEnumerator ActivateLoserScreen(int winnerScore, int loserScore, Player winnerPlayer, Player losePlayer)
    {

        //yield return StartCoroutine(NetworkManager.instance.AddMatchResultsToServer(false));
        NetworkManager.instance.AddMatchResultToLocal(winnerPlayer, losePlayer);

        NetworkManager.instance.ResetPlayerGameProperties();
        yield return new WaitForSeconds(2);
        loseScreen.SetActive(true);
        LoseScoreText.text = winnerScore + " - " + loserScore;
        yield return new WaitForSeconds(3);
        loseExitButton.interactable = true;
        if ((bool)PhotonNetwork.CurrentRoom.CustomProperties["isTorneo"])
        {
            NetworkManager.instance.userLogin.duelo_agendado.id = 0;
        }
    }

    public void SuccessOnDueloAgendadoRequest()
    {
        photonView.RPC("ActivateExitButtons", RpcTarget.All);
    }

    [PunRPC]
    public void ActivateExitButtons()
    {
        NetworkManager.instance.userLogin.duelo_agendado.id = 0;
        winExitButton.interactable = true;
        loseExitButton.interactable = true;
    }
}
