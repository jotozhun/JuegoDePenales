using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public Button playButton;
    public static NetworkManager instance;
    public int maxPlayers;
    string uri = "http://localhost/JuegoPenales/registerUser.php";
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            gameObject.SetActive(false);
        }
        else
        {
            maxPlayers = 2;
            instance = this;
            Screen.orientation = ScreenOrientation.Portrait;
            DontDestroyOnLoad(this);
        }
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        playButton.interactable = true;
    }

    public void CreateRoom(string roomName)
    {
        RoomOptions options = new RoomOptions();

        options.MaxPlayers = (byte)maxPlayers;

        PhotonNetwork.JoinOrCreateRoom(roomName, options, null, null);
    }

    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
        Screen.orientation = ScreenOrientation.Landscape;
    }

    public IEnumerator RegisterUser(string name, string username, string email, string password, TextMeshProUGUI status)
    {
        WWWForm form = new WWWForm();
        form.AddField("regName", name);
        form.AddField("regUser", username);
        form.AddField("regEmail", email);
        form.AddField("regPass", password);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(uri, form))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                status.text = webRequest.error;
                status.color = Color.red;
            }
            else
            {
                status.text = webRequest.downloadHandler.text;
                status.color = Color.green;
            }
        }
    }
}
