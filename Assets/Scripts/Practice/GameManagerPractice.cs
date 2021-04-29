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

    [Header("Game Settings")]
    public GameObject[] goalBounds;
    public GameObject[] missedGoalBounds;

    [Header("Animators")]
    public Animator goalAnim;
    public Animator missedGoalAnim;
    public Animator targetAnim;

    public int[] scores;
    private TimerPractice timScript;
    private CountKicks kickScipt;
    private TargetPractice targetScipt;
    public PlayerControllerPractice playerScript;

    public bool markGoal;
    public bool missGoal;
    public int numberKicks;            //Change the number kicks of players
    public Button backPracticeButton;
    public Button levelNormalButtom;
    public Button level1Buttom;
    public Button level2Buttom;


    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        backPracticeButton.interactable = true;
        levelNormalButtom.interactable = true;
        level1Buttom.interactable = true;
        level2Buttom.interactable = true;
        markGoal = false;
        missGoal = false;
        GameUIPractice.instance.players = new PlayerControllerPractice[1];
        scores = new int[1];
        ImInGame();
        //timScript = GameUIPractice.instance.time.GetComponent<TimerPractice>();
        kickScipt = GameUIPractice.instance.kick.GetComponent<CountKicks>();
        targetScipt = GameUIPractice.instance.target.GetComponent<TargetPractice>();
        //playerScript = GameUIPractice.instance.playerObject.GetComponent<PlayerControllerPractice>();
        targetScipt.Start();
    }

    void ImInGame()
    {
        SpawnPlayers();

    }

    void SpawnPlayers()
    {
        int indexOfKicker = NetworkManager.instance.userLogin.player;
        GameObject playerObj = Instantiate(GameUIPractice.instance.playersPrefabLocation[indexOfKicker], Vector3.one, Quaternion.identity);
        
        PlayerControllerPractice playerScript = playerObj.GetComponent<PlayerControllerPractice>();
        
        //initialize the player
        playerScript.Initialize();
    }

    //Goal and missed Goals
    public void MarkGoalToPlayer()
    {
        markGoal = true;
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
        //kickScipt.DecreaseKicks();
        numberKicks++;
        GameUIPractice.instance.celebrationGoalSound.Stop();
        GameUIPractice.instance.missedGoalSound.Play();
        StartCoroutine(DeactivateMissedGoalBounds());
    }

    public void MarkTargetToPlayer()
    {
        markGoal = true;
        //kickScipt.DecreaseKicks();
        numberKicks++;
        scores[0]++;
        playerScoresUI[0].text = scores[0].ToString();
        StartCoroutine(DeactivateTargetBounds());
    }

    public void SwitchPositions()
    {
        markGoal = false;
        missGoal = false;
        PlayerControllerPractice player = playerScript;
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
    }

    public void spawnAsKicker(PlayerControllerPractice player)
    {
        player.isGoalKeeper = false;
        //player.anim.SetBool("isGoalKeeper", false);
        player.gameObject.transform.position = GameUIPractice.instance.kickerSpawn.position;
        player.gameObject.transform.rotation = GameUIPractice.instance.kickerSpawn.rotation;
        player.ball.SetActive(true);
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

    IEnumerator DeactivateTargetBounds()
    {
        DeactivateBounds();
        targetAnim.SetTrigger("goal");
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

    public void OnGetOutPractice()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        SceneManager.LoadScene("Menu");
    }
}
