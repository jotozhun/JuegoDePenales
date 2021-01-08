using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManagerPractice : MonoBehaviour
{
    [HideInInspector]
    public static GameManagerPractice instance;

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


    public int[] scores;

    private TimerPractice timScript;

    private CountKicks kickScipt;
    public PlayerControllerPractice playerScript;
    public bool markGoal;
    public bool missGoal;

    public int numberKicks;            //Change the number kicks of players

    public Button backPracticeButton;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        backPracticeButton.interactable = true;
        markGoal = false;
        missGoal = false;
        GameUIPractice.instance.players = new PlayerControllerPractice[1];
        scores = new int[1];
        //photonView.RPC("ImInGame", RpcTarget.AllBuffered);
        ImInGame();
        timScript = GameUIPractice.instance.time.GetComponent<TimerPractice>();
        kickScipt = GameUIPractice.instance.kick.GetComponent<CountKicks>();
        //playerScript = GameUIPractice.instance.playerObject.GetComponent<PlayerControllerPractice>();
    }

    void ImInGame()
    {
        //playersInGame++;
        SpawnPlayers();
        /*if(PhotonNetwork.IsMasterClient && playersInGame == PhotonNetwork.PlayerList.Length)
        {
            photonView.RPC("SpawnPlayers", RpcTarget.All);
        }*/
    }

    void SpawnPlayers()
    {
        //Vector3 pos = new Vector3((float)-85.1, (float)-1.51, (float)-79.22);
        //Vector3 pos2 = new Vector3((float)-87.14, (float)-1.51, (float)-79.22);
        GameObject playerObj = Instantiate(GameUIPractice.instance.playerPrefabLocation, Vector3.one, Quaternion.identity);
        //Instantiate(GameUIPractice.instance.stadiumPrefabLocation, pos2, Quaternion.identity);
        //GameUIPractice.instance.stadiumPrefabLocation.SetActive(true);
        //Instantiate(GameUIPractice.instance.groundPrefabLocation, pos2, Quaternion.identity);
        PlayerControllerPractice playerScript = playerObj.GetComponent<PlayerControllerPractice>();

        //initialize the player
        playerScript.Initialize();
        //timScript.Start();
        //kickScipt.Start();
    }

    //Goal and missed Goals
    public void MarkGoalToPlayer()
    {
        markGoal = true;
        //Physics2D.gravity = new Vector2(0f, -7.51f);
        //kickScipt.DecreaseKicks();
        numberKicks++;
        scores[0]++;
        playerScoresUI[0].text = scores[0].ToString();
        GameUIPractice.instance.celebrationGoalSound.Play();
        GameUIPractice.instance.missedGoalSound.Stop();
        StartCoroutine(DeactivateGoalBounds());
    }

    public void MarkGoalMissedToPlayer()
    {
        missGoal = true;
        //Physics2D.gravity = new Vector2(0f, -7.51f);
        //kickScipt.DecreaseKicks();
        numberKicks++;
        GameUIPractice.instance.celebrationGoalSound.Stop();
        GameUIPractice.instance.missedGoalSound.Play();
        StartCoroutine(DeactivateMissedGoalBounds());
    }

    public void SwitchPositions()
    {
        markGoal = false;
        missGoal = false;
        PlayerControllerPractice player = playerScript;
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

    public void spawnAsGoalKeeper(PlayerControllerPractice player)
    {
        player.isGoalKeeper = true;
        //player.anim.SetBool("isGoalKeeper", true);
        player.gameObject.transform.position = GameUIPractice.instance.goalKeeperSpawn.position;
        player.gameObject.transform.rotation = GameUIPractice.instance.goalKeeperSpawn.rotation;
        player.ball.SetActive(false);
        player.canCover = true;

        timScript.R();
        timScript.StartTime();
        //kickScipt.RestartKicks();
        timScript.keeper = true;
    }

    public void spawnAsKicker(PlayerControllerPractice player)
    {
        /*GameUIPractice.instance.playerObject.playerCanCover = false;
        GameUIPractice.instance.playerObject.isGoalKeeper = false;
        GameUIPractice.instance.playerObject.canCover = false;
        //player.anim.SetBool("isGoalKeeper", false);
        GameUIPractice.instance.playerObject.gameObject.transform.position = GameUIPractice.instance.kickerSpawn.position;
        GameUIPractice.instance.playerObject.gameObject.transform.rotation = GameUIPractice.instance.kickerSpawn.rotation;
        GameUIPractice.instance.playerObject.ball.SetActive(true);
        //timScript.R();
        //timScript.StartTime();
        //kickScipt.RestartKicks();
        //timScript.keeper = false;*/
        player.playerCanCover = false;
        player.isGoalKeeper = false;
        player.canCover = false;
        //player.anim.SetBool("isGoalKeeper", false);
        player.gameObject.transform.position = GameUIPractice.instance.kickerSpawn.position;
        player.gameObject.transform.rotation = GameUIPractice.instance.kickerSpawn.rotation;
        player.ball.SetActive(true);
        //timScript.R();
        //timScript.StartTime();
        //kickScipt.RestartKicks();
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

    public void stopTime()
    {
        timScript.StopTime();
    }
    
    public bool activateSwitch()
    {
        //Debug.Log("kicks:" + kickScipt.numKicks);
        if (kickScipt.numKicks == 0)
        {
            return true;
        }
        return false;
    }
    
    public void decreaseKicksCount()
    {
        kickScipt.DecreaseKicks();
    }

    public void restartKicksCount()
    {
        kickScipt.RestartKicks();
    }
    
    public void restartTime()
    {
        timScript.R();
        timScript.StartTime();
    }

    public void keeperCanCover(bool cover)
    {
        PlayerControllerPractice player = playerScript;
        if (player.isGoalKeeper)
        {
            player.playerCanCover = cover;
        }
    }

    public void OnGetOutPractice()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        SceneManager.LoadScene("Menu");
    }
}
