using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    [Header("Interfaz Screens")]
    public GameObject uniformeScreen;
    public GameObject peinadosScreen;
    public GameObject characterViewsScreen;
    public GameObject emblemaScreen;

    [Header("Index stats")]
    public int indexOfPlayer;
    public int tmp_indexofPlayer;
    public int indexOfHaircut;
    public int tmp_indexOfHaircut;
    public int indexOfEmblema;
    public int tmp_indexOfEmblema;
    public bool isInitializing;
    public Button player_applyButton;
    public Button haircut_applyButton;
    public Button icon_applyButton;
    public MenuCharacter[] menu_players;
    public Animator[] player_animators;
    public AudioSource applyCharacter;

    public static InterfaceManager instance;

    private void Awake()
    {
        instance = this;
        isInitializing = true;
        Initialize();
    }

    private void Start()
    {
        indexOfPlayer = tmp_indexofPlayer = NetworkManager.instance.kicker_index;
        indexOfHaircut = tmp_indexOfHaircut = NetworkManager.instance.kicker_haircutIndex;
        indexOfEmblema = tmp_indexOfEmblema = NetworkManager.instance.emblemaIndex;
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
        if (indexOfPlayer == tmp_indexofPlayer)
            player_applyButton.interactable = false;
        else
            player_applyButton.interactable = true;
    }

    public void SelectHaircut(int index)
    {
        foreach (MenuCharacter menu_char in menu_players)
        {
            menu_char.SelectHaircut(index);
        }
        tmp_indexOfHaircut = index;
        if (indexOfHaircut == tmp_indexOfHaircut)
            haircut_applyButton.interactable = false;
        else
            haircut_applyButton.interactable = true;
    }

    public void SelectEmblema(int index)
    {
        tmp_indexOfEmblema = index;
        if (indexOfEmblema == tmp_indexOfEmblema)
            icon_applyButton.interactable = false;
        else
            icon_applyButton.interactable = true;
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

    public void OnApplyUniformeButton()
    {
        player_applyButton.interactable = false;
        indexOfPlayer = tmp_indexofPlayer;
        player_animators[indexOfPlayer].SetBool("applyButton", true);
        NetworkManager.instance.kicker_index = indexOfPlayer;
        if (applyCharacter.isPlaying)
            applyCharacter.Stop();
        applyCharacter.Play();
    }

    public void OnApplyPeinadoButton()
    {
        haircut_applyButton.interactable = false;
        indexOfHaircut = tmp_indexOfHaircut;
        player_animators[indexOfPlayer].SetBool("applyButton", true);
        NetworkManager.instance.kicker_haircutIndex = indexOfHaircut;
        if (applyCharacter.isPlaying)
            applyCharacter.Stop();
        applyCharacter.Play();
    }

    public void OnApplyEmblemaButton()
    {
        icon_applyButton.interactable = false;
        indexOfEmblema = tmp_indexOfEmblema;
        NetworkManager.instance.emblemaIndex = indexOfEmblema;
        if (applyCharacter.isPlaying)
            applyCharacter.Stop();
        applyCharacter.Play();
    }

    public void OnBackButton()
    {
        tmp_indexofPlayer = indexOfPlayer;
        SelectCharacter(indexOfPlayer);
        player_applyButton.interactable = false;

    }

}
