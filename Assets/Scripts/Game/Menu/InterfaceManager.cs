using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using AccountModels;

public class InterfaceManager : MonoBehaviour
{
    [Header("Interfaz Screens")]
    public GameObject uniformeScreen;
    public GameObject peinadosScreen;
    public GameObject characterViewsScreen;
    public GameObject emblemaScreen;

    private Player player;

    [Header("Index stats")]
    public int indexOfPlayer;
    public int tmp_indexofPlayer;
    public int indexOfHaircut;
    public int tmp_indexOfHaircut;
    public int indexOfEmblema;
    public int tmp_indexOfEmblema;
    public bool isInitializing;
    public Button player_applyButton;
    public MenuCharacter[] menu_players;
    public Animator[] player_animators;
    public AudioSource applyCharacter;

    public static InterfaceManager instance;
    public UserLogin userLogin;
    public NetworkManager networkManager;

    private void Awake()
    {
        instance = this;
        networkManager = NetworkManager.instance;
        userLogin = NetworkManager.instance.userLogin;
        isInitializing = true;
        Initialize();
    }

    private void Start()
    {
        player = PhotonNetwork.LocalPlayer;
        indexOfPlayer = tmp_indexofPlayer = userLogin.player;
        indexOfHaircut = tmp_indexOfHaircut = userLogin.haircut_player;
        indexOfEmblema = tmp_indexOfEmblema = userLogin.emblema;
        SelectCharacter(indexOfPlayer);
        SelectHaircut(indexOfHaircut);
        SelectEmblema(indexOfEmblema);
    }

    void Initialize()
    {
        player_animators = new Animator[menu_players.Length];
        for(int i = 0; i < menu_players.Length; i++)
        {
            Animator tmp_animator = menu_players[i].gameObject.GetComponent<Animator>();
            player_animators[i] = tmp_animator;
        }
    }

    public void SelectCharacter(int index)
    {
        foreach (MenuCharacter menu_char in menu_players)
        {
            if (menu_char.indexOfplayerM == index)
            {
                menu_char.gameObject.SetActive(true);
                tmp_indexofPlayer = index;
            }
            else
                menu_char.gameObject.SetActive(false);
        }
        player_applyButton.interactable = !checkApplyButton();
    }

    public bool checkApplyButton()
    {
        return tmp_indexOfEmblema == indexOfEmblema && tmp_indexOfHaircut == indexOfHaircut && tmp_indexofPlayer == indexOfPlayer;
    }

    public void SelectHaircut(int index)
    {
        foreach (MenuCharacter menu_char in menu_players)
        {
            menu_char.SelectHaircut(index);
        }
        tmp_indexOfHaircut = index;
        player_applyButton.interactable = !checkApplyButton();
    }

    public void SelectEmblema(int index)
    {
        tmp_indexOfEmblema = index;
        player_applyButton.interactable = !checkApplyButton();
    }

    public void DeactivateInterfaceScreens()
    {
        uniformeScreen.SetActive(false);
        peinadosScreen.SetActive(false);
        characterViewsScreen.SetActive(false);
        emblemaScreen.SetActive(false);
    }

    public void OnUniformeButton()
    {
        if(!uniformeScreen.activeSelf)
        {
            DeactivateInterfaceScreens();
            characterViewsScreen.SetActive(true);
            uniformeScreen.SetActive(true);
        }
    }

    public void OnPeinadosButton()
    {
        if(!peinadosScreen.activeSelf)
        {
            DeactivateInterfaceScreens();
            characterViewsScreen.SetActive(true);
            peinadosScreen.SetActive(true);
        }
    }

    public void OnEmblemaButton()
    {
        if(!emblemaScreen.activeSelf)
        {
            DeactivateInterfaceScreens();
            emblemaScreen.SetActive(true);
        }
    }

    public void OnApplyInterfazButton()
    {
        player_applyButton.interactable = false;

        indexOfPlayer = tmp_indexofPlayer;
        indexOfHaircut = tmp_indexOfHaircut;
        indexOfEmblema = tmp_indexOfEmblema;

        userLogin.player = indexOfPlayer;
        userLogin.haircut_player = indexOfHaircut;
        userLogin.emblema = indexOfEmblema;

        player_animators[indexOfPlayer].SetBool("applyButton", true);
        player.CustomProperties["KickerHaircutIndex"] = indexOfHaircut;
        player.CustomProperties["EmblemaIndex"] = indexOfEmblema;

        if (applyCharacter.isPlaying)
            applyCharacter.Stop();
        applyCharacter.Play();

        if(networkManager.isConnected)
        {
            StartCoroutine(NetworkAPICalls.instance.UpdateInterfazData(
                indexOfEmblema,
                indexOfHaircut,
                indexOfPlayer,
                networkManager.userToken.id,
                networkManager.userToken.token
                ));
        }
    }

    public void OnBackButton()
    {
        tmp_indexofPlayer = indexOfPlayer;
        SelectCharacter(indexOfPlayer);
        player_applyButton.interactable = false;
    }

}
