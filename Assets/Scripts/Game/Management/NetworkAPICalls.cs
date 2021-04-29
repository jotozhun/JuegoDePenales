using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using AccountModels;
using TMPro;

public class NetworkAPICalls : MonoBehaviour
{
    [Header("UI elements")]
    public TextMeshProUGUI loginStatus;
    public TextMeshProUGUI registerStatus;

    
    public NetworkManager manager;
    public static NetworkAPICalls instance;

    private void Awake()
    {
        instance = this;
    }

    public void TokenTest()
    {
        StartCoroutine(CallUserToken("Jopoelpe", "admin"));
    }

    public IEnumerator CallUserToken(string username, string password)
    {
        string testUri = "https://willymedi.pythonanywhere.com/usuarios/usuario/auth/";
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(testUri, form))
        {
            yield return webRequest.SendWebRequest();

            UserToken token = null;
            if (webRequest.isNetworkError)
            {
                token = new UserToken(600);
            }
            else
            {
                if (webRequest.responseCode == 200)
                {
                    string resp = webRequest.downloadHandler.text;
                    token = JsonUtility.FromJson<UserToken>(resp);
                    token.statusCode = 200;
                }
                else if (webRequest.responseCode == 403)
                {
                     token = new UserToken(403);

                }
                else
                {
                    token = new UserToken((int)webRequest.responseCode);
                }
            }
            manager.userToken = token;
        }
    }

    public IEnumerator GetLoginInfo(int id, string token)
    {
        string testUri = "https://willymedi.pythonanywhere.com/usuarios/usuario/" + id;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(testUri))
        {
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", "Bearer " + token);
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {
                Debug.Log("Error en la red, intente mas tarde");
            }
            else
            {
                UserLogin userLogin = null;
                if (webRequest.responseCode == 200)
                {
                    string resp = webRequest.downloadHandler.text;
                    userLogin = JsonUtility.FromJson<UserLogin>(resp);
                    userLogin.statusCode = 200;
                }
                else
                {
                    userLogin = new UserLogin((int)webRequest.responseCode);
                }
                manager.userLogin = userLogin;
            }
        }
    }

    public IEnumerator UpdateInterfazData(int emblemaIndex, int haircut_kicker, int kicker_index, int id, string token)
    {
        string testUri = "https://willymedi.pythonanywhere.com/usuarios/usuario/" + id;
        InterfazInfo interfazInfo = new InterfazInfo { emblema = emblemaIndex, haircut_player = haircut_kicker, player = kicker_index };
        string rawInterfazInfo = JsonUtility.ToJson(interfazInfo);
        using (UnityWebRequest webRequest = UnityWebRequest.Put(testUri, System.Text.Encoding.UTF8.GetBytes(rawInterfazInfo)))
        {
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", "Bearer " + token);
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {

            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
            }
        }
    }

    public IEnumerator AddMatchResultsToPlayer(bool isWin, int id, int goles_anotados, int goles_atajados, int goles_recibidos, string token)
    {
        string testUri = "https://willymedi.pythonanywhere.com/usuarios/usuario/" + id;

        manager.AddLocalMatchResult(isWin, goles_anotados, goles_atajados, goles_recibidos);

        UserLogin tmpUser = manager.userLogin;

        MatchResult matchResult = new MatchResult() {
            total_partidos = tmpUser.total_partidos,
            partidos_ganados = tmpUser.partidos_ganados,
            partidos_perdidos = tmpUser.partidos_perdidos,
            goles_anotados = tmpUser.goles_anotados,
            goles_atajados = tmpUser.goles_atajados,
            goles_recibidos = tmpUser.goles_recibidos
        };

        string rawMatchResult = JsonUtility.ToJson(matchResult);
        using (UnityWebRequest webRequest = UnityWebRequest.Put(testUri, System.Text.Encoding.UTF8.GetBytes(rawMatchResult)))
        {
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", "Bearer " + token);
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {

            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
            }
        }
    }

    public IEnumerator RegisterUser(string name, string username, string email, string password)
    {
        string testUri = "https://willymedi.pythonanywhere.com/usuarios/register/";

        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("username", username);
        form.AddField("email", email);
        form.AddField("password", password);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(testUri, form))
        {
            
            yield return webRequest.SendWebRequest();
            if(webRequest.isNetworkError)
            {
                manager.responses.registerCode = 600;
            }
            else
            {
                if(webRequest.responseCode == 200)
                {
                    manager.responses.registerCode = 200;
                }
            }

        }
    }
}
