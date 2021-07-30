using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuTutorial : MonoBehaviour
{
    [Header("InterfaceButtons")]
    public List<Button> characterButtons;
    public List<Button> haircutButtons;
    public List<Button> emblemaButtons;
    private List<List<Button>> interfButtonsList;

    int interfaceStep = 1;
    private List<string> DialogList = new List<string>() 
    { 
        "Elige un personaje",
        "Selecciona un corte",
        "Escoge un emblema",
        "Presiona el Botón \"Aplicar\""
    };

    [Header("UI Screens")]
    public GameObject interfaceDialog;
    public Button interfDialogButton;
    public TextMeshProUGUI interfText;

    public static MenuTutorial instance;
    public Menu menuManager;
    public InterfaceManager interfaceManager;

    private void Awake()
    {
        instance = this;
    }
    public void Initialize()
    {
         interfButtonsList = new List<List<Button>>()
         {
             characterButtons,
             haircutButtons,
             emblemaButtons
         };

        DialogEvents();
        interfaceDialog.SetActive(true);
    }

    private void DialogEvents()
    {
        foreach(Button charBut in characterButtons)
        {
            charBut.onClick.AddListener(() =>
            {
                if (!interfDialogButton.interactable)
                {
                    interfDialogButton.interactable = true;
                }
            });
        }

        foreach(Button haircutBut in haircutButtons)
        {
            haircutBut.onClick.AddListener(() =>
            {
                if (!interfDialogButton.interactable)
                {
                    interfDialogButton.interactable = true;
                }
            });
        }

        foreach(Button emblemBut in emblemaButtons)
        {
            emblemBut.onClick.AddListener(() =>
            {
                if (!interfDialogButton.interactable)
                {
                    interfDialogButton.interactable = true;
                }
            });
        }
    }

    private void RemoveListenersOf(int index)
    {
        foreach(Button tmpBut in interfButtonsList[index])
        {
            tmpBut.onClick.RemoveAllListeners();
        }
    }

    public void GoNextInterfaceWindow()
    {
        interfText.text = DialogList[interfaceStep];
        if (interfaceStep <= 2)
            RemoveListenersOf(interfaceStep - 1);

        if(interfaceStep == 1)
        {
            interfaceManager.OnPeinadosButton();
        }
        else if(interfaceStep == 2){
            interfaceManager.OnEmblemaButton();
        }
        else
        {
            interfaceDialog.SetActive(true);
            menuManager.SetScreen(menuManager.playerScreen);
        }
        interfaceStep += 1;
    }
}
