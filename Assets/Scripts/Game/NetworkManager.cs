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
    [Header("Screens")]
    public GameObject loginScreen;
    public GameObject playerScreen;

    [Header("Estadisticas")]
    public UserInfo userInfo;

    public Button playButton;
    public static NetworkManager instance;
    public int maxPlayers;
    string regUri = "http://localhost/JuegoPenales/registerUser.php";
    string logUri = "http://localhost/JuegoPenales/loginUser.php";
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

        using (UnityWebRequest webRequest = UnityWebRequest.Post(regUri, form))
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

    public IEnumerator LoginUser(string username, string password, TextMeshProUGUI status)
    {
        WWWForm form = new WWWForm();
        form.AddField("logUser", username);
        form.AddField("logPass", password);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(logUri, form))
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
                string resp = webRequest.downloadHandler.text;
                status.text = resp;
                status.color = Color.green;
                if (!status.text.Contains("Error"))
                {
                    userInfo = JsonUtility.FromJson<UserInfo>(resp);
                    Debug.Log(userInfo.nombre);
                    yield return new WaitForSeconds(2);
                    loginScreen.SetActive(false);
                    playerScreen.SetActive(true);
                }
            }
        }
    }

    public class UserInfo
    {
        public int id_user;
        public int id_torneo;
        public bool isadmin;
        public string nombre;
        public string username;
        public string email;
        public bool email_verified;
        public string password;
        public int total_partidos;
        public int partidos_ganados;
        public int partidos_perdidos;
        public int goles_anotados;
        public int goles_atajados;
        public int goles_recibidos;
        public int posicion_ranking;
    }
}
